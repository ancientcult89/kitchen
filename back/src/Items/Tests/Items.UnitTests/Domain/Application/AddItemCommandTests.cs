using FluentAssertions;
using Items.Core.Application.UseCases.Commands.AddItem;
using Items.Core.Domain.Model.ItemAgregate;
using Items.Core.Ports;
using NSubstitute;
using Primitives;

namespace Items.UnitTests.Domain.Application
{
    public class AddItemCommandHandlerTests
    {
        private readonly IItemRepository _itemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AddItemCommandHandler _handler;

        public AddItemCommandHandlerTests()
        {
            _itemRepository = Substitute.For<IItemRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new AddItemCommandHandler(_itemRepository, _unitOfWork);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateItemAndReturnSuccess()
        {
            // Arrange
            var command = new AddItemCommand
            {
                Name = "Test Item",
                MeasureType = "weight"
            };

            _itemRepository.CheckDuplicate(Arg.Any<AddItemCommand>()).Returns(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await _itemRepository.Received(1).AddAsync(Arg.Any<Item>());
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_DuplicateItemExists_ShouldReturnError()
        {
            // Arrange
            var command = new AddItemCommand
            {
                Name = "Test Item",
                MeasureType = "weight"
            };

            _itemRepository.CheckDuplicate(Arg.Any<AddItemCommand>()).Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("item.unique.violation");
            result.Error.Message.Should().Contain("Item with name 'Test Item'");

            await _itemRepository.DidNotReceive().AddAsync(Arg.Any<Item>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("unknown")]
        [InlineData("")]
        [InlineData(null)]
        public async Task Handle_InvalidMeasureType_ShouldReturnFailure(string invalidMeasureType)
        {
            // Arrange
            var command = new AddItemCommand
            {
                Name = "Test Item",
                MeasureType = invalidMeasureType
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            await _itemRepository.DidNotReceive().AddAsync(Arg.Any<Item>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Theory]
        [InlineData("weight")]
        [InlineData("WEIGHT")]
        [InlineData("Weight")]
        [InlineData("liquid")]
        [InlineData("LIQUID")]
        [InlineData("Liquid")]
        public async Task Handle_ValidMeasureTypeCaseInsensitive_ShouldCreateItem(string validMeasureType)
        {
            // Arrange
            var command = new AddItemCommand
            {
                Name = "Test Item",
                MeasureType = validMeasureType
            };

            _itemRepository.CheckDuplicate(Arg.Any<AddItemCommand>()).Returns(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await _itemRepository.Received(1).AddAsync(Arg.Any<Item>());
        }

        [Fact]
        public async Task Handle_ItemCreationFails_ShouldReturnError()
        {
            // Arrange
            var command = new AddItemCommand
            {
                Name = "", // Invalid name that should cause Item.Create to fail
                MeasureType = "weight"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            await _itemRepository.DidNotReceive().AddAsync(Arg.Any<Item>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new AddItemCommand
            {
                Name = "Test Item",
                MeasureType = "weight"
            };

            _itemRepository.CheckDuplicate(Arg.Any<AddItemCommand>()).Returns(false);
            _itemRepository.When(x => x.AddAsync(Arg.Any<Item>()))
                .Do(x => throw new InvalidOperationException("Database error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database error");
        }

        [Fact]
        public void Errors_ItemWithSameNameAndMeasureTypeExists_ShouldReturnCorrectError()
        {
            // Arrange
            var name = "Test Item";
            var measureType = MeasureType.Weight;

            // Act
            var error = AddItemCommandHandler.Errors.ItemWithSameNameAndMeasureTypeExists(name, measureType);

            // Assert
            error.Should().NotBeNull();
            error.Code.Should().Be("item.unique.violation");
            error.Message.Should().Be($"Item with name '{name}' and measure type '{measureType}' already exists");
        }

        [Fact]
        public void Errors_ItemWithIdAlreadyExists_ShouldReturnCorrectError()
        {
            // Arrange
            var itemId = Guid.NewGuid();

            // Act
            var error = AddItemCommandHandler.Errors.ItemWithIdAlreadyExists(itemId);

            // Assert
            error.Should().NotBeNull();
            error.Code.Should().Be("item.id.already.exists");
            error.Message.Should().Be($"Item with ID {itemId} already exists in the collection");
        }
    }
}
