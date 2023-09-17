using Common.Models.Login;
using FluentAssertions;
using GameServer.Features.Login;
using NUnit.Framework;

namespace Tests.UnitTests.ParserTests;

public class LoginInitEventParserUnitTests
{
    private readonly LoginInitEventParser _parser = new();

    [TestCaseSource(nameof(ValidJsonStrings))]
    public void Should_parse_when_correct_jsonString(string jsonString)
    {
        var eventData = _parser.Parse(jsonString);

        eventData.Should().NotBeNull();
        eventData.Should().BeOfType<LoginInitEventData>();
    }

    [TestCaseSource(nameof(InvalidJsonStrings))]
    public void Should_fail_when_incorrect_jsonString(string jsonString)
    {
        Action parsingAction = () => _parser.Parse(jsonString);

        parsingAction.Should().Throw<ArgumentException>();
    }

    private static IEnumerable<string> ValidJsonStrings()
    {
        yield return "{\"EventData\":{\"DeviceId\":\"" + Guid.NewGuid() + "\"}}";
    }

    private static IEnumerable<string> InvalidJsonStrings()
    {
        yield return "{\"EventData\":{}}";
        yield return "{\"EventData\":{\"ExtraField\":\"SomeValue\"}}";
        yield return "{\"EventData\":{\"deviceID\":\"" + Guid.NewGuid() + "\"}}"; // Incorrect casing
        yield return "{\"eventData\":{\"DeviceId\":null}}"; // Null DeviceId
        yield return "{\"eventData\":{\"DeviceId\":\"\", \"ExtraField\":\"SomeValue\"}}"; // Empty DeviceId
    }
}