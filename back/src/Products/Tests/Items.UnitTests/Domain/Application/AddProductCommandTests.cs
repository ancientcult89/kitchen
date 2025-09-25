using FluentAssertions;
using Products.Core.Application.UseCases.Commands.AddProduct;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;
using Products.Core.Ports;
using NSubstitute;
using Primitives;

namespace Products.UnitTests.Domain.Application
{
    public class AddItemCommandHandlerTests
    {
        private readonly IProductRepository _itemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AddProductCommandHandler _handler;

        public AddItemCommandHandlerTests()
        {
            _itemRepository = Substitute.For<IProductRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new AddProductCommandHandler(_itemRepository, _unitOfWork);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateItemAndReturnSuccess()
        {
            // Arrange
            var command = new AddProductCommand("Test Item", MeasureType.Weight.Id);

            _itemRepository.CheckDuplicate(Arg.Any<AddProductCommand>()).Returns(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            await _itemRepository.Received(1).AddAsync(Arg.Any<Product>());
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_DuplicateItemExists_ShouldReturnError()
        {
            // Arrange
            var command = new AddProductCommand("Test Item", MeasureType.Weight.Id);

            _itemRepository.CheckDuplicate(Arg.Any<AddProductCommand>()).Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("item.unique.violation");
            result.Error.Message.Should().Contain("Item with name 'Test Item'");

            await _itemRepository.DidNotReceive().AddAsync(Arg.Any<Product>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_InvalidMeasureType_ShouldReturnFailure()
        {
            // Arrange
            var command = new AddProductCommand("Test Item", -1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            await _itemRepository.DidNotReceive().AddAsync(Arg.Any<Product>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_ItemCreationFails_ShouldReturnError()
        {
            // Arrange
            var command = new AddProductCommand("", MeasureType.Weight.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            await _itemRepository.DidNotReceive().AddAsync(Arg.Any<Product>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new AddProductCommand("Test Item", MeasureType.Weight.Id);

            _itemRepository.CheckDuplicate(Arg.Any<AddProductCommand>()).Returns(false);
            _itemRepository.When(x => x.AddAsync(Arg.Any<Product>()))
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
            var error = AddProductCommandHandler.Errors.ItemWithSameNameAndMeasureTypeExists(name, measureType);

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
            var error = AddProductCommandHandler.Errors.ItemWithIdAlreadyExists(itemId);

            // Assert
            error.Should().NotBeNull();
            error.Code.Should().Be("item.id.already.exists");
            error.Message.Should().Be($"Item with ID {itemId} already exists in the collection");
        }
    }
}
