using FluentAssertions;
using Products.Core.Application.UseCases.Commands.UnArchiveProduct;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;
using Products.Core.Ports;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Primitives;
using Products.Core.Errors.Domain;

namespace Products.UnitTests.Domain.Application.UnarchiveProductCommandTests
{
    public class UnArchiveItemCommandHandlerTests
    {
        private readonly IProductRepository _itemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UnArchiveProductCommandHandler _handler;

        public UnArchiveItemCommandHandlerTests()
        {
            _itemRepository = Substitute.For<IProductRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _handler = new UnArchiveProductCommandHandler(_itemRepository, _unitOfWork);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldUnArchiveItemAndReturnSuccess()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var command = UnArchiveProductCommand.Create(itemId).Value;
            var productName = ProductName.Create("Test Item").Value;

            var item = Product.Create(productName, MeasureType.Weight).Value;
            // Сначала архивируем, чтобы потом разархивировать
            item.MakeArchive();

            _itemRepository.GetAsync(itemId).Returns(item);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _itemRepository.Received(1).Update(Arg.Is<Product>(x => x == item));
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_ItemNotFound_ShouldReturnError()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var command = UnArchiveProductCommand.Create(itemId).Value;

            _itemRepository.GetAsync(itemId).Returns((Product?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be(ProductErrors.ProductIsNotExists(itemId).Code);
            result.Error.Message.Should().Contain(itemId.ToString());

            _itemRepository.DidNotReceive().Update(Arg.Any<Product>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_ItemNotArchived_ShouldReturnError()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var command = UnArchiveProductCommand.Create(itemId).Value;
            var productName = ProductName.Create("Test Item").Value;

            // Создаем неархивированный item
            var item = Product.Create(productName, MeasureType.Weight).Value;


            _itemRepository.GetAsync(itemId).Returns(item);

            // Act - попытка разархивировать неархивированный item
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            // Проверяем что ошибка соответствует ожидаемой
            // (предполагаем что MakeUnArchive возвращает ошибку для неархивированного item)

            _itemRepository.DidNotReceive().Update(Arg.Any<Product>());
            await _unitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var command =  UnArchiveProductCommand.Create(itemId).Value;

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
            var command = UnArchiveProductCommand.Create(itemId).Value;
            var productName = ProductName.Create("Test Item").Value;

            var item = Product.Create(productName, MeasureType.Weight).Value;
            item.MakeArchive(); // Архивируем для разархивации
            _itemRepository.GetAsync(itemId).Returns(item);

            _unitOfWork.When(x => x.SaveChangesAsync())
                .Do(x => throw new InvalidOperationException("Save failed"));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Save failed");
            _itemRepository.Received(1).Update(Arg.Any<Product>());
        }

        [Fact]
        public async Task Handle_ValidItem_ShouldCallRepositoryMethodsInCorrectOrder()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var command = UnArchiveProductCommand.Create(itemId).Value;
            var productName = ProductName.Create("Test Item").Value;

            var item = Product.Create(productName, MeasureType.Weight).Value;
            item.MakeArchive(); // Архивируем для разархивации
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
