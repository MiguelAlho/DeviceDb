using AutoFixture;
using DeviceDb.Api.Domain.Devices;
using DeviceDb.Api.Features.V1.Controllers;
using DeviceDb.Api.Features.V1.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Net;
using Xunit;

namespace DeviceDb.Api.Tests.Features.V1.Controllers
{
    public class DeviceControllerTests
    {
        public class WhenPostingADevice
        {
            public class GivenANewValidDevice
            {
                private readonly Fixture _fixture = new();
                private readonly CreatedResult _createdResult;
                private readonly IDeviceRepository _repo;

                public GivenANewValidDevice()
                {
                    _repo = Substitute.For<IDeviceRepository>();

                    var controller = new DeviceController(_repo);
                    var request = new AddDeviceRequest() {
                        Name = _fixture.Create<string>(),
                        Brand = _fixture.Create<string>()
                    };

                    _createdResult = (CreatedResult)controller.AddDevice(request).Result;
                }

                [Fact]
                public void StatusShouldBeCreated() => _createdResult.StatusCode.Should().Be((int)HttpStatusCode.Created);

                [Fact]
                public void ResultShouldHaveResouceLocation() => _createdResult.Location.Should().NotBeEmpty();

                [Fact]
                public void SaveDeviceIsCalled() => _repo.Received(1).SaveDeviceAsync(Arg.Any<Device>());
            }
        }
    }


}
