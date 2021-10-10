using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceDb.Api.Adaptors;
using DeviceDb.Api.Domain.Devices;
using DeviceDb.Api.Features.V1.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Xunit;

namespace DeviceDb.Api.IntegrationTests.Features.V1;

public class DeviceController
{
    public class GetDevice
    {
        [Fact]
        public async Task GettingAnInexistentDeviceReturnsNotFound()
        {
            await using var app = new DeviceDbApplication();

            using var client = app.CreateClient();
            var id = Guid.NewGuid();

            using var _response = await client.GetAsync($"/api/v1/device/{id}");

           _response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GettingAnExistentDeviceReturnsNotFound()
        {
            var id = Guid.NewGuid();
            var device = Device.Create(DeviceId.From(id), "device name", BrandId.From("brand name"));

            await using var app = new DeviceDbApplication();
            await app.Repo.SaveDeviceAsync(device);

            using var client = app.CreateClient();

            
            using var _response = await client.GetAsync($"/api/v1/device/{id}");

            _response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var payload = JsonConvert.DeserializeObject<DeviceResponse>(await _response.Content.ReadAsStringAsync());

            payload.Should().BeEquivalentTo(new DeviceResponse {
                Id = id,
                Name = device.Name,
                Brand = device.BrandId.Value,
                CreatedOn = device.CreatedOn
            });
        }
    }

}

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
