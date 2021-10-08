using System;
using Xunit;
using FluentAssertions;
using DeviceDb.Api.Domain.Devices;
using AutoFixture;

namespace DeviceDb.Api.Tests.Domain.Devices
{
    public class DeviceIdTests
    {
        static Fixture _fixture = new Fixture();

        public class WhenCreatingAnId
        {
            private readonly Guid _idGuid;
            private readonly DeviceId _generatedId;

            public WhenCreatingAnId()
            {
                _idGuid = _fixture.Create<Guid>();
                _generatedId = DeviceId.From(_idGuid);
            }

            [Fact]
            public void ValueObjectIsGenerated() => _generatedId.Should().NotBeNull();

            [Fact]
            public void CanAccessInternalGuidValue() => _generatedId.Value.Should().Be(_idGuid);
        }

        public class InvariantsAreProtected
        {
            //non null inputg protected by non-nullable Guid

            [Fact]
            public void ExceptionIsThrownIfDeviceIdGuidIsEmpty()
            {
                Action act = () => DeviceId.From(Guid.Empty);
                act.Should().Throw<ArgumentException>();
            }

            [Fact]
            public void ExceptionIsThrownIfDeviceIdGuidIsDefault()
            {
                Action act = () => DeviceId.From(default);
                act.Should().Throw<ArgumentException>();
            }
        }
    }
}
