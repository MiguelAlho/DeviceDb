using System;
using Xunit;
using FluentAssertions;
using DeviceDb.Api.Domain.Devices;
using AutoFixture;
using FluentAssertions.Extensions;

namespace DeviceDb.Api.Tests.Domain.Devices
{
    public class DeviceTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public void CreateCreatesInstancesWithValidDate()
        {
            var id = DeviceId.Create();
            var name = _fixture.Create<string>();
            var brand = BrandId.From(_fixture.Create<string>());

            var device = Device.Create(id, name, brand);

            device.Id.Should().Be(id);
            device.Name.Should().Be(name);
            device.BrandId.Should().Be(brand);
            device.CreatedOn.Should().BeCloseTo(DateTime.Now, 1.Seconds());
        }
    }
}
