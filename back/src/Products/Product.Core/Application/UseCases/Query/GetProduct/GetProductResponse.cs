using Products.Core.Application.Dto.ProductAggregate;

namespace Products.Core.Application.UseCases.Query.GetProduct
{
    public class GetProductResponse
    {
        public GetProductResponse(ProductDto item)
        {
            Product = item;
        }
        public ProductDto Product { get; set; } = new ProductDto();
    }
}
