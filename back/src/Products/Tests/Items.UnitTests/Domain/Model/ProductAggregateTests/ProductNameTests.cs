using Primitives;
using Products.Core.Domain.Model.ProductAggregate;

namespace Products.UnitTests.Domain.Model.ProductAggregateTests
{
    public class ProductNameTests
    {
        [Theory]
        [InlineData("Valid Product Name")]
        [InlineData("Product 123")]
        [InlineData("A")] // Минимальная допустимая длина
        [InlineData("Very Long Product Name With Many Words And Characters")]
        public void Create_WithValidName_ShouldReturnSuccess(string validName)
        {
            // Act
            var result = ProductName.Create(validName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(validName, result.Value.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void Create_WithNullOrWhiteSpaceName_ShouldReturnFailure(string invalidName)
        {
            // Act
            var result = ProductName.Create(invalidName);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(GeneralErrors.ValueIsInvalid(nameof(invalidName)).Code, result.Error.Code);
        }

        [Fact]
        public void Equality_WhenSameValues_ShouldBeEqual()
        {
            // Arrange
            var name1 = ProductName.Create("Test Product").Value;
            var name2 = ProductName.Create("Test Product").Value;

            // Act & Assert
            Assert.Equal(name1, name2);
            Assert.True(name1 == name2);
            Assert.False(name1 != name2);
        }

        [Fact]
        public void Equality_WhenDifferentValues_ShouldNotBeEqual()
        {
            // Arrange
            var name1 = ProductName.Create("Product A").Value;
            var name2 = ProductName.Create("Product B").Value;

            // Act & Assert
            Assert.NotEqual(name1, name2);
            Assert.False(name1 == name2);
            Assert.True(name1 != name2);
        }

        [Fact]
        public void Equality_WhenDifferentCase_ShouldBeCaseSensitive()
        {
            // Arrange
            var name1 = ProductName.Create("Product").Value;
            var name2 = ProductName.Create("product").Value;

            // Act & Assert
            Assert.NotEqual(name1, name2);
            Assert.False(name1 == name2);
            Assert.True(name1 != name2);
        }

        [Fact]
        public void GetHashCode_WhenSameValues_ShouldBeEqual()
        {
            // Arrange
            var name1 = ProductName.Create("Test Product").Value;
            var name2 = ProductName.Create("Test Product").Value;

            // Act & Assert
            Assert.Equal(name1.GetHashCode(), name2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_WhenDifferentValues_ShouldNotBeEqual()
        {
            // Arrange
            var name1 = ProductName.Create("Product A").Value;
            var name2 = ProductName.Create("Product B").Value;

            // Act & Assert
            Assert.NotEqual(name1.GetHashCode(), name2.GetHashCode());
        }
    }
}
