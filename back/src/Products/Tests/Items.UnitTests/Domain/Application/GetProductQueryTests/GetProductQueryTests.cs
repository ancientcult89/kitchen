using FluentAssertions;
using Products.Core.Application.UseCases.Query.GetProduct;

namespace Products.UnitTests.Domain.Application.GetProductQueryTests
{
    public class GetProductQueryTests
    {
        [Fact]
        public void Create_WithValidGuid_ShouldReturnSuccessResultWithQuery()
        {
            // Arrange
            var validItemId = Guid.NewGuid();

            // Act
            var result = GetProductQuery.Create(validItemId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ProductId.Should().Be(validItemId);
        }

        [Fact]
        public void Create_WithEmptyGuid_ShouldReturnError()
        {
            // Arrange
            var emptyItemId = Guid.Empty;

            // Act
            var result = GetProductQuery.Create(emptyItemId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void Create_WithDefaultGuid_ShouldReturnError()
        {
            // Arrange
            var defaultItemId = default(Guid);

            // Act
            var result = GetProductQuery.Create(defaultItemId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
