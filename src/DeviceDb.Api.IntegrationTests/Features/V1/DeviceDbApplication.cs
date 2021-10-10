using DeviceDb.Api.Adaptors;
using DeviceDb.Api.Domain.Devices;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DeviceDb.Api.IntegrationTests.Features.V1;

class DeviceDbApplication : WebApplicationFactory<Program>
{
    InMemoryDeviceRepository _repo = new InMemoryDeviceRepository();

    public IDeviceRepository Repo { get => _repo; }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services => {
            services.AddSingleton<IDeviceRepository>(_repo);
        });

        return base.CreateHost(builder);
    }
}
