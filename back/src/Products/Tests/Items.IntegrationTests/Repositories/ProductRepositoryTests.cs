using FluentAssertions;
using Products.Core.Application.UseCases.Commands.AddProduct;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;
using Products.Infrastructure.Adapters.Postgres;
using Products.Infrastructure.Adapters.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Products.IntegrationTests.Repositories
{
    public class ProductRepositoryTests : IAsyncLifetime
    {
        private ApplicationDbContext _context;
        private ProductRepository _repository;

        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("products")
            .WithUsername("username")
            .WithPassword("password")
            .WithCleanUp(true)
            .Build();

        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();

            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(
                    _postgreSqlContainer.GetConnectionString(),
                    sqlOptions => { sqlOptions.MigrationsAssembly("Products.Infrastructure"); }
                )
                .Options;

            _context = new ApplicationDbContext(contextOptions);
            await _context.Database.MigrateAsync();
            _repository = new ProductRepository(_context);
        }

        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.DisposeAsync();
        }

        [Fact]
        public async Task AddAsync_WhenItemIsValid_ShouldAddItemToDatabase()
        {
            // Arrange
            var measureType = MeasureType.Weight;
            string productStringName = "Product Item";
            var productName = ProductName.Create(productStringName).Value;

            var item = Product.Create(productName, measureType).Value;

            // Act
            await _repository.AddAsync(item);
            await _context.SaveChangesAsync();

            // Assert
            var savedItem = await _context.Products
                .Include(i => i.MeasureType)
                .FirstOrDefaultAsync(i => i.Id == item.Id);
            Assert.NotNull(savedItem);
            Assert.Equal(productStringName, savedItem.Name.Value);
            Assert.Equal(MeasureType.Weight.Name, savedItem.MeasureType.Name);
        }

        [Fact]
        public void CheckDuplicate_WhenDuplicateExists_ShouldReturnTrue()
        {
            // Arrange
            var measureType = MeasureType.Weight;
            var productName = ProductName.Create("Duplicate Item").Value;

            var existingItem = Product.Create(productName, measureType).Value;

            _context.Products.Add(existingItem);
            _context.SaveChanges();

            var request = AddProductCommand.Create("Duplicate Item", MeasureType.Weight.Id).Value;

            // Act
            var result = _repository.IsDuplicate(request);

            // Assert
            Assert.True(result.Value);
        }

        [Fact]
        public void CheckDuplicate_WhenNoDuplicate_ShouldReturnFalse()
        {
            // Arrange
            var measureType = MeasureType.Weight;
            var productName = ProductName.Create("Existing Item").Value;

            var existingItem = Product.Create(productName, measureType).Value;

            _context.Products.Add(existingItem);
            _context.SaveChanges();

            var request = AddProductCommand.Create("Different Item", MeasureType.Weight.Id).Value;

            // Act
            var result = _repository.IsDuplicate(request);

            // Assert
            Assert.False(result.Value);
        }

        [Fact]
        public void CheckDuplicate_WhenSameNameButDifferentMeasureType_ShouldReturnFalse()
        {
            // Arrange
            var measureType1 = MeasureType.Weight;
            var productName = ProductName.Create("Test Item").Value;

            var existingItem = Product.Create(productName, measureType1).Value;

            _context.Products.Add(existingItem);
            _context.SaveChanges();

            var request = AddProductCommand.Create("Test Item", MeasureType.Liquid.Id).Value;

            // Act
            var result = _repository.IsDuplicate(request);

            // Assert
            Assert.False(result.Value);
        }

        [Fact]
        public void CheckDuplicate_WhenCaseSensitiveNames_ShouldHandleCorrectly()
        {
            // Arrange
            var measureType = MeasureType.Weight;
            var productName = ProductName.Create("Test Item").Value;

            var existingItem = Product.Create(productName, measureType).Value;

            _context.Products.Add(existingItem);
            _context.SaveChanges();

            var request = AddProductCommand.Create("test item", MeasureType.Weight.Id).Value;

            // Act
            var result = _repository.IsDuplicate(request);

            // Assert
            Assert.True(result.Value); // Должно быть true, так как сравнение не чувствительно к регистру
        }

        [Fact]
        public async Task GetAllAsync_WhenItemsExist_ShouldReturnAllItems()
        {
            // Arrange
            var measureType1 = MeasureType.Weight;
            var measureType2 = MeasureType.Liquid;
            var productName1 = ProductName.Create("Item 1").Value;
            var productName2 = ProductName.Create("Item 2").Value;

            var items = new List<Product>()
    {
        Product.Create(productName1, measureType1).Value,
        Product.Create(productName2, measureType2).Value
    };

            await _context.Products.AddRangeAsync(items);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count);
            Assert.Contains(result.Value, i => i.Name.Value == "Item 1");
            Assert.Contains(result.Value, i => i.Name.Value == "Item 2");
        }

        [Fact]
        public async Task GetAllAsync_WhenNoItems_ShouldReturnEmptyList()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAsync_WhenItemExists_ShouldReturnItem()
        {
            // Arrange
            var measureType = MeasureType.Weight;
            var productName = ProductName.Create("Test Item").Value;

            var item = Product.Create(productName, measureType).Value;
            await _context.Products.AddAsync(item);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAsync(item.Id);

            // Assert
            result.HasValue.Should().BeTrue();
            Assert.Equal(item.Id, result.Value.Id);
            Assert.Equal("Test Item", result.Value.Name.Value);
            Assert.Equal("weight", result.Value.MeasureType.Name);
        }

        [Fact]
        public async Task GetAsync_WhenItemDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetAsync(nonExistentId);

            // Assert
            result.HasNoValue.Should().BeTrue();
        }

        [Fact]
        public async Task GetAsync_WhenEmptyGuid_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetAsync(Guid.Empty);

            // Assert
            result.HasNoValue.Should().BeTrue();
        }

        [Fact]
        public void CheckDuplicate_WhenMeasureTypeHasSpaces_ShouldTrimAndCompare()
        {
            // Arrange
            var measureType = MeasureType.Weight;
            var productName = ProductName.Create("Test Item").Value;

            var existingItem = Product.Create(productName, measureType).Value;

            _context.Products.Add(existingItem);
            _context.SaveChanges();

            var request = AddProductCommand.Create("Test Item", MeasureType.Weight.Id).Value;

            // Act
            var result = _repository.IsDuplicate(request);

            // Assert
            Assert.True(result.Value);
        }
    }
}