namespace DeviceDb.Api.Domain.Devices;

/// <summary>
/// Identifier for an individual brand
/// </summary>
public record BrandId
{
    public string Value { get; }

    private BrandId(string value) => Value = value;

    internal static BrandId From(string brandId)
    {
        if (string.IsNullOrWhiteSpace(brandId) || brandId.Length > 100)
            throw new ArgumentException("invalid brand id format", nameof(brandId));

        return new BrandId(brandId);
    }

    public override string ToString() => Value;
}
