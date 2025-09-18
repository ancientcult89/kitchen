using CSharpFunctionalExtensions;
using Items.Core.Domain.Model.ItemAgregate;
using Primitives;

namespace Items.Core.Application
{
    public class ItemService : IItemService
    {
        public Result<List<Item>, Error> Add(Item newItem, List<Item> allItems)
        {
            if (newItem == null)
                return GeneralErrors.ValueIsRequired(nameof(newItem));

            if (allItems == null)
                return GeneralErrors.ValueIsRequired(nameof(allItems));

            if (allItems.Any(item => item.Id == newItem.Id))
                return Errors.ItemWithIdAlreadyExists(newItem.Id);

            if (allItems.Any(item =>
                item.Name.Equals(newItem.Name, StringComparison.OrdinalIgnoreCase) &&
                item.MeasureType == newItem.MeasureType))
            {
                return Errors.ItemWithSameNameAndMeasureTypeExists(newItem.Name, newItem.MeasureType);
            }

            allItems.Add(newItem);
            return allItems;
        }

        public static class Errors
        {
            public static Error ItemWithIdAlreadyExists(Guid itemId)
            {
                return new Error("item.id.already.exists", $"Item with ID {itemId} already exists in the collection");
            }

            public static Error ItemWithSameNameAndMeasureTypeExists(string name, MeasureType measureType)
            {
                return new Error("item.unique.violation", $"Item with name '{name}' and measure type '{measureType}' already exists");
            }
        }
    }
}
