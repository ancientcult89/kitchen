using Products.Core.Domain.Model.ProductAggregate;

namespace Products.Core.Application.Dto.ProductAggregate
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int MeasureTypeId { get; set; }
        public bool IsArchive { get; set; }
    }

    public static class ProductExtension
    {
        public static ProductDto ToDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name.Value,
                IsArchive = product.IsArchive,
                MeasureTypeId = product.MeasureType.Id,
            };
        }
    }
}