using AutoFixture;
using DeviceDb.Api.Domain.Devices;
using DeviceDb.Api.Features.V1.Controllers;
using DeviceDb.Api.Features.V1.Models;
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

}
