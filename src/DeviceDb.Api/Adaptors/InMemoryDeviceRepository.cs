using DeviceDb.Api.Domain.Devices;

namespace DeviceDb.Api.Adaptors
{
    public class InMemoryDeviceRepository : IDeviceRepository
    {
        List<Device> devices = new List<Device>();

        public async Task SaveDeviceAsync(Device device) => devices.Add(device);
    }
}
