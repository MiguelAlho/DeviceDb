using DeviceDb.Api.Domain.Devices;
using DeviceDb.Api.Features.V1.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeviceDb.Api.Features.V1.Controllers;

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
    [HttpPost("", Name = nameof(AddDevice))]
    [ProducesResponseType(typeof(CreatedResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddDevice([FromBody] AddDeviceRequest request)
    {
        var device = Device.Create(DeviceId.Create(), request.Name, BrandId.From(request.Brand));
        await _repo.SaveDeviceAsync(device);

        return new CreatedAtRouteResult(
            nameof(GetDevice), 
            new {id = device.Id.Value}, 
            null);
    }

    /// <summary>
    /// Get a device by an Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet("{id}", Name = nameof(GetDevice))]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDevice(Guid id)
    {
        var device = await _repo.GetDeviceAsync(DeviceId.From(id));

        if (device == default)
            return NotFound();

        return new OkObjectResult(new DeviceResponse {
            Id = device.Id.Value,
            Name = device.Name,
            Brand = device.BrandId.Value,
            CreatedOn = device.CreatedOn,
        });
    }

    /// <summary>
    /// Get a list of all the devices available
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet("", Name = nameof(GetListOfDevices))]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    public async IAsyncEnumerable<DeviceResponse> GetListOfDevices()
    {
        await foreach (var device in _repo.GetAllDevicesAsync()) {
            yield return new DeviceResponse {
                Id = device.Id.Value,
                Name = device.Name,
                Brand = device.BrandId.Value,
                CreatedOn = device.CreatedOn,
            };
        };
    }

    /// <summary>
    /// Deletes a device to the device database
    /// </summary>
    /// <param name="request">The device data</param>
    /// <returns></returns>
    [HttpDelete("{id}", Name = nameof(DeleteDevice))]
    [ProducesResponseType(typeof(CreatedResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDevice(Guid id)
    {
        var device = await _repo.GetDeviceAsync(DeviceId.From(id));

        if (device == default)
            return NotFound();

        await _repo.DeleteDeviceAsync(DeviceId.From(id));

        return Ok();
    }
}
