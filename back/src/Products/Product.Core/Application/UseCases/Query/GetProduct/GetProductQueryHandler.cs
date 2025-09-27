using CSharpFunctionalExtensions;
using MediatR;
using Products.Core.Application.Dto.ProductAggregate;
using Products.Core.Domain.Model.ProductAggregate;
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
            var getProductResult = await _productRepository.GetAsync(request.ProductId);

            if (getProductResult.HasNoValue)
                return null;

            var product = getProductResult.Value;

            return new GetProductResponse(MapProduct(product));
        }

        private ProductDto MapProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return new ProductDto()
            {
                Id = product.Id,
                IsArchive = product.IsArchive,
                MeasureType = product.MeasureType.ToString(),
                Name = product.Name,
            };
        }
    }
}
