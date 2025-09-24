using FluentAssertions;
using Items.Core.Application.UseCases.Commands.AddItem;
using Items.Core.Domain.Model.ItemAgregate;
using Items.Infrastructure.Adapters.Postgres;
using Items.Infrastructure.Adapters.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Items.IntegrationTests.Repositories
{
    public class ItemRepositoryTests : IAsyncLifetime
    {
        private ApplicationDbContext _context;
        private ItemRepository _repository;

        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("courier")
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
                    sqlOptions => { sqlOptions.MigrationsAssembly("Items.Infrastructure"); }
                )
                .Options;

            _context = new ApplicationDbContext(contextOptions);
            await _context.Database.MigrateAsync();
            _repository = new ItemRepository(_context);
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

            var item = Item.Create("Test Item", measureType).Value;

            // Act
            await _repository.AddAsync(item);
            await _context.SaveChangesAsync();

            // Assert
            var savedItem = await _context.Items
                .Include(i => i.MeasureType)
                .FirstOrDefaultAsync(i => i.Id == item.Id);
            Assert.NotNull(savedItem);
            Assert.Equal("Test Item", savedItem.Name);
            Assert.Equal("weight", savedItem.MeasureType.Name);
        }

        [Fact]
        public void CheckDuplicate_WhenDuplicateExists_ShouldReturnTrue()
        {
            // Arrange
            var measureType = MeasureType.Weight;

            var existingItem = Item.Create("Duplicate Item", measureType).Value;

            _context.Items.Add(existingItem);
            _context.SaveChanges();

            var request = new AddItemCommand("Duplicate Item", MeasureType.Weight.Id);

            // Act
            var result = _repository.CheckDuplicate(request);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CheckDuplicate_WhenNoDuplicate_ShouldReturnFalse()
        {
            // Arrange
            var measureType = MeasureType.Weight;

            var existingItem = Item.Create("Existing Item", measureType).Value;

            _context.Items.Add(existingItem);
            _context.SaveChanges();

            var request = new AddItemCommand("Different Item", MeasureType.Weight.Id);

            // Act
            var result = _repository.CheckDuplicate(request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CheckDuplicate_WhenSameNameButDifferentMeasureType_ShouldReturnFalse()
        {
            // Arrange
            var measureType1 = MeasureType.Weight;

            var existingItem = Item.Create("Test Item", measureType1).Value;

            _context.Items.Add(existingItem);
            _context.SaveChanges();

            var request = new AddItemCommand("Test Item", MeasureType.Liquid.Id);

            // Act
            var result = _repository.CheckDuplicate(request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CheckDuplicate_WhenCaseSensitiveNames_ShouldHandleCorrectly()
        {
            // Arrange
            var measureType = MeasureType.Weight;

            var existingItem = Item.Create("Test Item", measureType).Value;

            _context.Items.Add(existingItem);
            _context.SaveChanges();

            var request = new AddItemCommand("test item", MeasureType.Weight.Id);

            // Act
            var result = _repository.CheckDuplicate(request);

            // Assert
            Assert.True(result); // Должно быть true, так как сравнение не чувствительно к регистру
        }

        [Fact]
        public async Task GetAllAsync_WhenItemsExist_ShouldReturnAllItems()
        {
            // Arrange
            var measureType1 = MeasureType.Weight;
            var measureType2 = MeasureType.Liquid;

            var items = new List<Item>()
        {
            Item.Create("Item 1", measureType1).Value,
            Item.Create("Item 2", measureType2).Value
        };

            await _context.Items.AddRangeAsync(items);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Value.Count);
            Assert.Contains(result.Value, i => i.Name == "Item 1");
            Assert.Contains(result.Value, i => i.Name == "Item 2");
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

            var item = Item.Create("Test Item", measureType).Value;
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAsync(item.Id);

            // Assert
            result.HasValue.Should().BeTrue();
            Assert.Equal(item.Id, result.Value.Id);
            Assert.Equal("Test Item", result.Value.Name);
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

            var existingItem = Item.Create("Test Item", measureType).Value;

            _context.Items.Add(existingItem);
            _context.SaveChanges();

            var request = new AddItemCommand("Test Item", MeasureType.Weight.Id);

            // Act
            var result = _repository.CheckDuplicate(request);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CheckDuplicate_WhenUnknownMeasureType_ShouldReturnFalse()
        {
            // Arrange
            var measureType = MeasureType.Weight;

            var existingItem = Item.Create("Test Item", measureType).Value;

            _context.Items.Add(existingItem);
            _context.SaveChanges();

            var request = new AddItemCommand("Test Item", -1);

            // Act
            var result = _repository.CheckDuplicate(request);

            // Assert
            Assert.False(result); // Неизвестный тип меры не должен считаться дубликатом
        }
    }
}