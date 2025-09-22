using CSharpFunctionalExtensions;
using Items.Core.Application.Dto.ItemAggregate;
using Items.Core.Domain.Model.ItemAgregate;
using Items.Core.Ports;
using MediatR;

namespace Items.Core.Application.UseCases.Query.GetAllItems
{
    public class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, Maybe<GetAllItemsResponse>>
    {
        private readonly IItemRepository _itemRepository;
        public GetAllItemsQueryHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task<Maybe<GetAllItemsResponse>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
        {
            var result = await _itemRepository.GetAllAsync();

            if (result.HasNoValue)
                return null;

            if (result.Value.Count == 0)
                return null;

            return new GetAllItemsResponse(MapItems(result.Value));
        }

        private List<ItemDto> MapItems(List<Item> result)
        {
            var items = new List<ItemDto>();
            foreach (var item in result)
            {
                var itemDto = new ItemDto()
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsArchive = item.IsArchive,
                    MeasureType = item.MeasureType.ToString(),
                };
                items.Add(itemDto);
            }

            return items;
        }
    }
}
