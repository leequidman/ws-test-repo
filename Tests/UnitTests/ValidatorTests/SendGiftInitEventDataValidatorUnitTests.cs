using Common.Models;
using Common.Models.SendGift;
using FluentAssertions;
using FluentValidation;
using GameServer.Features.SendGift;
using NUnit.Framework;

namespace Tests.UnitTests.ValidatorTests;

public class SendGiftInitEventDataValidatorUnitTests
{
    private readonly SendGiftInitEventDataValidator _validator = new();

    [TestCaseSource(nameof(CorrectEventDataCases))]
    public void Should_success_when_correct_eventData(SendGiftInitEventData eventData)
    {
        Action validate = () => _validator.ValidateAndThrow(eventData);
        validate.Should().NotThrow();
    }

    [TestCaseSource(nameof(InCorrectEventDataCases))]
    public void Should_fail_when_incorrect_eventData(SendGiftInitEventData eventData)
    {
        Action validate = () => _validator.ValidateAndThrow(eventData);
        validate.Should().Throw<ValidationException>();
    }

    private static IEnumerable<SendGiftInitEventData> CorrectEventDataCases()
    {
        yield return new(Guid.NewGuid(), Guid.NewGuid(), ResourceType.Coins, 10);
        yield return new(Guid.NewGuid(), Guid.NewGuid(), ResourceType.Rolls, 5);
        yield return new(Guid.NewGuid(), Guid.NewGuid(), ResourceType.Coins, int.MaxValue);
        yield return new(Guid.NewGuid(), Guid.NewGuid(), ResourceType.Rolls, int.MaxValue - 1);
    }

    private static IEnumerable<SendGiftInitEventData> InCorrectEventDataCases()
    {
        yield return new(Guid.Empty, Guid.NewGuid(), ResourceType.Coins, 10);
        yield return new(Guid.NewGuid(), Guid.Empty, ResourceType.Coins, 10);
        yield return new(Guid.NewGuid(), Guid.NewGuid(), (ResourceType)4, 10);
        yield return new(Guid.NewGuid(), Guid.NewGuid(), ResourceType.Coins, 0);
        yield return new(Guid.NewGuid(), Guid.NewGuid(), (ResourceType)13, 11);
        yield return new(Guid.NewGuid(), Guid.NewGuid(), ResourceType.Coins, -1);
    }
}