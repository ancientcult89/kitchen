using CSharpFunctionalExtensions;
using MediatR;
using Products.Core.Application.Dto.ProductAggregate;
using Products.Core.Ports;

namespace Products.Core.Application.UseCases.Query.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Maybe<GetAllProductsResponse>>
    {
        private readonly IProductRepository _itemRepository;
        public GetAllProductsQueryHandler(IProductRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }
        public async Task<Maybe<GetAllProductsResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var result = await _itemRepository.GetAllAsync();

            if (result.HasNoValue)
                return null;

            if (result.Value.Count == 0)
                return null;

            return new GetAllProductsResponse(result.Value.Select(p => p.ToDto()).ToList());
        }
    }
}
