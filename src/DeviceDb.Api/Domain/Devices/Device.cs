namespace DeviceDb.Api.Domain.Devices
{
    public class Device
    {
        public DeviceId Id { get; }
        public string Name { get; }
        public BrandId BrandId { get; }
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
    }

    public interface IDeviceRepository
    {
        //public Task<Device> LoadDeviceAsync(DeviceId id);

        //public Task<IReadOnlyCollection<Device>> GetListOfDevicesAsync();

        //public Task<IReadOnlyCollection<Device>> GetListOfDevicesByBrandAsync(BrandId id);

        public Task SaveDeviceAsync(Device device);

        //public Task DeleteDeviceAsync(DeviceId id);

    }
}
