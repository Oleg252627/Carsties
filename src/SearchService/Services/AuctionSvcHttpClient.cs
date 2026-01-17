using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSvcHttpClient(HttpClient httpClient, IConfiguration configuration, DB db)
{
    public async Task<List<Item>?> GetItemsForSearchDb()
    {
        var lastUpdated = await db.Find<Item, string>()
        .Sort(z => z.Descending(i => i.UpdatedAt))
        .Project(z => z.UpdatedAt.ToString())
        .ExecuteFirstAsync();

        return await httpClient.GetFromJsonAsync<List<Item>>(
            $"{configuration["AuctionServiceUrl"]!}/api/auctions?date={lastUpdated}");
    }
}