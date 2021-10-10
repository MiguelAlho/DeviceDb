using System.ComponentModel.DataAnnotations;

namespace DeviceDb.Api.Features.V1.Models;

public record UpdateableDeviceRequest
{
    /// <summary>
    /// A user friendly device descriptor
    /// </summary>
    [MaxLength(100)]
    public string Name { get; init; }
    
    /// <summary>
    /// Brand identifying string for the device
    /// </summary>
    [MaxLength(100)]
    public string Brand { get; init; }
}
