using System;
using Xunit;
using FluentAssertions;
using DeviceDb.Api.Domain.Devices;
using AutoFixture;

namespace DeviceDb.Api.Tests.Domain.Devices
{
    public class DeviceTests
    {
        readonly Fixture _fixture = new();

        [Fact]
        public void CanCreateInstanceOfDevice()
        {
            var device = new Device(DeviceId.Create(), string.Empty, BrandId.From(_fixture.Create<string>()), DateTime.Now);

            device.Should().NotBeNull();
        }

        [Fact]
        public void CreateCreatesInstancesWithValidDate()
        {
            var id = DeviceId.Create();
            var name = _fixture.Create<string>();
            var brand = BrandId.From(_fixture.Create<string>());

            //var device = new Device.Create(id, name, brand);

            //device.Should().NotBeNull();
            //device.Id.Should().Be(id);
            //device.Name.Should().Be(name);
            //device.Brand.Should().Be(brand);
            //device.CreatedOn.Should().BeCloseTo(DateTime.Now, 10.Seconds);
        }
    }
}
