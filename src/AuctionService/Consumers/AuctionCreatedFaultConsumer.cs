using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine("--> Received AuctionCreated Fault");

        var exception = context.Message.Exceptions.FirstOrDefault();
        if (exception is not null && exception.ExceptionType == "System.ArgumentException")
        {
            context.Message.Message.Model = "FooBar";
            await context.Publish(context.Message.Message);
            Console.WriteLine("--> Published corrected AuctionCreated event");
        }
        else
        {
            Console.WriteLine("--> Unhandled exception type, not republishing");
        }
    }
}