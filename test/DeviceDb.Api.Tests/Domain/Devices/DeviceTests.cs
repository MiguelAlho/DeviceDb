using System;
using Xunit;
using FluentAssertions;
using DeviceDb.Api.Domain.Devices;
using AutoFixture;
using FluentAssertions.Extensions;
using DeviceDb.TestHelpers;

namespace DeviceDb.Api.Tests.Domain.Devices;

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

    [Fact]
    public void CanChangeMutableValuesInDevice()
    {
        var device = new DeviceBuilder().Build();
        var id = device.Id.Value;
        var createdOn = device.CreatedOn;

        var changes = new UpdateDevice { Name = "new name", Brand = "new brand" };

        device.Update(changes);

        //untouched:
        device.Id.Value.Should().Be(id);
        device.CreatedOn.Should().Be(createdOn);

        //mutated
        device.Name.Should().Be(changes.Name);
        device.BrandId.Value.Should().Be(changes.Brand);
    }
}
