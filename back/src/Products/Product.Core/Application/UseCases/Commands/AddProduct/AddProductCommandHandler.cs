using CSharpFunctionalExtensions;
using MediatR;
using Primitives;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Errors.Application;
using Products.Core.Ports;

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
            if (request == null)
                return CQSErrors.IncorrectCommand();

            var newProductResult = Product.Create(request.Name, request.MeasureType);

            if (newProductResult.IsFailure)
                return newProductResult.Error;

            Product newProduct = newProductResult.Value;

            var sameProductIsExists = _productRepository.IsDuplicate(request);

            if (!sameProductIsExists.IsSuccess)
                return sameProductIsExists.Error;

            if (sameProductIsExists.Value)
                return AddProductErrors.ItemWithSameNameAndMeasureTypeExists(newProduct.Name.Value, newProduct.MeasureType);

            await _productRepository.AddAsync(newProduct);
            await _unitOfWork.SaveChangesAsync();

            return UnitResult.Success<Error>();
        }
    }
}
