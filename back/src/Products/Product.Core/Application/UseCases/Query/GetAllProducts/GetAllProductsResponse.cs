using Products.Core.Application.Dto.ProductAggregate;

namespace Products.Core.Application.UseCases.Query.GetAllProducts
{
    public class GetAllProductsResponse
    {
        private GetAllProductsResponse() { }
        public GetAllProductsResponse(List<ProductDto> items)
        {
            Products.AddRange(items);
        }

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
