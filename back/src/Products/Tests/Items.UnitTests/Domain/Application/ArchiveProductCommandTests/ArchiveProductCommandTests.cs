using FluentAssertions;
using Products.Core.Application.UseCases.Commands.ArchiveProduct;

namespace Products.UnitTests.Domain.Application.ArchiveProductCommandTests
{
    public class ArchiveProductCommandTests
    {
        [Fact]
        public void ShouldCreateCommand_WhenValidProductIdProvided()
        {
            // Arrange
            var validProductId = Guid.NewGuid();

            // Act
            var result = ArchiveProductCommand.Create(validProductId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ProductId.Should().Be(validProductId);
        }

        [Fact]
        public void ShouldReturnError_WhenEmptyGuidProvided()
        {
            // Arrange
            var emptyProductId = Guid.Empty;

            // Act
            var result = ArchiveProductCommand.Create(emptyProductId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Message.Should().Contain("Value is required for");
            result.Error.Message.Should().Contain("productId");
        }

        [Fact]
        public void ShouldReturnError_WhenDefaultGuidProvided()
        {
            // Arrange
            var defaultProductId = default(Guid);

            // Act
            var result = ArchiveProductCommand.Create(defaultProductId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789abc")]
        [InlineData("00000000-0000-0000-0000-000000000001")]
        [InlineData("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
        public void ShouldCreateCommand_WithVariousValidGuids(string guidString)
        {
            // Arrange
            var productId = Guid.Parse(guidString);

            // Act
            var result = ArchiveProductCommand.Create(productId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.ProductId.Should().Be(productId);
        }

        [Fact]
        public void ShouldCreateDifferentCommands_ForDifferentProductIds()
        {
            // Arrange
            var firstProductId = Guid.NewGuid();
            var secondProductId = Guid.NewGuid();

            // Act
            var firstResult = ArchiveProductCommand.Create(firstProductId);
            var secondResult = ArchiveProductCommand.Create(secondProductId);

            // Assert
            firstResult.Value.ProductId.Should().Be(firstProductId);
            secondResult.Value.ProductId.Should().Be(secondProductId);
            firstResult.Value.ProductId.Should().NotBe(secondResult.Value.ProductId);
        }
    }
}
