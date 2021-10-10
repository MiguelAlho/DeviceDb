using DeviceDb.Api.Domain.Devices;

namespace DeviceDb.Api.Adaptors;

public class InMemoryDeviceRepository : IDeviceRepository
{
    private readonly List<Device> _devices = new();

    public async IAsyncEnumerable<Device> GetAllDevicesAsync()
    {
        foreach (var d in _devices)
            yield return d;
    }
    public async Task<Device?> GetDeviceAsync(Guid id) => _devices.FirstOrDefault(x => x.Id.Value == id);
    public async Task SaveDeviceAsync(Device device) => _devices.Add(device);
}
