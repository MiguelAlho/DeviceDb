﻿using DeviceDb.Api.Adaptors;
using DeviceDb.Api.Domain.Devices;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DeviceDb.Api.IntegrationTests.Features.V1;

internal class DeviceDbApplication : WebApplicationFactory<Program>
{
    private readonly InMemoryDeviceRepository _repo = new();

    public IDeviceRepository Repo { get => _repo; }

    override protected IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services => {
            services.AddSingleton<IDeviceRepository>(_repo);
        });

        return base.CreateHost(builder);
    }
}