using AutoFixture;
using DeviceDb.Api.Domain.Devices;
using DeviceDb.Api.Features.V1.Controllers;
using DeviceDb.Api.Features.V1.Models;
using DeviceDb.TestHelpers;
using NSubstitute;
using Xunit;

namespace DeviceDb.Api.Tests.Features.V1.Controllers;

public class DeviceControllerTests
{
    public class WhenPostingADevice
    {
        public class GivenANewValidDevice
        {
            private readonly Fixture _fixture = new();
            private readonly IDeviceRepository _repo;

            public GivenANewValidDevice()
            {
                _repo = Substitute.For<IDeviceRepository>();

                var controller = new DeviceController(_repo);
                var request = new AddDeviceRequest() {
                    Name = _fixture.Create<string>(),
                    Brand = _fixture.Create<string>()
                };

                _ = controller.AddDevice(request).Result;
            }

            [Fact]
            public void SaveDeviceIsCalled() => _repo.Received(1).SaveDeviceAsync(Arg.Any<Device>());
        }
    }

    public class WhenDeletingADevice
    {
        public class GivenAExistingDeviceId
        {
            private readonly Device _device;
            private readonly IDeviceRepository _repo;

            public GivenAExistingDeviceId()
            {
                _device = new DeviceBuilder().Build();

                _repo = Substitute.For<IDeviceRepository>();
                _repo.GetDeviceAsync(Arg.Is<DeviceId>(x => x.Value == _device.Id.Value)).Returns(_device);

                var controller = new DeviceController(_repo);
                _ = controller.DeleteDevice(_device.Id.Value).Result;
            }

            [Fact]
            public void ControllerChecksDeviceExists() => _repo.Received(1).GetDeviceAsync(Arg.Is<DeviceId>(x => x.Value == _device.Id.Value));

            [Fact]
            public void DeleteDeviceIsCalled() => _repo.Received(1).DeleteDeviceAsync(Arg.Is<DeviceId>(x => x.Value == _device.Id.Value));
        }

        public class GivenAInexistantDeviceId
        {
            private readonly Device _device;
            private readonly IDeviceRepository _repo;

            public GivenAInexistantDeviceId()
            {
                _device = new DeviceBuilder().Build();

                _repo = Substitute.For<IDeviceRepository>();
                _repo.GetDeviceAsync(Arg.Is<DeviceId>(x => x.Value == _device.Id.Value)).Returns(default(Device));

                var controller = new DeviceController(_repo);
                _ = controller.DeleteDevice(_device.Id.Value).Result;
            }

            [Fact]
            public void ControllerChecksDeviceExists() => _repo.Received(1).GetDeviceAsync(Arg.Is<DeviceId>(x => x.Value == _device.Id.Value));

            [Fact]
            public void DeleteDeviceIsNotCalled() => _repo.Received(0).DeleteDeviceAsync(Arg.Any<DeviceId>());
        }
    }
}