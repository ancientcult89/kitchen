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
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<UnitResult<Error>> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var measureTypeResult = MeasureType.CreateFromId(request.MeasureTypeId);
            if (measureTypeResult.IsFailure)
                return measureTypeResult.Error;

            MeasureType measureType = measureTypeResult.Value;

            var newProductResult = Product.Create(request.Name, measureType);

            if (newProductResult.IsFailure)
                return newProductResult.Error;

            Product newProduct = newProductResult.Value;

            var sameProductIsExists = _productRepository.CheckDuplicate(request);

            if (sameProductIsExists)
                return Errors.ItemWithSameNameAndMeasureTypeExists(newProduct.Name, newProduct.MeasureType);

            await _productRepository.AddAsync(newProduct);
            await _unitOfWork.SaveChangesAsync();

            return UnitResult.Success<Error>();
        }

        public static class Errors
        {
            [Obsolete]
            public static Error ItemWithIdAlreadyExists(Guid itemId)
            {
                return new Error("product.id.already.exists", $"Product with ID {itemId} already exists in the collection");
            }

            public static Error ItemWithSameNameAndMeasureTypeExists(string name, MeasureType measureType)
            {
                return new Error("product.unique.violation", $"Product with name '{name}' and measure type '{measureType}' already exists");
            }
        }
    }
}
