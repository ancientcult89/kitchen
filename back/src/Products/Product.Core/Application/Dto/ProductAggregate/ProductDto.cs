namespace Products.Core.Application.Dto.ProductAggregate
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MeasureType { get; set; }
        public bool IsArchive { get; set; }
    }
}