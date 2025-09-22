using CSharpFunctionalExtensions;
using FluentAssertions;
using Items.Core.Application.Dto.ItemAggregate;
using Items.Core.Application.UseCases.Query.GetAllItems;
using Items.Core.Domain.Model.ItemAgregate;
using Items.Core.Ports;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using NSubstitute;

namespace Items.UnitTests.Domain.Application
{
    public class GetAllItemsQueryTests
    {
        private readonly IItemRepository _mockRepository;
        private readonly GetAllItemsQueryHandler _handler;

        public GetAllItemsQueryTests()
        {
            _mockRepository = Substitute.For<IItemRepository>();
            _handler = new GetAllItemsQueryHandler(_mockRepository);
        }

        [Fact]
        public async Task Handle_WhenItemsExist_ReturnsGetAllItemsResponse()
        {
            // Arrange
            var items = new List<Item>
            {
                Item.Create("Item 1", MeasureType.Liquid).Value,
                Item.Create("Item 2", MeasureType.Weight).Value
            };

            var repositoryResponse = Maybe.From<List<Item>>(items);
            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllItemsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.HasValue);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Items.Count);

            var firstItem = result.Value.Items[0];
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
            var emptyItemsList = new List<Item>();
            var repositoryResponse = Maybe.From<List<Item>>(emptyItemsList);

            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllItemsQuery();

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
            var repositoryResponse = Maybe<List<Item>>.None;

            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllItemsQuery();

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
            _mockRepository.GetAllAsync().Returns<Task<Maybe<List<Item>>>>(x =>
                throw new InvalidOperationException("Database error"));

            var query = new GetAllItemsQuery();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public void MapItems_CorrectlyMapsItemToItemDto()
        {
            // Arrange
            var items = new List<Item>
            {
                Item.Create("Test Item", MeasureType.Liquid).Value
            };

            // Используем reflection для вызова private метода
            var handlerType = _handler.GetType();
            var mapItemsMethod = handlerType.GetMethod("MapItems",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = (List<ItemDto>)mapItemsMethod.Invoke(_handler, new object[] { items });

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
            var items = new List<Item>
        {
            Item.Create("Item 1", MeasureType.Weight).Value,
            Item.Create("Item 2", MeasureType.Liquid).Value,
            Item.Create("Item 3", MeasureType.Weight).Value
        };

            var repositoryResponse = Maybe.From<List<Item>>(items);
            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllItemsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Value.Items.Count);
            Assert.Equal(MeasureType.Weight.ToString(), result.Value.Items[0].MeasureType);
            Assert.Equal(MeasureType.Liquid.ToString(), result.Value.Items[1].MeasureType);
            Assert.Equal(MeasureType.Weight.ToString(), result.Value.Items[2].MeasureType);
        }

        [Fact]
        public async Task Handle_WhenItemsHaveArchiveStatus_ReturnsCorrectArchiveStatus()
        {
            // Arrange
            var item1 = Item.Create("Active Item", MeasureType.Liquid).Value;
            var item2 = Item.Create("Archived Item", MeasureType.Weight).Value;

            // Архивируем второй элемент
            item2.MakeArchive();

            var items = new List<Item> { item1, item2 };
            var repositoryResponse = Maybe.From<List<Item>>(items);

            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllItemsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Items.Count);
            Assert.False(result.Value.Items[0].IsArchive); // Первый элемент не архивирован
            Assert.True(result.Value.Items[1].IsArchive);  // Второй элемент архивирован
        }

        [Fact]
        public async Task Handle_WhenCalled_CallsRepositoryOnce()
        {
            // Arrange
            var items = new List<Item>
        {
            Item.Create("Item 1", MeasureType.Liquid).Value
        };

            var repositoryResponse = Maybe.From<List<Item>>(items);
            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllItemsQuery();

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            await _mockRepository.Received(1).GetAllAsync();
        }

        [Fact]
        public async Task Handle_WithCancellationToken_PropagatesTokenToRepository()
        {
            // Arrange
            var items = new List<Item>
        {
            Item.Create("Item 1", MeasureType.Liquid).Value
        };

            var repositoryResponse = Maybe.From<List<Item>>(items);
            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllItemsQuery();
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            await _mockRepository.Received(1).GetAllAsync();
        }
    }
}
