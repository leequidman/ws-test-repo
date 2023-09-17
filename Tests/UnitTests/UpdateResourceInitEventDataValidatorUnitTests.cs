using Common.Models;
using Common.Models.UpdateResources;
using FluentAssertions;
using FluentValidation;
using GameServer.Features.UpdateResource;
using NUnit.Framework;

namespace Tests.UnitTests;

public class UpdateResourceInitEventDataValidatorUnitTests
{
    private readonly UpdateResourceInitEventDataValidator _validator = new();

    [TestCaseSource(nameof(CorrectEventDataCases))]
    public void Should_success_when_correct_eventData(UpdateResourceInitEventData eventData)
    {
        Action validate = () => _validator.ValidateAndThrow(eventData);
        validate.Should().NotThrow();
    }

    [TestCaseSource(nameof(InCorrectEventDataCases))]
    public void Should_fail_when_incorrect_eventData(UpdateResourceInitEventData eventData)
    {
        Action validate = () => _validator.ValidateAndThrow(eventData);
        validate.Should().Throw<ValidationException>();
    }

    private static IEnumerable<UpdateResourceInitEventData> CorrectEventDataCases()
    {
        yield return new(Guid.NewGuid(), ResourceType.Coins, 10);
        yield return new(Guid.NewGuid(), ResourceType.Rolls, 5);
        yield return new(Guid.NewGuid(), ResourceType.Rolls, int.MaxValue);
        yield return new(Guid.NewGuid(), ResourceType.Rolls, int.MaxValue-1);
        yield return new(Guid.NewGuid(), ResourceType.Coins, 1);
    }
    private static IEnumerable<UpdateResourceInitEventData> InCorrectEventDataCases()
    {
        yield return new(Guid.Empty, ResourceType.Coins, 10);
        yield return new(Guid.NewGuid(), (ResourceType)4, 5);
        yield return new(Guid.NewGuid(), ResourceType.Rolls, 0);
        yield return new(Guid.NewGuid(), ResourceType.Rolls, -1);
        yield return new(Guid.NewGuid(), null, -1);
    }
}