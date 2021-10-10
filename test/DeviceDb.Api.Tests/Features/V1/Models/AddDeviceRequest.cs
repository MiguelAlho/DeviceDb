using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using DeviceDb.Api.Features.V1.Models;
using DeviceDb.Api.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace DeviceDb.Api.Tests.Features.V1.Models;

public class AddDeviceRequestTests
{
    private static readonly Fixture _fixture = new();

    [Fact]
    public void CompleteRequestModelValidates()
    {
        var request = new AddDeviceRequest() { Name = _fixture.Create<string>(), Brand = _fixture.Create<string>() };
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(request, context, results, true).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidInputs))]
    public void MissingInputObjectInvalidatesRequest(AddDeviceRequest request)
    {
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(request, context, results, true).Should().BeFalse();
    }

    public static IEnumerable<object[]> InvalidInputs => new List<object[]>
    {
            new object[] { new AddDeviceRequest() { Name = string.Empty, Brand = _fixture.Create<string>() } },
            new object[] { new AddDeviceRequest() { Name = _fixture.Create<string>(), Brand = string.Empty } },
            new object[] { new AddDeviceRequest() { Name = StringHelpers.GetLongString(100), Brand = _fixture.Create<string>() } },
            new object[] { new AddDeviceRequest() { Name = _fixture.Create<string>(), Brand = StringHelpers.GetLongString(100) } },
        };

}

