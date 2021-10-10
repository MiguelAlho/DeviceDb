using AutoFixture;
using DeviceDb.Api.Domain.Devices;

namespace DeviceDb.TestHelpers;

public class DeviceBuilder
{
    private readonly static Fixture _fixture = new();

    private Guid _deviceId = _fixture.Create<Guid>();
    private readonly string _name = _fixture.Create<string>();
    private readonly string _bandId = _fixture.Create<string>();
    private readonly DateTime _createdOn = _fixture.Create<DateTime>();

    public DeviceBuilder WithId(Guid id)
    {
        _deviceId = id;
        return this;
    }

    public Device Build() => new Device(DeviceId.From(_deviceId), _name, BrandId.From(_bandId), _createdOn);

}
