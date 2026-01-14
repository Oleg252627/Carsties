using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

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

        if (count == 0)
        {
            Console.WriteLine("Seeding initial data...");
            var items = await File.ReadAllTextAsync("Data/auctions.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var itemsData = JsonSerializer.Deserialize<List<Item>>(items, options);

            if (itemsData != null && itemsData.Count > 0)
            {
                await db.SaveAsync(itemsData);
                Console.WriteLine("Seeding completed.");
            }
            else
            {
                Console.WriteLine("No data found to seed.");
            }
        }
    }
}