using FluentAssertions;
using Items.Core.Application;
using Items.Core.Domain.Model.ItemAgregate;

namespace Items.UnitTests.Domain.Services
{
    public class ItemServiceShould
    {
        private readonly ItemService _itemService;
        private readonly List<Item> _items;
        private readonly MeasureType _weight;
        private readonly MeasureType _liquid;

        public ItemServiceShould()
        {
            _itemService = new ItemService();
            _items = new List<Item>();
            _weight = MeasureType.Weight;
            _liquid = MeasureType.Liquid;
        }

        [Fact]
        public void Add_WithValidNewItem_AddsItemAndReturnsSuccess()
        {
            // Arrange
            var newItem = Item.Create("Apple", _weight).Value;

            // Act
            var result = _itemService.Add(newItem, _items);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _items.Should().ContainSingle();
            _items.Should().Contain(newItem);
        }

        [Fact]
        public void Add_WithNullItem_ReturnsError()
        {
            // Act
            var result = _itemService.Add(null, _items);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            _items.Should().BeEmpty();
        }

        [Fact]
        public void Add_WithNullItemsCollection_ReturnsError()
        {
            // Arrange
            var newItem = Item.Create("Apple", _weight).Value;

            // Act
            var result = _itemService.Add(newItem, null);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void Add_WithDuplicateNameAndMeasureType_ReturnsError()
        {
            // Arrange
            var existingItem = Item.Create("Apple", _weight).Value;
            _items.Add(existingItem);

            var duplicateItem = Item.Create("Apple", _weight).Value;

            // Act
            var result = _itemService.Add(duplicateItem, _items);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("item.unique.violation");
            _items.Should().ContainSingle();
        }

        [Fact]
        public void Add_WithSameNameButDifferentMeasureType_AddsSuccessfully()
        {
            // Arrange
            var existingItem = Item.Create("Apple", _weight).Value;
            _items.Add(existingItem);

            var newItem = Item.Create("Apple", _liquid).Value;

            // Act
            var result = _itemService.Add(newItem, _items);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _items.Should().HaveCount(2);
        }

        [Fact]
        public void Add_WithSameMeasureTypeButDifferentName_AddsSuccessfully()
        {
            // Arrange
            var existingItem = Item.Create("Apple", _weight).Value;
            _items.Add(existingItem);

            var newItem = Item.Create("Orange", _weight).Value;

            // Act
            var result = _itemService.Add(newItem, _items);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _items.Should().HaveCount(2);
        }

        [Fact]
        public void Add_WithArchivedDuplicate_Failed()
        {
            // Arrange
            var archivedItem = Item.Create("Apple", _weight).Value;
            archivedItem.MakeArchive();
            _items.Add(archivedItem);

            var newItem = Item.Create("Apple", _weight).Value;

            // Act
            var result = _itemService.Add(newItem, _items);

            // Assert
            result.IsSuccess.Should().BeFalse();
            _items.Should().HaveCount(1);
        }

        [Fact]
        public void Add_WithCaseDifferentName_Failed()
        {
            // Arrange
            var existingItem = Item.Create("apple", _weight).Value;
            _items.Add(existingItem);

            var newItem = Item.Create("APPLE", _weight).Value;

            // Act
            var result = _itemService.Add(newItem, _items);

            // Assert
            result.IsSuccess.Should().BeFalse();
            _items.Should().HaveCount(1);
        }

        [Fact]
        public void Add_MultipleItems_AllAddedSuccessfully()
        {
            // Arrange
            var item1 = Item.Create("Apple", _weight).Value;
            var item2 = Item.Create("Orange", _weight).Value;
            var item3 = Item.Create("Milk", _liquid).Value;

            // Act
            var result1 = _itemService.Add(item1, _items);
            var result2 = _itemService.Add(item2, _items);
            var result3 = _itemService.Add(item3, _items);

            // Assert
            result1.IsSuccess.Should().BeTrue();
            result2.IsSuccess.Should().BeTrue();
            result3.IsSuccess.Should().BeTrue();
            _items.Should().HaveCount(3);
        }
    }
}
