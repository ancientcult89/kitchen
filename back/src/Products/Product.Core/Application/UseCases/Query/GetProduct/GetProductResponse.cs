using Products.Core.Application.Dto.ProductAggregate;

namespace Products.Core.Application.UseCases.Query.GetProduct
{
    public class GetProductResponse
    {
        public GetProductResponse(ProductDto product)
        {
            Product = product;
        }
        public ProductDto Product { get; set; } = new ProductDto();
    }
}
