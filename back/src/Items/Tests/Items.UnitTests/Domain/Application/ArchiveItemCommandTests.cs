using FluentAssertions;
using Items.Core.Application.UseCases.Commands.ArchiveItem;
using Items.Core.Domain.Model.ItemAgregate;
using Items.Core.Ports;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Primitives;

namespace Items.UnitTests.Domain.Application
{
    public class ArchiveItemCommandTests
    {
        private readonly IItemRepository _itemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ArchiveItemCommandHandler _handler;

        public ArchiveItemCommandTests()
        {
            _itemRepository = Substitute.For<IItemRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new ArchiveItemCommandHandler(_itemRepository, _unitOfWork);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldArchiveItemAndReturnSuccess()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var command = new ArchiveItemCommand(itemId);

            var item = Item.Create("Test Item", MeasureType.Weight).Value;
            _itemRepository.GetAsync(itemId).Returns(item);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _itemRepository.Received(1).Update(Arg.Is<Item>(x => x == item));
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_EmptyGuid_ShouldReturnError()
        {
            // Arrange
            var command = new ArchiveItemCommand(Guid.Empty);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("value.is.required");
            result.Error.Message.Should().Contain("ItemId");

            await _itemRepository.DidNotReceive().GetAsync(Arg.Any<Guid>());
            _itemRepository.DidNotReceive().Update(Arg.Any<Item>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_ItemNotFound_ShouldReturnError()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var command = new ArchiveItemCommand(itemId);

            _itemRepository.GetAsync(itemId).Returns((Item?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("no.such.item");
            result.Error.Message.Should().Contain(itemId.ToString());

            _itemRepository.DidNotReceive().Update(Arg.Any<Item>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var command = new ArchiveItemCommand(itemId);

            _itemRepository.GetAsync(itemId).ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database error");
        }

        [Fact]
        public async Task Handle_UnitOfWorkThrowsException_ShouldPropagateException()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var command = new ArchiveItemCommand(itemId);

            var item = Item.Create("Test Item", MeasureType.Weight).Value;
            _itemRepository.GetAsync(itemId).Returns(item);
            _unitOfWork.When(x => x.SaveChangesAsync())
                .Do(x => throw new InvalidOperationException("Save failed"));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Save failed");
            _itemRepository.Received(1).Update(Arg.Any<Item>());
        }

        [Fact]
        public async Task Handle_ValidItem_ShouldCallRepositoryMethodsInCorrectOrder()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var command = new ArchiveItemCommand(itemId);

            var item = Item.Create("Test Item", MeasureType.Weight).Value;
            _itemRepository.GetAsync(itemId).Returns(item);

            var callOrder = 0;
            _itemRepository.When(x => x.GetAsync(itemId)).Do(_ => callOrder++.Should().Be(0));
            _itemRepository.When(x => x.Update(item)).Do(_ => callOrder++.Should().Be(1));
            _unitOfWork.When(x => x.SaveChangesAsync()).Do(_ => callOrder++.Should().Be(2));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            callOrder.Should().Be(3);
        }
    }
}
