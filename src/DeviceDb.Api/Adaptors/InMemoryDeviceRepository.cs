using DeviceDb.Api.Domain.Devices;

namespace DeviceDb.Api.Adaptors
{
    public class InMemoryDeviceRepository : IDeviceRepository
    {
        private readonly List<Device> _devices = new();

        public async Task<Device?> GetDeviceAsync(Guid id) => _devices.FirstOrDefault(x => x.Id.Value == id);
        public async Task SaveDeviceAsync(Device device) => _devices.Add(device);
    }
}
