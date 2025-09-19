namespace Items.Core.Application.Dto.ItemAggregate
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MeasureType { get; set; }
        public bool IsArchive { get; set; }
    }
}