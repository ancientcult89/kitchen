using CSharpFunctionalExtensions;
using Items.Core.Domain.Model.ItemAgregate;
using Items.Core.Ports;
using MediatR;
using Primitives;

namespace Items.Core.Application.UseCases.Commands.AddItem
{
    public class AddItemCommandHandler : IRequestHandler<AddItemCommand, UnitResult<Error>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddItemCommandHandler(IItemRepository itemRepository, IUnitOfWork unitOfWork)
        {
            _itemRepository = itemRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<UnitResult<Error>> Handle(AddItemCommand request, CancellationToken cancellationToken)
        {
            var measureTypeResult = MeasureType.CreateFromId(request.MeasureType);
            if (measureTypeResult.IsFailure)
                return measureTypeResult.Error;

            MeasureType measureType = measureTypeResult.Value;

            var newItemResult = Item.Create(request.Name, measureType);

            if (newItemResult.IsFailure) 
                return newItemResult.Error;

            Item newItem = newItemResult.Value;

            var sameItemIsExists = _itemRepository.CheckDuplicate(request);

            if (sameItemIsExists)
                return Errors.ItemWithSameNameAndMeasureTypeExists(newItem.Name, newItem.MeasureType);

            await _itemRepository.AddAsync(newItem);
            await _unitOfWork.SaveChangesAsync();

            return UnitResult.Success<Error>();
        }

        public static class Errors
        {
            [Obsolete]
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
