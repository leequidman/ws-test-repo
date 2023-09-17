using Common.Models;
using Common.Models.GiftReceived;
using Common.Models.Login;
using Common.Models.SendGift;
using Common.Models.UpdateResources;
using FluentAssertions;
using FluentValidation;
using GameServer.Features.Login;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using NUnit.Framework;

namespace Tests
{
    public class UnitTests
    {
        private readonly LoginInitEventDataValidator _validator = new();

        [Test]
        public void Should_success_when_correct_login_data()
        {
            var eventData = new LoginInitEventData(Guid.NewGuid());
            Action validate = () => _validator.ValidateAndThrow(eventData);
            validate.Should().NotThrow();
        }

        [Test]
        public void Should_fail_when_empty_guid()
        {
            var eventData = new LoginInitEventData(Guid.Empty);
            Action validate = () => _validator.ValidateAndThrow(eventData);
            validate.Should().Throw<ValidationException>();
        }


        [TestCase("")]
        [TestCase(" ")]
        public void Should_fail_when_eventData_is_empty(object? eventData)
        {
            Action validate = () => _validator.ValidateAndThrow(eventData);
            validate.Should().Throw<ValidationException>();
        }

        [TestCaseSource(nameof(NotLoginDatas))]
        public void Should_fail_when_eventData_is_not_login(IEventData eventData)
        {
            Action validate = () => _validator.ValidateAndThrow(eventData);
            validate.Should().Throw<ValidationException>();
        }


        private static IEnumerable<IEventData> NotLoginDatas()
        {
            yield return new SendGiftInitEventData(Guid.NewGuid(), Guid.NewGuid(), ResourceType.Coins, 0);
            yield return new GiftReceivedEventData(Guid.NewGuid(), Guid.NewGuid(), ResourceType.Coins, 0, 0);
            yield return new UpdateResourceInitEventData(Guid.NewGuid(), ResourceType.Rolls, 0);
            yield return new UpdateResourceSuccessEventData(Guid.Empty, ResourceType.Rolls, 1);
        }
    }
}