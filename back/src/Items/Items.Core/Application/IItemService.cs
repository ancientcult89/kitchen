using CSharpFunctionalExtensions;
using Items.Core.Domain.Model.ItemAgregate;
using Primitives;

namespace Items.Core.Application
{
    public interface IItemService
    {
        public Result<List<Item>, Error> Add(Item newItem, List<Item> sameMeasuringTypeItems);
    }
}
