using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using DeviceDb.Api.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace DeviceDb.Api.Features.V1.Models
{
    public class AddDeviceRequestTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public void CompleteRequestModelValidates()
        {
            var request = new AddDeviceRequest() { Name = _fixture.Create<string>(), Brand = _fixture.Create<string>() };
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(request, context, results, true).Should().BeTrue();
        }

        [Fact]
        public void MissingNameInvalidatesRequest()
        {
            var request = new AddDeviceRequest() { Name = string.Empty, Brand = _fixture.Create<string>() };
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(request, context, results, true).Should().BeFalse();
        }

        [Fact]
        public void MissingBrandInvalidatesRequest()
        {
            var request = new AddDeviceRequest() { Name = _fixture.Create<string>(), Brand = string.Empty };
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(request, context, results, true).Should().BeFalse();
        }

        [Fact]
        public void ExtensileyLongNameInvalidatesRequest()
        {
            var request = new AddDeviceRequest() { Name = StringHelpers.GetLongString(100), Brand = _fixture.Create<string>() };
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(request, context, results, true).Should().BeFalse();
        }

        [Fact]
        public void ExtensivelyLongBrandInvalidatesRequest()
        {
            var request = new AddDeviceRequest() { Name = _fixture.Create<string>(), Brand = StringHelpers.GetLongString(100) };
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(request, context, results, true).Should().BeFalse();
        }

    }
}
