using CSharpFunctionalExtensions;
using MediatR;
using Products.Core.Application.Dto.ProductAggregate;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Ports;

namespace Products.Core.Application.UseCases.Query.GetProduct
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Maybe<GetProductResponse>>
    {
        private readonly IProductRepository _itemRepository;
        public GetProductQueryHandler(IProductRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task<Maybe<GetProductResponse>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var getItemResult = await _itemRepository.GetAsync(request.ItemId);

            if (getItemResult.HasNoValue)
                return null;

            var item = getItemResult.Value;

            return new GetProductResponse(MapItem(item));
        }

        private ProductDto MapItem(Product item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return new ProductDto()
            {
                Id = item.Id,
                IsArchive = item.IsArchive,
                MeasureType = item.MeasureType.ToString(),
                Name = item.Name,
            };
        }
    }
}
