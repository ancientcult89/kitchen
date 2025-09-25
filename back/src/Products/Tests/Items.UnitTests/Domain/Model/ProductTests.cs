using FluentAssertions;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;

namespace Products.UnitTests.Domain.Model
{
    public class ProductTests
    {
        [Fact]
        public void Create_WithValidParameters_ReturnsSuccessResult()
        {
            // Arrange
            var name = "Test Item";
            var measureType = MeasureType.Weight;

            // Act
            var result = Product.Create(name, measureType);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be(name);
            result.Value.MeasureType.Should().Be(measureType);
            result.Value.IsArchive.Should().BeFalse();
            result.Value.Id.Should().NotBe(Guid.Empty);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_WithInvalidName_ReturnsErrorResult(string invalidName)
        {
            // Arrange
            var measureType = MeasureType.Weight;

            // Act
            var result = Product.Create(invalidName, measureType);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void Create_WithNullMeasureType_ReturnsErrorResult()
        {
            // Arrange
            var name = "Test Item";

            // Act
            var result = Product.Create(name, null);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void MakeArchive_WhenNotArchived_ArchivesItemAndReturnsSuccess()
        {
            // Arrange
            var item = CreateTestItem();
            item.IsArchive.Should().BeFalse();

            // Act
            var result = item.MakeArchive();

            // Assert
            result.IsSuccess.Should().BeTrue();
            item.IsArchive.Should().BeTrue();
        }

        [Fact]
        public void MakeArchive_WhenAlreadyArchived_ReturnsError()
        {
            // Arrange
            var item = CreateTestItem();
            item.MakeArchive(); // Архивируем первый раз
            var originalId = item.Id;

            // Act
            var result = item.MakeArchive();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("item.is.already.archived");
            result.Error.Message.Should().Contain(originalId.ToString());
            item.IsArchive.Should().BeTrue();
        }

        [Fact]
        public void MakeUnArchive_WhenArchived_UnarchivesItemAndReturnsSuccess()
        {
            // Arrange
            var item = CreateTestItem();
            item.MakeArchive();
            item.IsArchive.Should().BeTrue();

            // Act
            var result = item.MakeUnArchive();

            // Assert
            result.IsSuccess.Should().BeTrue();
            item.IsArchive.Should().BeFalse();
        }

        [Fact]
        public void MakeUnArchive_WhenAlreadyUnArchived_ReturnsError()
        {
            // Arrange
            var item = CreateTestItem();
            item.IsArchive.Should().BeFalse();
            var originalId = item.Id;

            // Act
            var result = item.MakeUnArchive();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("item.is.already.unarchived");
            result.Error.Message.Should().Contain(originalId.ToString());
            item.IsArchive.Should().BeFalse(); // Состояние не должно измениться
        }

        [Fact]
        public void Errors_ItemIsAlreadyArchived_WithEmptyGuid_ThrowsArgumentException()
        {
            // Arrange
            var action = () => Product.Errors.ItemIsAlreadyArchived(Guid.Empty);

            // Act & Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Errors_ItemIsAlreadyUnArchived_WithEmptyGuid_ThrowsArgumentException()
        {
            // Arrange
            var action = () => Product.Errors.ItemIsAlreadyUnArchived(Guid.Empty);

            // Act & Assert
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Errors_ItemIsAlreadyArchived_WithValidGuid_ReturnsErrorWithCorrectProperties()
        {
            // Arrange
            var itemId = Guid.NewGuid();

            // Act
            var error = Product.Errors.ItemIsAlreadyArchived(itemId);

            // Assert
            error.Should().NotBeNull();
            error.Code.Should().Be("item.is.already.archived");
            error.Message.Should().Contain(itemId.ToString());
        }

        [Fact]
        public void Errors_ItemIsAlreadyUnArchived_WithValidGuid_ReturnsErrorWithCorrectProperties()
        {
            // Arrange
            var itemId = Guid.NewGuid();

            // Act
            var error = Product.Errors.ItemIsAlreadyUnArchived(itemId);

            // Assert
            error.Should().NotBeNull();
            error.Code.Should().Be("item.is.already.unarchived");
            error.Message.Should().Contain(itemId.ToString());
        }

        private Product CreateTestItem()
        {
            var result = Product.Create("Test Item", MeasureType.Weight);
            return result.Value;
        }
    }
}
