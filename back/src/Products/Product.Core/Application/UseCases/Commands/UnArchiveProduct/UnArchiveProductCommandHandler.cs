using CSharpFunctionalExtensions;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Ports;
using MediatR;
using Primitives;
using Products.Core.Errors.Domain;

namespace Products.Core.Application.UseCases.Commands.UnArchiveProduct
{
    public class UnArchiveProductCommandHandler : IRequestHandler<UnArchiveProductCommand, UnitResult<Error>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UnArchiveProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UnitResult<Error>> Handle(UnArchiveProductCommand request, CancellationToken cancellationToken)
        {
            var unarchivedProductResult = await _productRepository.GetAsync(request.ProductId);

            if (unarchivedProductResult.HasNoValue)
                return ProductErrors.ProductIsNotExists(request.ProductId);

            Product unarchiverProduct = unarchivedProductResult.Value;

            var result = unarchiverProduct.MakeUnArchive();

            if (!result.IsSuccess)
                return result.Error;

            _productRepository.Update(unarchiverProduct);

            await _unitOfWork.SaveChangesAsync();

            return UnitResult.Success<Error>();
        }
    }
}
