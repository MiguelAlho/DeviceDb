using AutoFixture;
using DeviceDb.Api.Domain.Devices;
using DeviceDb.Api.Features.V1.Controllers;
using DeviceDb.Api.Features.V1.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;
using System.Linq;

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


        public class WhenGettingADevice
        {
            public class GivenAnExistingId
            {
                private readonly Fixture _fixture = new();
                private readonly IDeviceRepository _repo;
                private readonly Guid _id;
                private readonly Device _device;
                private readonly OkObjectResult _response;
                private readonly DeviceResponse? _result;

                public GivenAnExistingId()
                {
                    _id = _fixture.Create<Guid>();
                    _device = Device.Create(DeviceId.From(_id), _fixture.Create<string>(), BrandId.From(_fixture.Create<string>()));
                    _repo = Substitute.For<IDeviceRepository>();
                    _repo.GetDeviceAsync(Arg.Is<Guid>(x => x == _id)).Returns(_device);

                    var controller = new DeviceController(_repo);

                    _response = (OkObjectResult)controller.GetDevice(_id).Result;
                    _result = _response.Value as DeviceResponse;
                }

                [Fact]
                public void StatusShouldBeOk() => _response.StatusCode.Should().Be((int)HttpStatusCode.OK);

                [Fact]
                public void ResponseHasDeviceData() => _result.Should().BeEquivalentTo(new DeviceResponse {
                    Id = _id,
                    Name = _device.Name,
                    Brand = _device.BrandId.Value,
                    CreatedOn = _device.CreatedOn
                });
            }

            public class GivenANonExistantId
            {
                private readonly Fixture _fixture = new();
                private readonly IDeviceRepository _repo;
                private readonly Guid _id;
                private readonly Device _device;
                private readonly NotFoundResult _response;

                public GivenANonExistantId()
                {
                    _id = _fixture.Create<Guid>();
                    _device = Device.Create(DeviceId.From(_id), _fixture.Create<string>(), BrandId.From(_fixture.Create<string>()));
                    _repo = Substitute.For<IDeviceRepository>();
                    _repo.GetDeviceAsync(Arg.Any<Guid>()).Returns(default(Device));

                    var controller = new DeviceController(_repo);

                    _response = (NotFoundResult)controller.GetDevice(_id).Result;
                }

                [Fact]
                public void StatusShouldBeCreated() => _response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            }
        }

        public class WhenGettingAListOfDevices
        {

            private readonly Fixture _fixture = new();
            private readonly IDeviceRepository _repo;
            private readonly Device _device1;
            private readonly Device _device2;
            private readonly List<DeviceResponse> _result;

            public WhenGettingAListOfDevices()
            {
                _device1 = Device.Create(DeviceId.From(_fixture.Create<Guid>()), _fixture.Create<string>(), BrandId.From(_fixture.Create<string>()));
                _device2 = Device.Create(DeviceId.From(_fixture.Create<Guid>()), _fixture.Create<string>(), BrandId.From(_fixture.Create<string>()));
                
                _repo = Substitute.For<IDeviceRepository>(); 
                _repo.GetAllDevicesAsync().Returns(new[] { _device1, _device2 }.ToAsyncEnumerable());

                var controller = new DeviceController(_repo);

                _result = controller.GetListOfDevices().ToListAsync().Result;
            }

            [Fact]
            public void ResponseHasDeviceData() => _result.Should().BeEquivalentTo(new DeviceResponse[] {
                new DeviceResponse{
                    Id = _device1.Id.Value,
                    Name = _device1.Name,
                    Brand = _device1.BrandId.Value,
                    CreatedOn = _device1.CreatedOn
                },
                new DeviceResponse{
                    Id = _device2.Id.Value,
                    Name = _device2.Name,
                    Brand = _device2.BrandId.Value,
                    CreatedOn = _device2.CreatedOn
                }
            });

        }
    }
}
