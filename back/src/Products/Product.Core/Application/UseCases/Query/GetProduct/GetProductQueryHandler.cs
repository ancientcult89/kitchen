using CSharpFunctionalExtensions;
using MediatR;
using Primitives;
using Products.Core.Application.Dto.ProductAggregate;
using Products.Core.Ports;

namespace Products.Core.Application.UseCases.Query.GetProduct
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Maybe<GetProductResponse>>
    {
        private readonly IProductRepository _productRepository;
        public GetProductQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<Maybe<GetProductResponse>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            if(request.ProductId == Guid.Empty)
                return null;

            var getProductResult = await _productRepository.GetAsync(request.ProductId);

            if (getProductResult.HasNoValue)
                return null;

            var product = getProductResult.Value;

            return new GetProductResponse(product.ToDto());
        }
    }
}
