using DeviceDb.Api.Domain.Devices;
using DeviceDb.Api.Features.V1.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeviceDb.Api.Features.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceRepository _repo;

        public DeviceController(IDeviceRepository repo) => _repo = repo;

        /// <summary>
        /// Adds a device to the device database, generating a resource link for it
        /// </summary>
        /// <param name="request">The device data</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(CreatedResult), 201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddDevice([FromBody] AddDeviceRequest request)
        {
            var device = Device.Create(DeviceId.Create(), request.Name, BrandId.From(request.Brand));
            await _repo.SaveDeviceAsync(device);

            return new CreatedResult($"replacewithurl/device/{device.Id.Value}", null);
        }
    }


}
