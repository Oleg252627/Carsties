using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionUpdatedConsumer(DB dB, IMapper mapper) : IConsumer<AuctionUpdated>
{
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("--> Consumer Auction updated: " + context.Message.Id);

        var item = mapper.Map<Item>(context.Message);

        var result = await dB.Update<Item>()
        .Match(i => i.ID== context.Message.Id)
        .ModifyOnly(x => new
        {
            x.Make,
            x.Model,
            x.Year,
            x.Color,
            x.Mileage
        }, item)
        .ExecuteAsync();

        if (!result.IsAcknowledged)
        {
            throw new MessageException(typeof(AuctionUpdated), "Failed to update auction item in MongoDB");
        }
    }
}