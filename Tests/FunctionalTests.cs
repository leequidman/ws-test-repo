using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;
using Common.Models;
using Common.Models.GiftReceived;
using Common.Models.Login;
using Common.Models.SendGift;
using Common.Models.UpdateResources;
using FluentAssertions;
using GameClient;
using NUnit.Framework;

namespace Tests
{
    [Parallelizable(ParallelScope.All)]
    public class FunctionalTests
    {
        [Test]
        public async Task Should_login_successfully()
        {
            Client client = new(Guid.NewGuid());
            try
            {
                var receiveTask = await client.Connect();

                await client.Login();
                var loginSuccessfulEvent = await GetEventWithDelay<LoginSuccessfulEventData>(client.Messages);
                loginSuccessfulEvent.PlayerId.Should().NotBeEmpty();

                await client.Disconnect();
                await receiveTask;
            }
            catch
            {
                await client.Disconnect();
                throw;
            }
        }

        [Test]
        public async Task Should_fail_sequential_login()
        {
            Client client = new(Guid.NewGuid());
            try
            {
                var receiveTask = await client.Connect();

                await client.Login();
                var loginSuccessfulEvent = await GetEventWithDelay<LoginSuccessfulEventData>(client.Messages);
                loginSuccessfulEvent.PlayerId.Should().NotBeEmpty();

                await client.Login();
                var loginFailedEvent = await GetEventWithDelay<LoginFailedEventData>(client.Messages);
                loginFailedEvent.ErrorMessage.Should().Be($"Player {loginSuccessfulEvent.PlayerId} is already online");

                await client.Disconnect();
                await receiveTask;
            }
            catch
            {
                await client.Disconnect();
                throw;
            }
        }

        [Test]
        public async Task Should_send_gifts()
        {
            Client client1 = new(Guid.NewGuid());
            Client client2 = new(Guid.NewGuid());
            try
            {
                var receiveTask1 = await client1.Connect();

                await client1.Login();
                var player1Id = (await GetEventWithDelay<LoginSuccessfulEventData>(client1.Messages)).PlayerId;


                await client1.UpdateResource(ResourceType.Coins, 100);
                var updateResourceSuccessEvent = await GetEventWithDelay<UpdateResourceSuccessEventData>(client1.Messages);
                updateResourceSuccessEvent.NewAmount.Should().Be(100);


                var receiveTask2 = await client2.Connect();
                await client2.Login();
                var player2Id = (await GetEventWithDelay<LoginSuccessfulEventData>(client2.Messages)).PlayerId;


                await client2.UpdateResource(ResourceType.Coins, 2);
                var updateResourceSuccessEvent2 = await GetEventWithDelay<UpdateResourceSuccessEventData>(client2.Messages);
                updateResourceSuccessEvent2.NewAmount.Should().Be(2);

                await client1.SendGift(player1Id, player2Id, ResourceType.Coins, 1);
                var sendGiftSuccessEvent = await GetEventWithDelay<SendGiftSuccessEventData>(client1.Messages);
                sendGiftSuccessEvent.SenderId.Should().Be(player1Id);
                sendGiftSuccessEvent.SenderCurrentAmount.Should().Be(99);
                sendGiftSuccessEvent.ReceiverId.Should().Be(player2Id);
                sendGiftSuccessEvent.ReceiverCurrentAmount.Should().Be(3);
                sendGiftSuccessEvent.Resource.Should().Be(ResourceType.Coins);

                var giftReceivedEvent = await GetEventWithDelay<GiftReceivedEventData>(client2.Messages);
                giftReceivedEvent.SenderId.Should().Be(player1Id);
                giftReceivedEvent.ReceiverId.Should().Be(player2Id);
                giftReceivedEvent.Resource.Should().Be(ResourceType.Coins);
                giftReceivedEvent.AmountSent.Should().Be(1);
                giftReceivedEvent.ReceiverNewAmount.Should().Be(3);


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

        private async Task<TEventType> GetEventWithDelay<TEventType>(ConcurrentQueue<string> queue)
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
}