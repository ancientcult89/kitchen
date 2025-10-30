using CSharpFunctionalExtensions;
using FluentAssertions;
using Products.Core.Domain.Model.MeasureAggregate;

namespace Products.UnitTests.Domain.Model.MeasureAggregateTests
{
    public class MeasureShortNameTests
    {
        private const int MaxLength = 6;

        [Theory]
        [InlineData("кг")]
        [InlineData("м")]
        [InlineData("л")]
        [InlineData("шт")]
        [InlineData("уп")]
        [InlineData("грамм")]
        [InlineData("метров")] // Максимальная длина
        public void Create_WithValidName_ShouldReturnMeasureShortName(string validName)
        {
            // Act
            var result = MeasureShortName.Create(validName);

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
            var result = MeasureShortName.Create(invalidName);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Theory]
        [InlineData("килограмм")] // 9 символов
        [InlineData("verylong")] // 8 символов
        [InlineData("1234567")] // 7 символов
        public void Create_WithNameExceedingMaxLength_ShouldReturnError(string longName)
        {
            // Act
            var result = MeasureShortName.Create(longName);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void Create_WithNameAtMaxLength_ShouldSucceed()
        {
            // Arrange
            var maxLengthName = new string('a', MaxLength); // "aaaaaa"

            // Act
            var result = MeasureShortName.Create(maxLengthName);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(maxLengthName);
            result.Value.Value.Should().HaveLength(MaxLength);
        }

        [Fact]
        public void Create_WithNameOneCharOverMaxLength_ShouldFail()
        {
            // Arrange
            var tooLongName = new string('a', MaxLength + 1); // "aaaaaaa"

            // Act
            var result = MeasureShortName.Create(tooLongName);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void TwoMeasureShortNames_WithSameValue_ShouldBeEqual()
        {
            // Arrange
            var name1 = MeasureShortName.Create("кг").Value;
            var name2 = MeasureShortName.Create("кг").Value;

            // Act & Assert
            name1.Should().BeEquivalentTo(name2);
            name1.GetHashCode().Should().Be(name2.GetHashCode());
        }

        [Fact]
        public void TwoMeasureShortNames_WithDifferentValues_ShouldNotBeEqual()
        {
            // Arrange
            var name1 = MeasureShortName.Create("кг").Value;
            var name2 = MeasureShortName.Create("г").Value;

            // Act & Assert
            name1.Should().NotBeEquivalentTo(name2);
            name1.GetHashCode().Should().NotBe(name2.GetHashCode());
        }

        [Fact]
        public void MeasureShortName_ShouldBeValueObject()
        {
            // Arrange
            var name = MeasureShortName.Create("л").Value;

            // Act & Assert
            name.Should().BeAssignableTo<ValueObject>();
        }

        [Theory]
        [InlineData("  кг  ", "  кг  ")] // Проверка, что пробелы не триммируются
        [InlineData("  м", "  м")] // Начальные пробелы
        [InlineData("л  ", "л  ")] // Конечные пробелы
        public void Create_WithNameContainingSpaces_ShouldPreserveSpaces(string inputName, string expectedName)
        {
            // Act
            var result = MeasureShortName.Create(inputName);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(expectedName);
        }

        [Theory]
        [InlineData("  кг  ")] // Пробелы в начале и конце (длина 6)
        [InlineData("   м")] // Пробелы в начале (длина 4)
        public void Create_WithSpacesWithinMaxLength_ShouldSucceed(string nameWithSpaces)
        {
            // Act
            var result = MeasureShortName.Create(nameWithSpaces);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(nameWithSpaces);
            result.Value.Value.Should().HaveLength(nameWithSpaces.Length);
        }

        [Fact]
        public void Create_WithSpacesExceedingMaxLength_ShouldFail()
        {
            // Arrange
            var nameWithManySpaces = "      кг"; // 7 символов

            // Act
            var result = MeasureShortName.Create(nameWithManySpaces);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
        }
    }
}
