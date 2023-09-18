using Common.Models.SendGift;
using FluentAssertions;
using GameServer.Features.SendGift;
using NUnit.Framework;

namespace Tests.UnitTests.ParserTests;

public class SendGiftInitEventParserUnitTests
{
    private readonly SendGiftInitInitEventParser _parser = new();

    [TestCaseSource(nameof(ValidJsonStrings))]
    public void Should_parse_when_correct_jsonString(string jsonString)
    {
        var eventData = _parser.Parse(jsonString);
        eventData.Should().NotBeNull().And.BeOfType<SendGiftInitEventData>();
    }

    [TestCaseSource(nameof(InvalidJsonStrings))]
    public void Should_fail_when_incorrect_jsonString(string jsonString)
    {
        Action parsingAction = () => _parser.Parse(jsonString);
        parsingAction.Should().Throw<ArgumentException>();
    }

    private static IEnumerable<string> ValidJsonStrings()
    {
        yield return
            $"{{\"EventData\":{{\"SenderId\":\"{Guid.NewGuid()}\", \"ReceiverId\":\"{Guid.NewGuid()}\", \"Resource\":\"Coins\", \"Amount\":10}}}}";
        yield return
            $"{{\"EventData\":{{\"SenderId\":\"{Guid.NewGuid()}\", \"ReceiverId\":\"{Guid.NewGuid()}\", \"Resource\":\"Rolls\", \"Amount\":5}}}}";
        yield return
            $"{{\"EventData\":{{\"SenderId\":\"{Guid.NewGuid()}\", \"ReceiverId\":\"{Guid.NewGuid()}\", \"Resource\":\"Rolls\", \"Amount\":0}}}}";
    }

    private static IEnumerable<string> InvalidJsonStrings()
    {
        yield return "{\"EventData\":{}}";
        yield return $"{{\"EventData\":{{\"SenderId\":\"{Guid.NewGuid()}\"}}}}";
        yield return $"{{\"EventData\":{{\"SenderId\":\"\", \"ReceiverId\":\"{Guid.NewGuid()}\", \"Resource\":\"Coins\", \"Amount\":10}}}}";
        yield return $"{{\"EventData\":{{\"SenderId\":\"{Guid.NewGuid()}\", \"ReceiverId\":\"\", \"Resource\":\"Coins\", \"Amount\":10}}}}";
        yield return
            $"{{\"EventData\":{{\"SenderId\":\"{Guid.NewGuid()}\", \"ReceiverId\":\"{Guid.NewGuid()}\", \"Resource\":\"InvalidType\", \"Amount\":10}}}}";
        yield return
            $"{{\"EventData\":{{\"SenderId\":\"{Guid.NewGuid()}\", \"ReceiverId\":\"{Guid.NewGuid()}\", \"Resource\":\"Coins\", \"Amount\":\"notAnInt\"}}}}";
        yield return
            $"{{\"Event\":{{\"SenderId\":\"{Guid.NewGuid()}\", \"ReceiverId\":\"{Guid.NewGuid()}\", \"Resource\":\"Coins\", \"Amount\":10}}}}"; // Incorrect root field
        yield return
            $"{{\"EventData\":{{\"SenderId\":null, \"ReceiverId\":\"{Guid.NewGuid()}\", \"Resource\":\"Coins\", \"Amount\":10}}}}"; // SenderId is null
        yield return $"{{\"EventData\":{{\"SenderId\":\"{Guid.NewGuid()}\", \"ReceiverId\":\"\", \"Resource\":\"Coins\"}}}}"; // Missing Amount
        yield return
            $"{{\"EventData\":{{\"SenderId\":\"{Guid.NewGuid()}\", \"ReceiverId\":\"{Guid.NewGuid()}\", \"Resource\":\"Coins\", \"Amount\":\"\"}}}}"; // Empty Amount
    }
}