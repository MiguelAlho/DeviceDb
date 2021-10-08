using AutoFixture;
using DeviceDb.Api.Domain.Devices;
using DeviceDb.Api.Features.V1.Controllers;
using DeviceDb.Api.Features.V1.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Net;
using Xunit;

namespace DeviceDb.Api.Tests.Controllers
{
    public class DeviceControllerTests
    {
        public class PostDevice
        {
            public class WhenPostingANewValidDevice
            {
                Fixture _fixture = new Fixture();
                private CreatedResult createdResult;
                private IDeviceRepository _repo;

                public WhenPostingANewValidDevice()
                {
                    _repo = Substitute.For<IDeviceRepository>();

                    var controller = new DeviceController(_repo);
                    var request = new AddDeviceRequest()
                    {
                        Name = _fixture.Create<string>(),
                        Brand = _fixture.Create<string>()
                    };

                    createdResult = (CreatedResult)controller.AddDevice(request).Result;
                }

                [Fact]
                public void StatusShouldBeCreated() => createdResult.StatusCode.Should().Be((int)HttpStatusCode.Created);

                [Fact]
                public void ResultShouldHaveResouceLocation() => createdResult.Location.Should().NotBeEmpty();

                [Fact]
                public void SaveDeviceIsCalled() => _repo.Received(1).SaveDeviceAsync(Arg.Any<Device>());
            }
        }
    }

    
}
