using FluentAssertions;
using Products.Core.Application.Dto.ProductAggregate;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;

namespace Products.UnitTests.Domain.Application.DTO
{
    public class PorductMappingTests
    {
        [Fact]
        public void ToDto_WithValidProduct_ReturnsCorrectProductDto()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productName = ProductName.Create("Test Product").Value;
            var measureType = MeasureType.Weight; // Используем статическое свойство

            var product = Product.Create(productName, measureType).Value;

            // Act
            var result = product.ToDto();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(product.Id);
            result.Name.Should().Be("Test Product");
            result.MeasureTypeId.Should().Be(1); // Weight имеет Id = 1
            result.IsArchive.Should().BeFalse();
        }

        [Fact]
        public void ToDto_WithWeightMeasureType_ReturnsCorrectMeasureTypeId()
        {
            // Arrange
            var productName = ProductName.Create("Weight Product").Value;
            var measureType = MeasureType.Weight;

            var product = Product.Create(productName, measureType).Value;

            // Act
            var result = product.ToDto();

            // Assert
            result.MeasureTypeId.Should().Be(1);
        }

        [Fact]
        public void ToDto_WithLiquidMeasureType_ReturnsCorrectMeasureTypeId()
        {
            // Arrange
            var productName = ProductName.Create("Liquid Product").Value;
            var measureType = MeasureType.Liquid;

            var product = Product.Create(productName, measureType).Value;

            // Act
            var result = product.ToDto();

            // Assert
            result.MeasureTypeId.Should().Be(2); // Liquid имеет Id = 2
        }

        [Fact]
        public void ToDto_WithArchivedProduct_ReturnsArchivedProductDto()
        {
            // Arrange
            var productName = ProductName.Create("Archived Product").Value;
            var measureType = MeasureType.Liquid;

            var product = Product.Create(productName, measureType).Value;
            product.MakeArchive(); // Архивируем продукт

            // Act
            var result = product.ToDto();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(product.Id);
            result.Name.Should().Be("Archived Product");
            result.MeasureTypeId.Should().Be(2);
            result.IsArchive.Should().BeTrue();
        }

        [Fact]
        public void ToDto_WithUnarchivedProduct_ReturnsUnarchivedProductDto()
        {
            // Arrange
            var productName = ProductName.Create("Unarchived Product").Value;
            var measureType = MeasureType.Weight;

            var product = Product.Create(productName, measureType).Value;
            // Продукт создается неархивированным по умолчанию

            // Act
            var result = product.ToDto();

            // Assert
            result.Should().NotBeNull();
            result.IsArchive.Should().BeFalse();
            result.MeasureTypeId.Should().Be(1);
        }

        [Fact]
        public void ToDto_ProductNameValue_IsCorrectlyMappedToString()
        {
            // Arrange
            var expectedName = "Special Product Name";
            var productName = ProductName.Create(expectedName).Value;
            var measureType = MeasureType.Weight;

            var product = Product.Create(productName, measureType).Value;

            // Act
            var result = product.ToDto();

            // Assert
            result.Name.Should().Be(expectedName);
            result.Name.Should().BeOfType<string>();
        }

        [Fact]
        public void ToDto_ProductId_IsPreserved()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            var productName = ProductName.Create("Test Product").Value;
            var measureType = MeasureType.Liquid;

            var product = Product.Create(productName, measureType).Value;

            // Используем рефлексию для установки Id (только для тестирования)
            var idField = typeof(Product).BaseType.GetProperty("Id");
            idField.SetValue(product, expectedId);

            // Act
            var result = product.ToDto();

            // Assert
            result.Id.Should().Be(expectedId);
            result.MeasureTypeId.Should().Be(2); // Проверяем, что Liquid остался
        }

        [Fact]
        public void ToDto_MeasureTypeFromCreateFromId_ReturnsCorrectDto()
        {
            // Arrange
            var productName = ProductName.Create("Created From Id Product").Value;
            var measureType = MeasureType.CreateFromId(1).Value; // Создаем через фабричный метод

            var product = Product.Create(productName, measureType).Value;

            // Act
            var result = product.ToDto();

            // Assert
            result.MeasureTypeId.Should().Be(1);
            result.Name.Should().Be("Created From Id Product");
        }

        [Fact]
        public void ToDto_MeasureTypeFromCreateFromName_ReturnsCorrectDto()
        {
            // Arrange
            var productName = ProductName.Create("Created From Name Product").Value;
            var measureType = MeasureType.CreateFromName("liquid").Value; // Создаем через фабричный метод

            var product = Product.Create(productName, measureType).Value;

            // Act
            var result = product.ToDto();

            // Assert
            result.MeasureTypeId.Should().Be(2); // liquid имеет Id = 2
            result.Name.Should().Be("Created From Name Product");
        }
    }
}
