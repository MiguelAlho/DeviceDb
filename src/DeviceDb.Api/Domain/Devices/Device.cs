namespace DeviceDb.Api.Domain.Devices;

public class Device
{
    public DeviceId Id { get; }
    public string Name { get; private set; }
    public BrandId BrandId { get; private set; }
    public DateTime CreatedOn { get; }

    internal Device(DeviceId id, string name, BrandId brandId, DateTime createdOn)
    {
        Id = id;
        Name = name;
        BrandId = brandId;
        CreatedOn = createdOn;
    }

    internal static Device Create(DeviceId id, string name, BrandId brandId)
        => new(id, name, brandId, DateTime.Now);
    internal void Update(UpdateDevice changes) {
        Name = changes.Name;
        BrandId = BrandId.From(changes.Brand);
    }
}

internal record UpdateDevice
{
    public string Name { get; init; }
    public string Brand { get; init; }
}

public interface IDeviceRepository
{
    //public Task<IReadOnlyCollection<Device>> GetListOfDevicesByBrandAsync(BrandId id);

    public Task<Device?> GetDeviceAsync(DeviceId guid);
    public IAsyncEnumerable<Device> GetAllDevicesAsync();
    public IAsyncEnumerable<Device> GetAllDevicesByBrandAsync(BrandId brandId);

    public Task SaveDeviceAsync(Device device);
    public Task DeleteDeviceAsync(DeviceId id);    
}
