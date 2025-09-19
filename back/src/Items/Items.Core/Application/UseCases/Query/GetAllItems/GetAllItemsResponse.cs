using Items.Core.Application.Dto.ItemAggregate;

namespace Items.Core.Application.UseCases.Query.GetAllItems
{
    public class GetAllItemsResponse
    {
        private GetAllItemsResponse() { }
        public GetAllItemsResponse(List<ItemDto> items)
        {
            Items.AddRange(items);
        }

        public List<ItemDto> Items { get; set; } = new List<ItemDto>();
    }
}
