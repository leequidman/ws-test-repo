using Common.Models;
using Common.Models.Requests.Abstract;

namespace Common.EventHandling;

public interface IEventParser
{
    public EventType EventType { get; }
    public IEventData Parse(string jsonString);
}