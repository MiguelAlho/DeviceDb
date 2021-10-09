using System;

namespace DeviceDb.Api.Features.V1.Models
{
    internal record DeviceResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Brand { get; init; }
        public DateTime CreatedOn { get; init; }
    }
}
