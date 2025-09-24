using FluentAssertions;
using Items.Core.Domain.Model.ItemAgregate;

namespace Items.UnitTests.Domain.Model
{
    public class MeasureTypeTests
    {
        [Fact]
        public void Weight_Should_ReturnCorrectMeasureType()
        {
            // Act
            var result = MeasureType.Weight;

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("weight");
        }

        [Fact]
        public void Liquid_Should_ReturnCorrectMeasureType()
        {
            // Act
            var result = MeasureType.Liquid;

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(2);
            result.Name.Should().Be("liquid");
        }

        [Fact]
        public void List_Should_ReturnAllMeasureTypes()
        {
            // Act
            var result = MeasureType.List();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(MeasureType.Weight);
            result.Should().Contain(MeasureType.Liquid);
        }

        [Theory]
        [InlineData("weight", 1)]
        [InlineData("WEIGHT", 1)]
        [InlineData("Weight", 1)]
        [InlineData("liquid", 2)]
        [InlineData("LIQUID", 2)]
        [InlineData("Liquid", 2)]
        public void CreateFromName_WithValidName_ShouldReturnCorrectMeasureType(string name, int expectedId)
        {
            // Act
            var result = MeasureType.CreateFromName(name);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(expectedId);
            result.Value.Name.Should().Be(name.ToLowerInvariant());
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("invalid")]
        [InlineData("mass")]
        [InlineData("volume")]
        public void CreateFromName_WithInvalidName_ShouldReturnError(string invalidName)
        {
            // Act
            var result = MeasureType.CreateFromName(invalidName);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("unknown.measure.type");
        }

        [Theory]
        [InlineData(1, "weight")]
        [InlineData(2, "liquid")]
        public void CreateFromId_WithValidId_ShouldReturnCorrectMeasureType(int id, string expectedName)
        {
            // Act
            var result = MeasureType.CreateFromId(id);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(id);
            result.Value.Name.Should().Be(expectedName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(3)]
        [InlineData(100)]
        public void CreateFromId_WithInvalidId_ShouldReturnError(int invalidId)
        {
            // Act
            var result = MeasureType.CreateFromId(invalidId);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("unknown.measure.type");
        }

        [Fact]
        public void CreateFromId_Error_ShouldContainPossibleValues()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = MeasureType.CreateFromId(invalidId);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Message.Should().Contain("Possible values for MeasureType:");
            result.Error.Message.Should().Contain("weight");
            result.Error.Message.Should().Contain("liquid");
        }

        [Fact]
        public void CreateFromName_Error_ShouldContainPossibleValues()
        {
            // Arrange
            var invalidName = "invalid";

            // Act
            var result = MeasureType.CreateFromName(invalidName);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Message.Should().Contain("Possible values for MeasureType:");
            result.Error.Message.Should().Contain("weight");
            result.Error.Message.Should().Contain("liquid");
        }

        [Fact]
        public void ToString_ShouldReturnName()
        {
            // Arrange
            var measureType = MeasureType.Weight;

            // Act
            var result = measureType.ToString();

            // Assert
            result.Should().Be("weight");
        }

        [Fact]
        public void MeasureTypes_ShouldHaveUniqueIds()
        {
            // Arrange
            var measureTypes = MeasureType.List();

            // Act & Assert
            measureTypes.Select(mt => mt.Id).Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void MeasureTypes_ShouldHaveUniqueNames()
        {
            // Arrange
            var measureTypes = MeasureType.List();

            // Act & Assert
            measureTypes.Select(mt => mt.Name).Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void PrivateConstructor_ShouldCreateInstanceWithCorrectProperties()
        {
            // Arrange
            var id = 5;
            var name = "custom";

            // Act - используем рефлексию для тестирования приватного конструктора
            var measureType = Activator.CreateInstance(typeof(MeasureType), true) as MeasureType;
            // Устанавливаем значения через рефлексию (для полноты тестирования)
            var idProperty = typeof(MeasureType).GetProperty("Id");
            var nameProperty = typeof(MeasureType).GetProperty("Name");

            if (idProperty != null && idProperty.CanWrite)
                idProperty.SetValue(measureType, id);
            if (nameProperty != null && nameProperty.CanWrite)
                nameProperty.SetValue(measureType, name);

            // Assert
            measureType.Should().NotBeNull();
            measureType.Id.Should().Be(id);
            measureType.Name.Should().Be(name);
        }

        [Fact]
        public void Equality_ShouldWorkCorrectly()
        {
            // Arrange
            var weight1 = MeasureType.Weight;
            var weight2 = MeasureType.CreateFromId(1).Value;
            var liquid = MeasureType.Liquid;

            // Assert
            weight1.Should().Be(weight2);
            weight1.GetHashCode().Should().Be(weight2.GetHashCode());
            weight1.Should().NotBe(liquid);
        }
    }
}
