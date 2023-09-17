using Common.Models;

namespace GameServer.Transport;

public interface IEventParser
{
    public EventType EventType { get; }
    public IEventData Parse(string jsonString);

    public static string BuildErrorMessage(string jsonString, string additionalMessage, EventType eventType) =>
        $"Invalid data format for EventType '{eventType}'. {additionalMessage}" +
        $"{Environment.NewLine}{jsonString}: {jsonString}";
}