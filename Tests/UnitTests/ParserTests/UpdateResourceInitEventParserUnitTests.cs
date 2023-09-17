using Common.Models.UpdateResources;
using FluentAssertions;
using GameServer.Features.UpdateResource;
using NUnit.Framework;

namespace Tests.UnitTests.ParserTests;

public class UpdateResourceInitEventParserUnitTests
{
    private readonly UpdateResourceInitEventParser _parser = new();

    [TestCaseSource(nameof(ValidJsonStrings))]
    public void Should_parse_when_correct_jsonString(string jsonString)
    {
        var eventData = _parser.Parse(jsonString);
        eventData.Should().NotBeNull().And.BeOfType<UpdateResourceInitEventData>();
    }

    [TestCaseSource(nameof(InvalidJsonStrings))]
    public void Should_fail_when_incorrect_jsonString(string jsonString)
    {
        Action parsingAction = () => _parser.Parse(jsonString);
        parsingAction.Should().Throw<ArgumentException>();
    }

    private static IEnumerable<string> ValidJsonStrings()
    {
        yield return $"{{\"EventData\":{{\"PlayerId\":\"{Guid.NewGuid()}\", \"ResourceType\":\"Coins\", \"Amount\":10}}}}";
        yield return $"{{\"EventData\":{{\"PlayerId\":\"{Guid.NewGuid()}\", \"ResourceType\":\"Rolls\", \"Amount\":5}}}}";
    }

    private static IEnumerable<string> InvalidJsonStrings()
    {
        yield return "{\"EventData\":{}}";
        yield return $"{{\"EventData\":{{\"PlayerId\":\"{Guid.NewGuid()}\"}}}}";
        yield return "{\"EventData\":{\"PlayerId\":\"\", \"ResourceType\":\"Coins\", \"Amount\":10}}";
        yield return $"{{\"EventData\":{{\"PlayerId\":\"{Guid.NewGuid()}\", \"ResourceType\":\"InvalidType\", \"Amount\":10}}}}";
        yield return $"{{\"EventData\":{{\"PlayerId\":\"{Guid.NewGuid()}\", \"ResourceType\":\"Coins\", \"Amount\":\"notAnInt\"}}}}";
        yield return $"{{\"Event\":{{\"PlayerId\":\"{Guid.NewGuid()}\", \"ResourceType\":\"Coins\", \"Amount\":10}}}}"; // Incorrect root field
        yield return "{\"EventData\":{\"PlayerId\":null, \"ResourceType\":\"Coins\", \"Amount\":10}}"; // PlayerId is null
        yield return $"{{\"EventData\":{{\"PlayerId\":\"{Guid.NewGuid()}\", \"ResourceType\":\"Coins\"}}}}"; // Missing Amount
        yield return $"{{\"EventData\":{{\"PlayerId\":\"{Guid.NewGuid()}\", \"ResourceType\":\"Coins\", \"Amount\":\"\"}}}}"; // Empty Amount
    }
}