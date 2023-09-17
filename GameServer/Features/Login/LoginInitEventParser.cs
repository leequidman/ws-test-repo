using System.Text.Json.Nodes;
using Common.EventHandling;
using Common.Models;
using Common.Models.Requests.Abstract;
using Common.Models.Requests.Login;

namespace GameServer.Features.Login;

public class LoginInitEventParser : IEventParser
{
    public EventType EventType => EventType.LoginInit;

    public IEventData Parse(string jsonString)
    {
        var jsonObj = JsonNode.Parse(jsonString)?.AsObject();

        var node = jsonObj![nameof(IEvent.EventData)]?[nameof(LoginInitEventData.DeviceId)];
        if (node == null)
            throw new ArgumentException($"Invalid data format for EventType '{EventType}'." +
                                        $"{Environment.NewLine}{jsonString}: {jsonString}");
        return new LoginInitEventData(Guid.Parse(node.ToString()));
    }
}