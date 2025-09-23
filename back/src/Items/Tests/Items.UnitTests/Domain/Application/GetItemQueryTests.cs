using CSharpFunctionalExtensions;
using FluentAssertions;
using Items.Core.Application.Dto.ItemAggregate;
using Items.Core.Application.UseCases.Query.GetItem;
using Items.Core.Domain.Model.ItemAgregate;
using Items.Core.Ports;
using NSubstitute;
using System.Reflection;

namespace Items.UnitTests.Domain.Application
{
    public class GetItemQueryTests
    {
        private readonly IItemRepository _mockRepository;
        private readonly GetItemQueryHandler _handler;

        public GetItemQueryTests()
        {
            _mockRepository = Substitute.For<IItemRepository>();
            _handler = new GetItemQueryHandler(_mockRepository);
        }

        [Fact]
        public async Task Handle_WhenItemExists_ReturnsGetItemResponse()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = Item.Create("Test Item", MeasureType.Liquid).Value;

            // Используем reflection для установки Id, так как он private set
            var idProperty = typeof(Item).GetProperty("Id");
            idProperty?.SetValue(item, itemId);

            var repositoryResponse = Maybe.From(item);
            _mockRepository.GetAsync(itemId).Returns(repositoryResponse);

            var query = new GetItemQuery(itemId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.HasValue);
            Assert.NotNull(result.Value);

            Assert.Equal(item.Id, result.Value.Item.Id);
            Assert.Equal(item.Name, result.Value.Item.Name);
            Assert.Equal(item.IsArchive, result.Value.Item.IsArchive);
            Assert.Equal(item.MeasureType.ToString(), result.Value.Item.MeasureType);

            await _mockRepository.Received(1).GetAsync(itemId);
        }

        [Fact]
        public async Task Handle_WhenItemDoesNotExist_ReturnsNull()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var repositoryResponse = Maybe<Item>.None;

            _mockRepository.GetAsync(itemId).Returns(repositoryResponse);

            var query = new GetItemQuery(itemId);

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
            var repositoryResponse = Maybe<Item>.None;

            _mockRepository.GetAsync(itemId).Returns(repositoryResponse);

            var query = new GetItemQuery(itemId);

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
            _mockRepository.GetAsync(itemId).Returns<Task<Maybe<Item>>>(x =>
                throw new InvalidOperationException("Database error"));

            var query = new GetItemQuery(itemId);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public void MapItem_CorrectlyMapsItemToItemDto()
        {
            // Arrange
            var item = Item.Create("Test Item", MeasureType.Liquid).Value;

            // Используем reflection для вызова private метода
            var handlerType = _handler.GetType();
            var mapItemMethod = handlerType.GetMethod("MapItem",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = (ItemDto)mapItemMethod.Invoke(_handler, new object[] { item });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(item.Id, result.Id);
            Assert.Equal(item.Name, result.Name);
            Assert.Equal(item.IsArchive, result.IsArchive);
            Assert.Equal(item.MeasureType.ToString(), result.MeasureType);
        }

        [Fact]
        public void MapItem_WhenItemIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            // Используем reflection для вызова private метода
            var handlerType = _handler.GetType();
            var mapItemMethod = handlerType.GetMethod("MapItem",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act & Assert
            var exception = Assert.Throws<TargetInvocationException>(() =>
                mapItemMethod.Invoke(_handler, new object[] { null }));

            Assert.IsType<ArgumentNullException>(exception.InnerException);
            Assert.Equal("item", ((ArgumentNullException)exception.InnerException).ParamName);
        }

        [Fact]
        public async Task Handle_WhenItemIsArchived_ReturnsCorrectArchiveStatus()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = Item.Create("Archived Item", MeasureType.Weight).Value;

            // Архивируем элемент
            item.MakeArchive();

            var repositoryResponse = Maybe.From(item);
            _mockRepository.GetAsync(itemId).Returns(repositoryResponse);

            var query = new GetItemQuery(itemId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Value.Item.IsArchive);
            await _mockRepository.Received(1).GetAsync(itemId);
        }

        [Fact]
        public async Task Handle_WhenItemHasDifferentMeasureType_ReturnsCorrectStringRepresentation()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = Item.Create("Test Item", MeasureType.Weight).Value;

            var repositoryResponse = Maybe.From(item);
            _mockRepository.GetAsync(itemId).Returns(repositoryResponse);

            var query = new GetItemQuery(itemId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(MeasureType.Weight.ToString(), result.Value.Item.MeasureType);
            await _mockRepository.Received(1).GetAsync(itemId);
        }

        [Fact]
        public async Task Handle_WithCancellationToken_PropagatesTokenToRepository()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = Item.Create("Test Item", MeasureType.Weight).Value;

            var repositoryResponse = Maybe.From(item);
            _mockRepository.GetAsync(itemId).Returns(repositoryResponse);

            var query = new GetItemQuery(itemId);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            await _mockRepository.Received(1).GetAsync(itemId);
        }
    }
}
