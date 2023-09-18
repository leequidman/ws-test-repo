using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;
using Common.Models;
using Common.Models.UpdateResources;
using Common.Models.GiftReceived;
using Common.Models.Login;
using Common.Models.SendGift;

namespace GameClient;

/// <summary>
/// Used for local debugging server
/// </summary>
internal class Program
{
    static async Task Main()
    {
        while (true)
        {
            Console.Write("Press any key to start");
            Console.ReadKey();
            break;
        }

        Client client1 = new(Guid.NewGuid());
        Client client2 = new(Guid.NewGuid());
        try
        {
            var receiveTask1 = await client1.Connect();

            await client1.Login();
            var loginSuccessfulEvent = await GetEventWithDelay<LoginSuccessfulEventData>(client1.Messages);

            await client1.Login();
            await GetEventWithDelay<LoginFailedEventData>(client1.Messages);

            await client1.UpdateResource(ResourceType.Coins, 100);
            await GetEventWithDelay<UpdateResourceSuccessEventData>(client1.Messages);

            client2 = new(Guid.NewGuid());
            var receiveTask2 = await client2.Connect();
            await client2.Login();
            var loginSuccessfulEvent2 = await GetEventWithDelay<LoginSuccessfulEventData>(client2.Messages);

            await client2.UpdateResource(ResourceType.Coins, 2);
            await GetEventWithDelay<UpdateResourceSuccessEventData>(client2.Messages);

            await client1.SendGift(loginSuccessfulEvent.PlayerId, loginSuccessfulEvent2.PlayerId, ResourceType.Coins, 1);
            await GetEventWithDelay<SendGiftSuccessEventData>(client1.Messages);

            await GetEventWithDelay<GiftReceivedEventData>(client2.Messages);


            await client1.Disconnect();
            await client2.Disconnect();

            await receiveTask1;
            await receiveTask2;
        }
        catch
        {
            await client1.Disconnect();
            await client2.Disconnect();
            throw;
        }
    }

    private static async Task<TEventType> GetEventWithDelay<TEventType>(ConcurrentQueue<string> queue)
    {
        if (!queue.TryDequeue(out var jsonString))
        {
            await Task.Delay(1000);
            if (!queue.TryDequeue(out jsonString))
                throw new("Expected message was not received");
        }

        var jsonObj = JsonNode.Parse(jsonString)!.AsObject();

        var eventData = jsonObj[nameof(IEvent.EventData)];


        var receivedEvent = eventData.Deserialize<TEventType>();
        return receivedEvent!;
    }
}