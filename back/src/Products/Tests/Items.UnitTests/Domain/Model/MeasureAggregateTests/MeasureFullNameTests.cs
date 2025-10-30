using CSharpFunctionalExtensions;
using FluentAssertions;
using Products.Core.Domain.Model.MeasureAggregate;

namespace Products.UnitTests.Domain.Model.MeasureAggregateTests
{
    public class MeasureFullNameTests
    {
        [Theory]
        [InlineData("Килограмм")]
        [InlineData("Метр")]
        [InlineData("Литр")]
        [InlineData("Штука")]
        public void Create_WithValidName_ShouldReturnMeasureFullName(string validName)
        {
            // Act
            var result = MeasureFullName.Create(validName);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Value.Should().Be(validName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void Create_WithInvalidName_ShouldReturnError(string invalidName)
        {
            // Act
            var result = MeasureFullName.Create(invalidName);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void Create_WithValidName_ShouldHaveCorrectValue()
        {
            // Arrange
            var expectedName = "Килограмм";

            // Act
            var measureFullName = MeasureFullName.Create(expectedName).Value;

            // Assert
            measureFullName.Value.Should().Be(expectedName);
        }

        [Fact]
        public void TwoMeasureFullNames_WithSameValue_ShouldBeEqual()
        {
            // Arrange
            var name1 = MeasureFullName.Create("Метр").Value;
            var name2 = MeasureFullName.Create("Метр").Value;

            // Act & Assert
            name1.Should().BeEquivalentTo(name2);
            name1.GetHashCode().Should().Be(name2.GetHashCode());
        }

        [Fact]
        public void TwoMeasureFullNames_WithDifferentValues_ShouldNotBeEqual()
        {
            // Arrange
            var name1 = MeasureFullName.Create("Метр").Value;
            var name2 = MeasureFullName.Create("Килограмм").Value;

            // Act & Assert
            name1.Should().NotBeEquivalentTo(name2);
            name1.GetHashCode().Should().NotBe(name2.GetHashCode());
        }

        [Fact]
        public void MeasureFullName_ShouldBeValueObject()
        {
            // Arrange
            var name = MeasureFullName.Create("Литр").Value;

            // Act & Assert
            name.Should().BeAssignableTo<ValueObject>();
        }

        [Theory]
        [InlineData("  Килограмм  ", "Килограмм")] // Проверка, что пробелы не тримятся
        public void Create_WithNameContainingSpaces_ShouldPreserveSpaces(string inputName, string expectedName)
        {
            // Act
            var result = MeasureFullName.Create(inputName);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(inputName); // Значение должно сохраняться как есть
            result.Value.Value.Should().NotBe(expectedName); // Должно отличаться от тримленной версии
        }

        [Fact]
        public void Create_WithLongValidName_ShouldSucceed()
        {
            // Arrange
            var longName = new string('a', 100); // Длинное, но валидное имя

            // Act
            var result = MeasureFullName.Create(longName);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(longName);
        }
    }
}
