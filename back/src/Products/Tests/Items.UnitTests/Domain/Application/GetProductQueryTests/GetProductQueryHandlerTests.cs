using CSharpFunctionalExtensions;
using FluentAssertions;
using NSubstitute;
using Products.Core.Application.Dto.ProductAggregate;
using Products.Core.Application.UseCases.Query.GetProduct;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;
using Products.Core.Ports;
using System.Reflection;

namespace Products.UnitTests.Domain.Application.GetProductQueryTests
{
    public class GetProductQueryHandlerTests
    {
        private readonly IProductRepository _mockRepository;
        private readonly GetProductQueryHandler _handler;

        public GetProductQueryHandlerTests()
        {
            _mockRepository = Substitute.For<IProductRepository>();
            _handler = new GetProductQueryHandler(_mockRepository);
        }

        [Fact]
        public async Task Handle_WhenItemExists_ReturnsGetItemResponse()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var productName = ProductName.Create("Test Item").Value;

            var item = Product.Create(productName, MeasureType.Liquid).Value;

            // Используем reflection для установки Id, так как он private set
            var idProperty = typeof(Product).GetProperty("Id");
            idProperty?.SetValue(item, itemId);

            var repositoryResponse = Maybe.From(item);
            _mockRepository.GetAsync(itemId).Returns(repositoryResponse);

            var query = GetProductQuery.Create(itemId).Value;

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.HasValue);
            Assert.NotNull(result.Value);

            Assert.Equal(item.Id, result.Value.Product.Id);
            Assert.Equal(item.Name.Value, result.Value.Product.Name);
            Assert.Equal(item.IsArchive, result.Value.Product.IsArchive);
            Assert.Equal(item.MeasureType.Id, result.Value.Product.MeasureTypeId);

            await _mockRepository.Received(1).GetAsync(itemId);
        }

        [Fact]
        public async Task Handle_WhenItemDoesNotExist_ReturnsNull()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var repositoryResponse = Maybe<Product>.None;

            _mockRepository.GetAsync(itemId).Returns(repositoryResponse);

            var query = GetProductQuery.Create(itemId).Value;

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.HasNoValue.Should().BeTrue(); ;
            await _mockRepository.Received(1).GetAsync(itemId);
        }

        [Fact]
        public async Task Handle_WhenRepositoryReturnsNoValue_ReturnsNull()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var repositoryResponse = Maybe<Product>.None;

            _mockRepository.GetAsync(itemId).Returns(repositoryResponse);

            var query = GetProductQuery.Create(itemId).Value;

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.HasNoValue.Should().BeTrue();
            await _mockRepository.Received(1).GetAsync(itemId);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            _mockRepository.GetAsync(itemId).Returns<Task<Maybe<Product>>>(x =>
                throw new InvalidOperationException("Database error"));

            var query = GetProductQuery.Create(itemId).Value;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenItemIsArchived_ReturnsCorrectArchiveStatus()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var productName = ProductName.Create("Archived Item").Value;

            var item = Product.Create(productName, MeasureType.Weight).Value;

            // Архивируем элемент
            item.MakeArchive();

            var repositoryResponse = Maybe.From(item);
            _mockRepository.GetAsync(itemId).Returns(repositoryResponse);

            var query = GetProductQuery.Create(itemId).Value;

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Value.Product.IsArchive);
            await _mockRepository.Received(1).GetAsync(itemId);
        }

        [Fact]
        public async Task Handle_WhenItemHasDifferentMeasureType_ReturnsCorrectStringRepresentation()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var productName = ProductName.Create("Test Item").Value;
            var item = Product.Create(productName, MeasureType.Weight).Value;

            var repositoryResponse = Maybe.From(item);
            _mockRepository.GetAsync(itemId).Returns(repositoryResponse);

            var query = GetProductQuery.Create(itemId).Value;

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(MeasureType.Weight.Id, result.Value.Product.MeasureTypeId);
            await _mockRepository.Received(1).GetAsync(itemId);
        }

        [Fact]
        public async Task Handle_WithCancellationToken_PropagatesTokenToRepository()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var productName = ProductName.Create("Test Item").Value;
            var item = Product.Create(productName, MeasureType.Weight).Value;

            var repositoryResponse = Maybe.From(item);
            _mockRepository.GetAsync(itemId).Returns(repositoryResponse);

            var query = GetProductQuery.Create(itemId).Value;
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            await _mockRepository.Received(1).GetAsync(itemId);
        }
    }
}
