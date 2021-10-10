using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using DeviceDb.Api.Domain.Devices;
using DeviceDb.Api.Features.V1.Models;
using DeviceDb.TestHelpers;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DeviceDb.Api.IntegrationTests.Features.V1;

public class DeviceControllerTests
{
    static Fixture _fixture = new();

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
        public async Task GettingAnExistentDeviceReturnsOKWithCorrectPayload()
        {
            var id = Guid.NewGuid();
            var device = new DeviceBuilder().WithId(id).Build();

            await using var app = new DeviceDbApplication();
            await app.Repo.SaveDeviceAsync(device);

            using var client = app.CreateClient();
            using var _response = await client.GetAsync($"/api/v1/device/{id}");

            _response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var payload = JsonConvert.DeserializeObject<DeviceResponse>(await _response.Content.ReadAsStringAsync());

            payload.Should().BeEquivalentTo(DeviceToDeviceResponse(device));
        }        
    }

    public class GetDeviceList
    {
        [Fact]
        public async Task GettingDeviceListWithNoDevicesReturnsOKWithEmptyArrayPayload()
        {
            await using var app = new DeviceDbApplication();
            //no repo setup, so empty device db

            using var client = app.CreateClient();
            using var _response = await client.GetAsync($"/api/v1/device/");

            _response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var payload = JsonConvert.DeserializeObject<DeviceResponse[]>(await _response.Content.ReadAsStringAsync());

            payload.Should().BeEquivalentTo(new DeviceResponse[] { });
        }

        [Fact]
        public async Task GettingDeviceListWithDevicesReturnsOKWithValidPayload()
        {
            var device1 = new DeviceBuilder().Build();
            var device2 = new DeviceBuilder().Build();

            await using var app = new DeviceDbApplication();
            await app.Repo.SaveDeviceAsync(device1);
            await app.Repo.SaveDeviceAsync(device2);

            using var client = app.CreateClient();
            using var _response = await client.GetAsync($"/api/v1/device/");

            _response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var payload = JsonConvert.DeserializeObject<DeviceResponse[]>(await _response.Content.ReadAsStringAsync());

            payload.Should().BeEquivalentTo(new DeviceResponse[] { 
                DeviceToDeviceResponse(device1),
                DeviceToDeviceResponse(device2)
            });
        }    
    }

    public class PostDevice
    {
        [Fact]
        public async Task PostingInexistentDeviceReturnsCreated()
        {
            var request = new AddDeviceRequest {
                Brand = _fixture.Create<string>(),
                Name = _fixture.Create<string>()
            };

            await using var app = new DeviceDbApplication();
            //no repo setup, so empty device db

            using var client = app.CreateClient();
            using var _response = await client.PostAsync($"/api/v1/device/", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            _response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

            //check location header is correctly set
            var createdDevice = (await app.Repo.GetAllDevicesAsync().ToListAsync()).Single();
            _response.Headers.Location?.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped).Should().Be($"api/v1/Device/{createdDevice.Id.Value}");            
        }

        [Theory]
        [MemberData(nameof(InvalidInputs))]
        public async Task MissingInputObjectInvalidatesRequest(AddDeviceRequest request)
        {
            await using var app = new DeviceDbApplication();
            
            using var client = app.CreateClient();
            using var _response = await client.PostAsync($"/api/v1/device/", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            _response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        public static IEnumerable<object[]> InvalidInputs => new List<object[]>
        {
            new object[] { new AddDeviceRequest() { Name = string.Empty, Brand = _fixture.Create<string>() } },
            new object[] { new AddDeviceRequest() { Name = _fixture.Create<string>(), Brand = string.Empty } },
            new object[] { new AddDeviceRequest() { Name = StringHelpers.GetLongString(100), Brand = _fixture.Create<string>() } },
            new object[] { new AddDeviceRequest() { Name = _fixture.Create<string>(), Brand = StringHelpers.GetLongString(100) } },
        };
    }

    protected static DeviceResponse DeviceToDeviceResponse(Domain.Devices.Device device) => new DeviceResponse {
        Id = device.Id.Value,
        Name = device.Name,
        Brand = device.BrandId.Value,
        CreatedOn = device.CreatedOn
    };

}
