using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using DeviceDb.Api.Domain.Devices;

namespace DeviceDb.Api.Tests.Domain.Devices
{
    public class DeviceTests
    {
        [Fact]
        public void CanCreateInstanceOfDevice()
        {
            var device = new Device();

            device.Should().NotBeNull();
        }
    }
}
