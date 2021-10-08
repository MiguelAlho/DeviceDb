namespace DeviceDb.Api.Features.V1.Models
{
    public record AddDeviceRequest
    {
        /// <summary>
        /// A user friendly device descriptor
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// Brand identifying string for the device
        /// </summary>
        public string Brand { get; init; }
    }


}
