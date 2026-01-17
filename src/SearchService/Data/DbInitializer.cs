using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        var db = await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")!));

        await db.Index<Item>()
            .Key(i => i.Make, KeyType.Text)
            .Key(i => i.Model, KeyType.Text)
            .Key(i => i.Color, KeyType.Text)
            .CreateAsync();

        var count = await db.CountAsync<Item>();

        using var scope = app.Services.CreateScope();

        var auctionClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();
        var items = await auctionClient.GetItemsForSearchDb();

        if (items is not null && items.Count > 0)
        {
            Console.WriteLine($"Seeding {items.Count} items into SearchDb...");
            await db.SaveAsync(items);
        }
    }
}