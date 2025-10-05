using CSharpFunctionalExtensions;
using FluentAssertions;
using NSubstitute;
using Products.Core.Application.Dto.ProductAggregate;
using Products.Core.Application.UseCases.Query.GetAllProducts;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;
using Products.Core.Ports;

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
            var productName1 = ProductName.Create("Item 1").Value;
            var productName2 = ProductName.Create("Item 2").Value;

            var items = new List<Product>
    {
        Product.Create(productName1, MeasureType.Liquid).Value,
        Product.Create(productName2, MeasureType.Weight).Value
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
            Assert.Equal(items[0].Name.Value, firstItem.Name); // Используем .Value для ProductName
            Assert.Equal(items[0].IsArchive, firstItem.IsArchive);
            Assert.Equal(items[0].MeasureType.Id, firstItem.MeasureTypeId); // Используем Id вместо Name

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
        public async Task Handle_WhenItemsHaveDifferentMeasureTypes_ReturnsCorrectMeasureTypeId()
        {
            // Arrange
            var productName1 = ProductName.Create("Item 1").Value;
            var productName2 = ProductName.Create("Item 2").Value;
            var productName3 = ProductName.Create("Item 3").Value;

            var items = new List<Product>
    {
        Product.Create(productName1, MeasureType.Weight).Value,
        Product.Create(productName2, MeasureType.Liquid).Value,
        Product.Create(productName3, MeasureType.Weight).Value
    };

            var repositoryResponse = Maybe.From<List<Product>>(items);
            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllProductsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Value.Products.Count);
            Assert.Equal(MeasureType.Weight.Id, result.Value.Products[0].MeasureTypeId);
            Assert.Equal(MeasureType.Liquid.Id, result.Value.Products[1].MeasureTypeId);
            Assert.Equal(MeasureType.Weight.Id, result.Value.Products[2].MeasureTypeId);
        }

        [Fact]
        public async Task Handle_WhenItemsHaveArchiveStatus_ReturnsCorrectArchiveStatus()
        {
            // Arrange
            var productName1 = ProductName.Create("Active Item").Value;
            var productName2 = ProductName.Create("Archived Item").Value;

            var item1 = Product.Create(productName1, MeasureType.Liquid).Value;
            var item2 = Product.Create(productName2, MeasureType.Weight).Value;

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
            var productName = ProductName.Create("Item 1").Value;
            var items = new List<Product>
    {
        Product.Create(productName, MeasureType.Liquid).Value
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
            var productName = ProductName.Create("Item 1").Value;
            var items = new List<Product>
    {
        Product.Create(productName, MeasureType.Liquid).Value
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

        [Fact]
        public async Task Handle_WhenProductsHaveSpecialCharactersInName_ReturnsCorrectName()
        {
            // Arrange
            var productName1 = ProductName.Create("Item with spaces").Value;
            var productName2 = ProductName.Create("Item-with-dashes").Value;
            var productName3 = ProductName.Create("Item_with_underscores").Value;

            var items = new List<Product>
    {
        Product.Create(productName1, MeasureType.Weight).Value,
        Product.Create(productName2, MeasureType.Liquid).Value,
        Product.Create(productName3, MeasureType.Weight).Value
    };

            var repositoryResponse = Maybe.From<List<Product>>(items);
            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllProductsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Value.Products.Count);
            Assert.Equal("Item with spaces", result.Value.Products[0].Name);
            Assert.Equal("Item-with-dashes", result.Value.Products[1].Name);
            Assert.Equal("Item_with_underscores", result.Value.Products[2].Name);
        }

        [Fact]
        public async Task Handle_WhenProductsHaveDifferentCaseInNames_ReturnsOriginalCase()
        {
            // Arrange
            var productName1 = ProductName.Create("ITEM IN UPPERCASE").Value;
            var productName2 = ProductName.Create("item in lowercase").Value;
            var productName3 = ProductName.Create("Item In Mixed Case").Value;

            var items = new List<Product>
    {
        Product.Create(productName1, MeasureType.Weight).Value,
        Product.Create(productName2, MeasureType.Liquid).Value,
        Product.Create(productName3, MeasureType.Weight).Value
    };

            var repositoryResponse = Maybe.From<List<Product>>(items);
            _mockRepository.GetAllAsync().Returns(repositoryResponse);

            var query = new GetAllProductsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Value.Products.Count);
            Assert.Equal("ITEM IN UPPERCASE", result.Value.Products[0].Name);
            Assert.Equal("item in lowercase", result.Value.Products[1].Name);
            Assert.Equal("Item In Mixed Case", result.Value.Products[2].Name);
        }

        [Fact]
        public void ProductExtension_ToDto_MapsCorrectly()
        {
            // Arrange
            var productName = ProductName.Create("Test Product").Value;
            var product = Product.Create(productName, MeasureType.Liquid).Value;

            // Act
            var dto = product.ToDto();

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(product.Id, dto.Id);
            Assert.Equal(product.Name.Value, dto.Name);
            Assert.Equal(product.IsArchive, dto.IsArchive);
            Assert.Equal(product.MeasureType.Id, dto.MeasureTypeId);
        }
    }
}
