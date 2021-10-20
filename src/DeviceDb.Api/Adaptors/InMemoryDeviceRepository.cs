using System.Data;
using Dapper;
using DeviceDb.Api.Domain.Devices;
using Microsoft.Data.Sqlite;

namespace DeviceDb.Api.Adaptors;

public class SqliteDeviceRepository : IDeviceRepository
{
    private readonly string _connectionString;

    public SqliteDeviceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public class DeviceRecord
    {
        public string Id {  get; set; }
        public string Name {  get; set; }   
        public string BrandId {  get; set; }    
        public DateTime CreatedOn {  get; set; }
    }

    public async Task DeleteDeviceAsync(DeviceId id)
    {
        using var connection = await CreateOpenConnection();
        var devices = await connection.ExecuteAsync("DELETE FROM Device WHERE Id = @Id", new { Id = id.Value });
    }

    public async IAsyncEnumerable<Device> GetAllDevicesAsync()
    {
        using var connection = await CreateOpenConnection();
        var devices = await connection.QueryAsync<DeviceRecord>("SELECT * FROM Device");

        foreach (var device in devices) {
            yield return new Device(
                DeviceId.From(Guid.Parse(device!.Id)),
                device.Name,
                BrandId.From(device.BrandId),
                device.CreatedOn
            );
        }
    }

    public async IAsyncEnumerable<Device> GetAllDevicesByBrandAsync(BrandId brandId, PageInfo pageInfo)
    {
        using var connection = await CreateOpenConnection();
        var devices = await connection.QueryAsync<DeviceRecord>(
            "SELECT * FROM Device WHERE BrandId=@BrandId ORDER BY CreatedOn DESC LIMIT @Size OFFSET @Offset", 
            new { BrandId = brandId.Value, Size=pageInfo.Size, Offset=pageInfo.Offset });

        foreach (var device in devices) {
            yield return new Device(
                DeviceId.From(Guid.Parse(device.Id)),
                device.Name,
                BrandId.From(device.BrandId),
                device.CreatedOn
            );
        }
    }

    public async Task<Device?> GetDeviceAsync(DeviceId id) {
        using var connection = await CreateOpenConnection();

        var devices = await connection.QueryAsync<DeviceRecord>("SELECT * FROM Device WHERE Id=@Id", new { Id = id.Value });
        var device = devices.FirstOrDefault();

        if (device == default)
            return default;

        return new Device(
            DeviceId.From(Guid.Parse(device!.Id)),
            device.Name,
            BrandId.From(device.BrandId),
            device.CreatedOn
        );
    }

    public async Task SaveDeviceAsync(Device device)
    {
        var upsert = @"
INSERT INTO Device(Id, Name, BrandId, CreatedOn)
    VALUES(@Id, @Name, @BrandId, @CreatedOn)
    ON CONFLICT(Id) DO UPDATE SET
        Name = excluded.Name,
        BrandId = excluded.BrandId";

        using (var connection = await CreateOpenConnection())
        await connection.ExecuteAsync(upsert, new {
            Id = device.Id.Value,
            Name = device.Name,
            BrandId = device.BrandId.Value,
            CreatedOn = device.CreatedOn
        });
    }

    private async Task<IDbConnection> CreateOpenConnection()
    {
        var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }
}
