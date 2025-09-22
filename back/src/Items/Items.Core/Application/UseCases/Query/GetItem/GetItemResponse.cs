using Items.Core.Application.Dto.ItemAggregate;

namespace Items.Core.Application.UseCases.Query.GetItem
{
    public class GetItemResponse
    {
        public GetItemResponse(ItemDto item)
        {
            Item = item;
        }
        public ItemDto Item { get; set; } = new ItemDto();
    }
}
