namespace DeviceDb.Api.Domain.Devices;

/// <summary>
/// Identifier for an individual device
/// 
/// Current schema is a based on a simple guid, but could be based on any other
/// RFC for IoT device identification if the domain warrants it.
/// </summary>
public record DeviceId
{
    internal Guid Value { get; }

    public static DeviceId Create() => new(Guid.NewGuid());

    private DeviceId(Guid value) => Value = value;

    internal static DeviceId From(Guid deviceId)
    {
        if (deviceId == default)
            throw new ArgumentException("invalid device id format", nameof(deviceId));

        return new DeviceId(deviceId);
    }
}
