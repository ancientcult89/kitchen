using FluentAssertions;
using Items.Core.Domain.Model.ItemAgregate;

namespace Items.UnitTests.Domain.Model
{
    public class MeasureTypeTests
    {
        [Fact]
        public void Weight_StaticProperty_ShouldReturnCorrectMeasureType()
        {
            // Act
            var weight = MeasureType.Weight;

            // Assert
            weight.Should().NotBeNull();
            weight.Name.Should().Be("weight");
        }

        [Fact]
        public void Liquid_StaticProperty_ShouldReturnCorrectMeasureType()
        {
            // Act
            var liquid = MeasureType.Liquid;

            // Assert
            liquid.Should().NotBeNull();
            liquid.Name.Should().Be("liquid");
        }

        [Fact]
        public void WeightAndLiquid_ShouldNotBeEqual()
        {
            // Act
            var weight = MeasureType.Weight;
            var liquid = MeasureType.Liquid;

            // Assert
            weight.Should().NotBe(liquid);
            weight.Name.Should().NotBe(liquid.Name);
        }

        [Theory]
        [InlineData("weight")]
        [InlineData("WEIGHT")]
        [InlineData("Weight")]
        [InlineData(" weight ")]
        public void CreateFromString_ValidWeightType_ShouldReturnWeightMeasureType(string input)
        {
            // Act
            var result = MeasureType.CreateFromString(input);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(MeasureType.Weight);
            result.Value.Name.Should().Be("weight");
        }

        [Theory]
        [InlineData("liquid")]
        [InlineData("LIQUID")]
        [InlineData("Liquid")]
        [InlineData(" liquid ")]
        public void CreateFromString_ValidLiquidType_ShouldReturnLiquidMeasureType(string input)
        {
            // Act
            var result = MeasureType.CreateFromString(input);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(MeasureType.Liquid);
            result.Value.Name.Should().Be("liquid");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CreateFromString_EmptyOrNullInput_ShouldReturnError(string input)
        {
            // Act
            var result = MeasureType.CreateFromString(input);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("measure.type.cannot.be.empty");
            result.Error.Message.Should().Be("Measure type name cannot be empty");
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("volume")]
        [InlineData("length")]
        [InlineData("mass")]
        public void CreateFromString_InvalidType_ShouldReturnError(string input)
        {
            // Act
            var result = MeasureType.CreateFromString(input);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("unknown.measure.type");
            result.Error.Message.Should().Be($"Unknown measure type: {input.Trim().ToLowerInvariant()}");
        }

        [Fact]
        public void CreateFromString_WithExtraWhitespace_ShouldTrimCorrectly()
        {
            // Arrange
            var input = "  weight  ";

            // Act
            var result = MeasureType.CreateFromString(input);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("weight");
        }

        [Fact]
        public void CreateFromString_CaseSensitivity_ShouldBeCaseInsensitive()
        {
            // Act
            var result1 = MeasureType.CreateFromString("WEIGHT");
            var result2 = MeasureType.CreateFromString("weight");
            var result3 = MeasureType.CreateFromString("Weight");

            // Assert
            result1.IsSuccess.Should().BeTrue();
            result2.IsSuccess.Should().BeTrue();
            result3.IsSuccess.Should().BeTrue();

            result1.Value.Should().Be(result2.Value);
            result2.Value.Should().Be(result3.Value);
            result1.Value.Name.Should().Be("weight");
        }

        [Fact]
        public void Equality_SameMeasureTypes_ShouldBeEqual()
        {
            // Arrange
            var weight1 = MeasureType.Weight;
            var weight2 = MeasureType.CreateFromString("weight").Value;

            // Act & Assert
            weight1.Should().Be(weight2);
            weight1.GetHashCode().Should().Be(weight2.GetHashCode());
            (weight1 == weight2).Should().BeTrue();
        }

        [Fact]
        public void Equality_DifferentMeasureTypes_ShouldNotBeEqual()
        {
            // Arrange
            var weight = MeasureType.Weight;
            var liquid = MeasureType.Liquid;

            // Act & Assert
            weight.Should().NotBe(liquid);
            weight.GetHashCode().Should().NotBe(liquid.GetHashCode());
            (weight == liquid).Should().BeFalse();
        }

        [Fact]
        public void ToString_ShouldReturnName()
        {
            // Arrange
            var weight = MeasureType.Weight;

            // Act
            var result = weight.ToString();

            // Assert
            result.Should().Be("weight");
        }

        [Fact]
        public void PrivateConstructor_ShouldSetNameCorrectly()
        {
            // This test is more for documentation purposes since we can't directly test private constructor
            // but we can verify that the static properties work correctly

            // Act
            var weight = MeasureType.Weight;
            var liquid = MeasureType.Liquid;

            // Assert
            weight.Name.Should().Be("weight");
            liquid.Name.Should().Be("liquid");
        }

        [Fact]
        public void ValueObject_Immutability_ShouldBePreserved()
        {
            // Arrange
            var weight = MeasureType.Weight;

            // Act & Assert - Attempt to verify immutability through reflection or behavior
            weight.Name.Should().Be("weight");
            // The Name property should have private setter, making it immutable after construction
        }
    }
}
