using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionCreatedConsumer(DB Db, IMapper mapper) : IConsumer<AuctionCreated>
{
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine("--> Consumer Auction created" + context.Message.Id);
        var item = mapper.Map<Item>(context.Message);
        
        await Db.SaveAsync(item);
    }
}