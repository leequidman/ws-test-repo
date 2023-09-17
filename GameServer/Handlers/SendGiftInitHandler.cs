using System.Net.WebSockets;
using System.Security.AccessControl;
using Common.EventHandling;
using Common.Models;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.GiftReceived;
using Common.Models.Requests.SendGift;
using Common.Models.Requests.UpdateResources;
using Common.Transport;
using GameServer.EventDataProcessing;
using GameServer.Services;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace GameServer.Handlers;

public class SendGiftInitHandler : IEventHandler
{
    private readonly SendGiftInitEventDataProcessor _dataProcessor = new();
    private readonly IPlayersService _playersService;
    private readonly IWebSocketHandler _webSocketHandler;
    private readonly IConnectionService _connectionService;
    private readonly ILogger _logger;       

    public SendGiftInitHandler(IPlayersService playersService, IWebSocketHandler webSocketHandler, IConnectionService connectionService, ILogger logger)
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
        
        var sender = await _playersService.FindPlayer(data.SenderId);
        if (sender is null)
        {
            resultEvent = new SendGiftFailureEvent(new($"Player {data.SenderId} not found, can't send gift from him"));
            await _webSocketHandler.SendEvent(ws, resultEvent);
            return;
        }
        var receiver = await _playersService.FindPlayer(data.ReceiverId);
        if (receiver is null)
        {
            resultEvent = new SendGiftFailureEvent(new($"Player {data.ReceiverId} not found, can't send gift to him"));
            await _webSocketHandler.SendEvent(ws, resultEvent);
            return;
        }

        if (!sender.Resources.TryGetValue(data.Resource, out var senderAmount) || senderAmount < data.Amount)
        {
            resultEvent = new SendGiftFailureEvent(new($"Player '{data.SenderId}' has '{senderAmount}' " +
                                                       $"of '{data.Resource}', that's not enough to send '{data.Amount}' as gift"));
            await _webSocketHandler.SendEvent(ws, resultEvent);
            return;
        }

        if (!receiver.Resources.TryGetValue(data.Resource, out var receiverAmount))
        {
            await _playersService.UpdateResources(data.ReceiverId, data.Resource, 0);
            receiverAmount = 0;
        }

        unchecked
        {
            if (receiverAmount + data.Amount < 0) // int overflow   
            {
                resultEvent = new SendGiftFailureEvent(new(
                    $"Player '{data.ReceiverId}' has too much of '{data.Resource}' ('{receiverAmount}')," +
                    " keep this gift to yourself"));  
                await _webSocketHandler.SendEvent(ws, resultEvent); 
                return;
            }
        }

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
}