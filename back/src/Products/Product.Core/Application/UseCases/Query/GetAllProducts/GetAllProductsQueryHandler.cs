using CSharpFunctionalExtensions;
using Products.Core.Application.Dto.ProductAggregate;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Ports;
using MediatR;

namespace Products.Core.Application.UseCases.Query.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Maybe<GetAllProductsResponse>>
    {
        private readonly IProductRepository _itemRepository;
        public GetAllProductsQueryHandler(IProductRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task<Maybe<GetAllProductsResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var result = await _itemRepository.GetAllAsync();

            if (result.HasNoValue)
                return null;

            if (result.Value.Count == 0)
                return null;

            return new GetAllProductsResponse(MapProducts(result.Value));
        }

        private List<ProductDto> MapProducts(List<Product> result)
        {
            var products = new List<ProductDto>();
            foreach (var product in result)
            {
                var productDto = new ProductDto()
                {
                    Id = product.Id,
                    Name = product.Name,
                    IsArchive = product.IsArchive,
                    MeasureType = product.MeasureType.ToString(),
                };
                products.Add(productDto);
            }

            return products;
        }
    }
}
