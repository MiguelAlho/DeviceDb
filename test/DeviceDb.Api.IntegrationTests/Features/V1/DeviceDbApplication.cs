using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DeviceDb.Api.Adaptors;
using DeviceDb.Api.Adaptors.Sql.Migrations;
using DeviceDb.Api.Domain.Devices;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DeviceDb.Api.IntegrationTests.Features.V1;

internal class DeviceDbApplication : WebApplicationFactory<Program>
{
    private string conString = $"Data Source={Guid.NewGuid().ToString("N")};Mode=Memory;Cache=Shared";
    
    //ensure open connection so inmemory db will stay throughout test lifetime
    private IDbConnection masterConnection;
    private SqlDeviceRepository _repo;

    public IDeviceRepository Repo { get => _repo; }

    public DeviceDbApplication()
    {
        masterConnection = OpenPersistantConnection(conString);
        _repo = new(conString);
    }

    override protected IHost CreateHost(IHostBuilder builder)
    {
        builder
            .ConfigureHostConfiguration(configBuilder => {
                configBuilder.AddInMemoryCollection(
                    new Dictionary<string, string> { 
                        ["Database"] = conString
                    });                
            })
            .ConfigureServices(services => {
                services.AddSingleton<IDeviceRepository>(_repo);
            });

        return base.CreateHost(builder);
    }

    private static IDbConnection OpenPersistantConnection(string connString)
    {
        SqliteConnection conn = new SqliteConnection(connString);
        conn.Open();
        return conn;
    }

    public override ValueTask DisposeAsync()
    {
        if(masterConnection != default)
            masterConnection.Dispose();
        return base.DisposeAsync();
    }

    public class MigratingSqlDeviceRepository : SqlDeviceRepository
    {
        public MigratingSqlDeviceRepository(string conString) : base(conString)
        {

        }
    }
}
