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
    public async Task<Device?> GetDeviceAsync(DeviceId id) => _devices.FirstOrDefault(x => x.Id.Value == id.Value);
    public async Task SaveDeviceAsync(Device device)
    {
        var repoDevice = _devices.FirstOrDefault(o => o.Id.Value == device.Id.Value);

        if(repoDevice != default) {
            _devices.Remove(repoDevice);
        }

        _devices.Add(device);
    }
    public async Task DeleteDeviceAsync(DeviceId id)
    {
        var device = _devices.FirstOrDefault(o => o.Id.Value == id.Value);

        if (device == default)
            throw new InvalidOperationException("Device does not exist");

        _devices.Remove(device!);
    }
}
