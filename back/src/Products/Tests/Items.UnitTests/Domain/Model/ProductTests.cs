using FluentAssertions;
using Primitives;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;
using Products.Core.Errors.Domain;

namespace Products.UnitTests.Domain.Model
{
    public class ProductTests
    {
        [Fact]
        public void Create_WithValidParameters_ShouldReturnSuccess()
        {
            // Arrange
            var productName = ProductName.Create("Test Product").Value;
            var measureType = MeasureType.Weight;

            // Act
            var result = Product.Create(productName, measureType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(productName, result.Value.Name);
            Assert.Equal(measureType, result.Value.MeasureType);
            Assert.False(result.Value.IsArchive);
            Assert.NotEqual(Guid.Empty, result.Value.Id);
        }

        [Fact]
        public void Create_WithNullProductName_ShouldReturnFailure()
        {
            // Arrange
            ProductName productName = null;
            var measureType = MeasureType.Weight;

            // Act
            var result = Product.Create(productName, measureType);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(GeneralErrors.ValueIsRequired(nameof(productName)).Code, result.Error.Code);
        }

        [Fact]
        public void Create_WithNullMeasureType_ShouldReturnFailure()
        {
            // Arrange
            var productName = ProductName.Create("Test Product").Value;
            MeasureType measureType = null;

            // Act
            var result = Product.Create(productName, measureType);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(GeneralErrors.ValueIsRequired(nameof(measureType)).Code, result.Error.Code);
        }

        [Fact]
        public void Create_WithValidParameters_ShouldGenerateUniqueId()
        {
            // Arrange
            var productName1 = ProductName.Create("Product 1").Value;
            var productName2 = ProductName.Create("Product 2").Value;
            var measureType = MeasureType.Weight;

            // Act
            var result1 = Product.Create(productName1, measureType);
            var result2 = Product.Create(productName2, measureType);

            // Assert
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);
            Assert.NotEqual(result1.Value.Id, result2.Value.Id);
        }

        [Fact]
        public void MakeArchive_WhenProductIsNotArchived_ShouldArchiveSuccessfully()
        {
            // Arrange
            var product = CreateTestProduct();

            // Act
            var result = product.MakeArchive();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(product.IsArchive);
        }

        [Fact]
        public void MakeArchive_WhenProductIsAlreadyArchived_ShouldReturnFailure()
        {
            // Arrange
            var product = CreateTestProduct();
            product.MakeArchive(); // Архивируем первый раз

            // Act
            var result = product.MakeArchive();

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(ProductErrors.ProductIsAlreadyArchived(product.Id).Code, result.Error.Code);
            Assert.True(product.IsArchive);
        }

        [Fact]
        public void MakeUnArchive_WhenProductIsArchived_ShouldUnArchiveSuccessfully()
        {
            // Arrange
            var product = CreateTestProduct();
            product.MakeArchive(); // Сначала архивируем

            // Act
            var result = product.MakeUnArchive();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(product.IsArchive);
        }

        [Fact]
        public void MakeUnArchive_WhenProductIsAlreadyUnArchived_ShouldReturnFailure()
        {
            // Arrange
            var product = CreateTestProduct(); // Продукт уже не архивирован

            // Act
            var result = product.MakeUnArchive();

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(ProductErrors.ProductIsAlreadyUnArchived(product.Id).Code, result.Error.Code);
            Assert.False(product.IsArchive);
        }

        [Fact]
        public void MakeArchive_And_MakeUnArchive_ShouldWorkInSequence()
        {
            // Arrange
            var product = CreateTestProduct();

            // Act & Assert - Архивируем
            var archiveResult = product.MakeArchive();
            Assert.True(archiveResult.IsSuccess);
            Assert.True(product.IsArchive);

            // Act & Assert - Разархивируем
            var unArchiveResult = product.MakeUnArchive();
            Assert.True(unArchiveResult.IsSuccess);
            Assert.False(product.IsArchive);

            // Act & Assert - Архивируем снова
            var archiveAgainResult = product.MakeArchive();
            Assert.True(archiveAgainResult.IsSuccess);
            Assert.True(product.IsArchive);
        }

        [Fact]
        public void Create_WithDifferentMeasureTypes_ShouldWorkCorrectly()
        {
            // Arrange
            var productName = ProductName.Create("Test Product").Value;
            var weightMeasureType = MeasureType.Weight;
            var liquidMeasureType = MeasureType.Liquid;

            // Act
            var weightProduct = Product.Create(productName, weightMeasureType).Value;
            var liquidProduct = Product.Create(productName, liquidMeasureType).Value;

            // Assert
            Assert.Equal(weightMeasureType, weightProduct.MeasureType);
            Assert.Equal(liquidMeasureType, liquidProduct.MeasureType);
        }

        private Product CreateTestProduct()
        {
            var productName = ProductName.Create("Test Product").Value;
            var measureType = MeasureType.Weight;
            return Product.Create(productName, measureType).Value;
        }
    }
}
