using CSharpFunctionalExtensions;
using FluentAssertions;
using Products.Core.Application.Dto.ProductAggregate;
using Products.Core.Application.UseCases.Query.GetAllProducts;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;
using Products.Core.Ports;
using NSubstitute;

namespace Products.UnitTests.Domain.Application
{
    public class GetAllProductsQueryTests
    {
        private readonly IProductRepository _mockRepository;
        private readonly GetAllProductsQueryHandler _handler;

        public GetAllProductsQueryTests()
        {
            _mockRepository = Substitute.For<IProductRepository>();
            _handler = new GetAllProductsQueryHandler(_mockRepository);
        }

        [Fact]
        public async Task Handle_WhenItemsExist_ReturnsGetAllItemsResponse()
        {
            // Arrange
            var items = new List<Product>
            {
                Product.Create("Item 1", MeasureType.Liquid).Value,
                Product.Create("Item 2", MeasureType.Weight).Value
            };

            var repositoryResponse = Maybe.From<List<Product>>(items);
            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllProductsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.HasValue);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Products.Count);

            var firstItem = result.Value.Products[0];
            Assert.Equal(items[0].Id, firstItem.Id);
            Assert.Equal(items[0].Name, firstItem.Name);
            Assert.Equal(items[0].IsArchive, firstItem.IsArchive);
            Assert.Equal(items[0].MeasureType.ToString(), firstItem.MeasureType);

            await _mockRepository.Received(1).GetAllAsync();
        }

        [Fact]
        public async Task Handle_WhenNoItemsExist_ReturnsNull()
        {
            // Arrange
            var emptyItemsList = new List<Product>();
            var repositoryResponse = Maybe.From<List<Product>>(emptyItemsList);

            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllProductsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.HasNoValue.Should().BeTrue();
            await _mockRepository.Received(1).GetAllAsync();
        }

        [Fact]
        public async Task Handle_WhenRepositoryReturnsNoValue_ReturnsNull()
        {
            // Arrange
            var repositoryResponse = Maybe<List<Product>>.None;

            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllProductsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.HasNoValue.Should().BeTrue();
            await _mockRepository.Received(1).GetAllAsync();
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockRepository.GetAllAsync().Returns<Task<Maybe<List<Product>>>>(x =>
                throw new InvalidOperationException("Database error"));

            var query = new GetAllProductsQuery();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public void MapItems_CorrectlyMapsItemToItemDto()
        {
            // Arrange
            var items = new List<Product>
            {
                Product.Create("Test Item", MeasureType.Liquid).Value
            };

            // Используем reflection для вызова private метода
            var handlerType = _handler.GetType();
            var mapItemsMethod = handlerType.GetMethod("MapProducts",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = (List<ProductDto>)mapItemsMethod.Invoke(_handler, new object[] { items });

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            var itemDto = result[0];
            Assert.Equal(items[0].Id, itemDto.Id);
            Assert.Equal(items[0].Name, itemDto.Name);
            Assert.Equal(items[0].IsArchive, itemDto.IsArchive);
            Assert.Equal(items[0].MeasureType.ToString(), itemDto.MeasureType);
        }

        [Fact]
        public async Task Handle_WhenItemsHaveDifferentMeasureTypes_ReturnsCorrectStringRepresentation()
        {
            // Arrange
            var items = new List<Product>
        {
            Product.Create("Item 1", MeasureType.Weight).Value,
            Product.Create("Item 2", MeasureType.Liquid).Value,
            Product.Create("Item 3", MeasureType.Weight).Value
        };

            var repositoryResponse = Maybe.From<List<Product>>(items);
            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllProductsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Value.Products.Count);
            Assert.Equal(MeasureType.Weight.ToString(), result.Value.Products[0].MeasureType);
            Assert.Equal(MeasureType.Liquid.ToString(), result.Value.Products[1].MeasureType);
            Assert.Equal(MeasureType.Weight.ToString(), result.Value.Products[2].MeasureType);
        }

        [Fact]
        public async Task Handle_WhenItemsHaveArchiveStatus_ReturnsCorrectArchiveStatus()
        {
            // Arrange
            var item1 = Product.Create("Active Item", MeasureType.Liquid).Value;
            var item2 = Product.Create("Archived Item", MeasureType.Weight).Value;

            // Архивируем второй элемент
            item2.MakeArchive();

            var items = new List<Product> { item1, item2 };
            var repositoryResponse = Maybe.From<List<Product>>(items);

            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllProductsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Products.Count);
            Assert.False(result.Value.Products[0].IsArchive); // Первый элемент не архивирован
            Assert.True(result.Value.Products[1].IsArchive);  // Второй элемент архивирован
        }

        [Fact]
        public async Task Handle_WhenCalled_CallsRepositoryOnce()
        {
            // Arrange
            var items = new List<Product>
        {
            Product.Create("Item 1", MeasureType.Liquid).Value
        };

            var repositoryResponse = Maybe.From<List<Product>>(items);
            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllProductsQuery();

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            await _mockRepository.Received(1).GetAllAsync();
        }

        [Fact]
        public async Task Handle_WithCancellationToken_PropagatesTokenToRepository()
        {
            // Arrange
            var items = new List<Product>
        {
            Product.Create("Item 1", MeasureType.Liquid).Value
        };

            var repositoryResponse = Maybe.From<List<Product>>(items);
            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllProductsQuery();
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            await _mockRepository.Received(1).GetAllAsync();
        }
    }
}
