using AutoFixture;
using DeviceDb.Api.Domain.Devices;

namespace DeviceDb.TestHelpers;

public class DeviceBuilder
{
    private static readonly Fixture _fixture = new();

    private Guid _deviceId = _fixture.Create<Guid>();
    private readonly string _name = _fixture.Create<string>();
    private string _brandId = _fixture.Create<string>();
    private readonly DateTime _createdOn = _fixture.Create<DateTime>();

    public DeviceBuilder WithId(Guid id)
    {
        _deviceId = id;
        return this;
    }

    public DeviceBuilder WithBrand(string brand)
    {
        _brandId = brand;
        return this;
    }

    public Device Build() => new(DeviceId.From(_deviceId), _name, BrandId.From(_brandId), _createdOn);
}
