using DeviceDb.Api.Domain.Devices;
using DeviceDb.Api.Features.V1.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DeviceDb.Api.Features.V1.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceRepository _repo;

    public DeviceController(IDeviceRepository repo) => _repo = repo;

   
    /// <summary>
    /// Get a device
    /// </summary>
    /// <param name="id">Guid-based Id for the device in the system.</param>
    /// <returns>The device's details</returns>
    /// <response code="200">Produced if the device exists and returns the device's details</response>
    /// <response code="400">Produced if the device id is not a valid Guid</response>
    /// <response code="404">Produced if the device does not exist</response>
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
    /// <returns>A list of all the recorded devices with their details.</returns>
    /// <response code="200">The list of recorded devices.</response>
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
    /// Search devices by brand
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet("search", Name = nameof(SearchDevicesByBrand))]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status400BadRequest)]
    public async IAsyncEnumerable<DeviceResponse> SearchDevicesByBrand([FromQuery] string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentNullException(nameof(brand));

        await foreach (var device in _repo.GetAllDevicesByBrandAsync(BrandId.From(brand))) {
            yield return new DeviceResponse {
                Id = device.Id.Value,
                Name = device.Name,
                Brand = device.BrandId.Value,
                CreatedOn = device.CreatedOn,
            };
        };
    }

    /// <summary>
    /// Adds a device to the device database, generating a resource link for it.
    /// </summary>
    /// <remarks>
    /// The POST operation takes a single device definition in the request body and adds it as a new 
    /// device in the store.
    /// 
    /// Sample request payload:
    /// 
    ///     {
    ///         "name" : "device name",
    ///         "brand": "brand key"
    ///     }
    /// 
    /// The brand property should contain the common string that references the brand in the system.
    /// 
    /// Each device posted is considered new and a new Id is generated for it. The POST operation does 
    /// not handle request de-duplication. Retries for the same payload will generate multiple devices 
    /// in the store.
    /// </remarks>
    /// <param name="request">The data that describes the device</param>
    /// <returns>The newly generated resource location</returns>
    /// <response code="201">Produced if the device is stored. The response's location header will contain the link to the generated resource.</response>
    /// <response code="400">Produced if the payload is malformed or the data supplied does not conform to the defined input rules.</response>
    [HttpPost("", Name = nameof(AddDevice))]
    [ProducesResponseType(typeof(CreatedResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddDevice([FromBody] AddDeviceRequest request)
    {
        var device = Device.Create(DeviceId.Create(), request.Name, BrandId.From(request.Brand));
        await _repo.SaveDeviceAsync(device);

        return new CreatedAtRouteResult(
            nameof(GetDevice),
            new { id = device.Id.Value },
            null);
    }


    /// <summary>
    /// Deletes a device
    /// </summary>
    /// <returns></returns>
    /// <response code="204">Produced if the device was deleted from the data store</response>
    /// <response code="400">Produced if the device id is malformed in the url</response>
    /// <response code="404">Produced if the device was not found in the data store</response>
    [HttpDelete("{id}", Name = nameof(DeleteDevice))]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDevice(Guid id)
    {
        var device = await _repo.GetDeviceAsync(DeviceId.From(id));

        if (device == default)
            return NotFound();

        await _repo.DeleteDeviceAsync(DeviceId.From(id));

        return Ok();
    }

    /// <summary>
    /// Replaces the mutable properties of a device, based on the JsonPatch supplied.
    /// </summary>
    /// <remarks>
    /// The Patch request will update the device's mutable properties. The immutable properties (id, createdOn) 
    /// will not be modified. 'Replace' is the only supported patch operation on the device resource. Other 
    /// operation types will generate a Bad Request response.
    /// 
    /// Request must be made with Content-Type header defined as "application/json-patch+json".
    /// 
    /// Sample patch request payload:
    /// 
    ///     [
    ///     	{ "op" : "replace", "path":"/name", "value": "new name" }
    ///     	{ "op" : "replace", "path":"/brand", "value": "new brand" }
    ///     ]
    /// 
    /// </remarks>
    /// <param name="id">The device id</param>
    /// <param name="patchDocument">The JSON Patch document with the list of replace operations</param>
    /// <returns></returns>
    /// <response code="204">Produced if the device was correctly patched</response>
    /// <response code="400">Produced if the device id is malformed in the url or the json patch payload is invalid</response>
    /// <response code="404">Produced if the device was not found in the data store</response>
    [HttpPatch("{id}", Name = nameof(PatchDevice))]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchDevice(Guid id, [FromBody] JsonPatchDocument<UpdateableDeviceRequest> patchDocument)
    {
        if(patchDocument == default)
            return BadRequest();

        if (patchDocument.Operations.Any(o => o.op != "replace"))
            return new BadRequestObjectResult(new { error = "Only replace patches supported in this domain." });

        var device = await _repo.GetDeviceAsync(DeviceId.From(id));
        if (device == default)
            return NotFound();

        var update = new UpdateableDeviceRequest() { Name = device.Name, Brand = device.BrandId.Value};
        patchDocument.ApplyTo(update);

        //now that we have the final values for the mutable properties, we can apply the Update Device domain command
        device.Update(new UpdateDevice { Name = update.Name, Brand = update.Brand });

        await _repo.SaveDeviceAsync(device);
        return NoContent();
    }
}
