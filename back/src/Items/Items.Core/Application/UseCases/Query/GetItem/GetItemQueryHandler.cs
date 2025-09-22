using CSharpFunctionalExtensions;
using Items.Core.Application.Dto.ItemAggregate;
using Items.Core.Domain.Model.ItemAgregate;
using Items.Core.Ports;
using MediatR;

namespace Items.Core.Application.UseCases.Query.GetItem
{
    public class GetItemQueryHandler : IRequestHandler<GetItemQuery, Maybe<GetItemResponse>>
    {
        private readonly IItemRepository _itemRepository;
        public GetItemQueryHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task<Maybe<GetItemResponse>> Handle(GetItemQuery request, CancellationToken cancellationToken)
        {
            var getItemResult = await _itemRepository.GetAsync(request.ItemId);

            if (getItemResult.HasNoValue)
                return null;

            var item = getItemResult.Value;

            return new GetItemResponse(MapItem(item));
        }

        private ItemDto MapItem(Item item)
        {
            if(item == null)
                throw new ArgumentNullException(nameof(item));

            return new ItemDto() { 
                Id =  item.Id,
                IsArchive = item.IsArchive,
                MeasureType = item.MeasureType.ToString(),
                Name = item.Name,
            };
        }
    }
}
