using FluentAssertions;
using Products.Core.Application.UseCases.Commands.AddProduct;
using Products.Core.Domain.Model.SharedKernel;

namespace Products.UnitTests.Domain.Application.AddProductCommandTests
{
    public class AddProductCommandTests
    {
        [Theory]
        [InlineData("Valid Product Name", 1)]
        [InlineData("Another Product", 2)]
        public void ShouldCreateCommand_WhenValidParametersProvided(string name, int measureTypeId)
        {
            // Act
            var result = AddProductCommand.Create(name, measureTypeId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Value.Should().Be(name);
            result.Value.MeasureType.Id.Should().Be(measureTypeId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        public void ShouldReturnError_WhenNameIsNullOrWhiteSpace(string invalidName)
        {
            // Arrange
            var validMeasureTypeId = MeasureType.Weight.Id;

            // Act
            var result = AddProductCommand.Create(invalidName, validMeasureTypeId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Message.Should().Contain("Value is invalid for");
            result.Error.Message.Should().Contain("name");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        [InlineData(999)]
        public void ShouldReturnError_WhenMeasureTypeIdIsInvalid(int invalidMeasureTypeId)
        {
            // Arrange
            var validName = "Valid Product Name";

            // Act
            var result = AddProductCommand.Create(validName, invalidMeasureTypeId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Message.Should().Contain("Possible values for MeasureType:");
        }

        [Fact]
        public void ShouldReturnError_WhenBothParametersAreInvalid()
        {
            // Arrange
            var invalidName = "";
            var invalidMeasureTypeId = 999;

            // Act
            var result = AddProductCommand.Create(invalidName, invalidMeasureTypeId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            // Проверяем, что возвращается ошибка для первого невалидного параметра (name)
            result.Error.Message.Should().Contain("name");
        }
    }
}
