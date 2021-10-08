using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using DeviceDb.Api.Domain.Devices;
using AutoFixture;

namespace DeviceDb.Api.Tests.Domain.Devices
{
    public class BrandIdTests
    {
        static Fixture _fixture = new Fixture();

        public class WhenCreatingAnId
        {
            private readonly string _idString;
            private readonly BrandId _generatedId;

            public WhenCreatingAnId()
            {
                _idString = _fixture.Create<string>();
                _generatedId = BrandId.From(_idString);
            }
           
            [Fact]
            public void ValueObjectIsGenerated() => _generatedId.Should().NotBeNull();

            [Fact]
            public void CanGenerateStringVersionAgain() => _generatedId.ToString().Should().Be(_idString);
        }

        public class InvariantsAreProtected
        {
            //non null inputg protected by non-nullable string

            [Fact]
            public void ExceptionIsThrownIfBrandIdStringIsEmpty()
            {
                Action act = () => BrandId.From(String.Empty);
                act.Should().Throw<ArgumentException>();
            }

            [Fact]
            public void ExceptionIsThrownIfBrandIdStringIsWhitespace()
            {
                Action act = () => BrandId.From("   ");
                act.Should().Throw<ArgumentException>();
            }

            [Fact]
            public void ExceptionIsThrownIfBrandIdStringIsLongerThen100Characters()
            {
                var longId = string.Join("", Enumerable.Repeat(0, 101).Select(n => (char)new Random().Next(127)));

                Action act = () => BrandId.From(longId);
                act.Should().Throw<ArgumentException>();
            }
        }
    }
}
