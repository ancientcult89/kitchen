using CSharpFunctionalExtensions;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;
using Products.Core.Ports;
using MediatR;
using Primitives;

namespace Products.Core.Application.UseCases.Commands.AddProduct
{
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, UnitResult<Error>>
    {
        private readonly IProductRepository _itemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddProductCommandHandler(IProductRepository itemRepository, IUnitOfWork unitOfWork)
        {
            _itemRepository = itemRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<UnitResult<Error>> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var measureTypeResult = MeasureType.CreateFromId(request.MeasureType);
            if (measureTypeResult.IsFailure)
                return measureTypeResult.Error;

            MeasureType measureType = measureTypeResult.Value;

            var newItemResult = Product.Create(request.Name, measureType);

            if (newItemResult.IsFailure)
                return newItemResult.Error;

            Product newItem = newItemResult.Value;

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
