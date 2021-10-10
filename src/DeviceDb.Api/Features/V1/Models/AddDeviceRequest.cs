using System.ComponentModel.DataAnnotations;

namespace DeviceDb.Api.Features.V1.Models;

public record AddDeviceRequest
{
    /// <summary>
    /// A user friendly device descriptor
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; init; }
    /// <summary>
    /// Brand identifying string for the device
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Brand { get; init; }
}
