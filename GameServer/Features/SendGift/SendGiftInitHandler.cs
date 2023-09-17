using System.Net.WebSockets;
using Common.EventHandling;
using Common.Models;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.GiftReceived;
using Common.Models.Requests.SendGift;
using Common.Transport;
using GameServer.Repositories.Models;
using GameServer.Services;
using ILogger = Serilog.ILogger;

namespace GameServer.Features.SendGift;

public class SendGiftInitHandler : IEventHandler
{
    private readonly SendGiftInitEventDataProcessor _dataProcessor = new();
    private readonly IPlayersService _playersService;
    private readonly IWebSocketHandler _webSocketHandler;
    private readonly IConnectionService _connectionService;
    private readonly ILogger _logger;

    public SendGiftInitHandler(IPlayersService playersService, IWebSocketHandler webSocketHandler, IConnectionService connectionService,
        ILogger logger)
    {
        _playersService = playersService;
        _webSocketHandler = webSocketHandler;
        _connectionService = connectionService;
        _logger = logger;
    }

    public EventType EventType => EventType.SendGiftInit;


    public async Task Handle(object? eventData, WebSocket ws)
    {
        var data = _dataProcessor.PrepareEventData(eventData);

        IEvent resultEvent;

        var (isTransactionPossible, resultData, error) = await IsTransactionPossible(data);
        if (!isTransactionPossible)
        {
            resultEvent = new SendGiftFailureEvent(new(error!));
            await _webSocketHandler.SendEvent(ws, resultEvent);
            return;
        }

        var (sender, senderAmount, receiver, receiverAmount) =
            ((Player sender, int senderAmount, Player receiver, int receiverAmount))resultData!;

        await _playersService.TransferResources(sender, receiver, data.Resource, data.Amount);
        resultEvent = new SendGiftSuccessEvent(
            new(data.SenderId,
                data.ReceiverId,
                data.Resource,
                senderAmount - data.Amount,
                receiverAmount + data.Amount));
        await _webSocketHandler.SendEvent(ws, resultEvent);


        var isReceiverOnline = _connectionService.IsOnline(data.ReceiverId);
        if (isReceiverOnline)
        {
            var receiverWs = _connectionService.GetWebSocket(data.ReceiverId);
            if (receiverWs != null) // mb it was closed and deleted between method calls 
            {
                var giftReceivedEvent = new GiftReceivedEvent(
                    new(data.SenderId,
                        data.ReceiverId,
                        data.Resource,
                        data.Amount,
                        receiverAmount + data.Amount));
                _logger.Information($"Receiver '{data.ReceiverId}' is online, notifying him");
                await _webSocketHandler.SendEvent(receiverWs, giftReceivedEvent);
            }
        }
    }

    // better use TResult pattern, but will require too many boilerplate code for 1 call
    private async Task<(bool isTransactionPossible, object? result, string? error)> IsTransactionPossible(SendGiftInitEventData data)
    {
        var sender = await _playersService.FindPlayer(data.SenderId);
        if (sender is null)
            return (false, null, $"Player {data.SenderId} not found, can't send gift from him");

        var receiver = await _playersService.FindPlayer(data.ReceiverId);
        if (receiver is null)
            return (false, null, $"Player {data.ReceiverId} not found, can't send gift to him");

        if (!sender.Resources.TryGetValue(data.Resource, out var senderAmount) || senderAmount < data.Amount)
            return (false, null, $"Player '{data.SenderId}' has '{senderAmount}' " +
                                 $"of '{data.Resource}', that's not enough to send '{data.Amount}' as gift");

        if (!receiver.Resources.TryGetValue(data.Resource, out var receiverAmount))
        {
            await _playersService.UpdateResources(data.ReceiverId, data.Resource, 0);
            receiverAmount = 0;
        }

        unchecked
        {
            if (receiverAmount + data.Amount < 0) // int overflow   
                return (false, null, $"Player '{data.ReceiverId}' has too much of '{data.Resource}' ('{receiverAmount}')," +
                                     " keep this gift to yourself");
        }

        (Player sender, int senderAmount, Player receiver, int receiverAmount) resultData =
            (sender, senderAmount, receiver, receiverAmount);
        return (true, resultData, null);
    }
}