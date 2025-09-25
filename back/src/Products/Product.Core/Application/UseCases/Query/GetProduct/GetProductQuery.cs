using CSharpFunctionalExtensions;
using MediatR;

namespace Products.Core.Application.UseCases.Query.GetProduct
{
    public class GetProductQuery : IRequest<Maybe<GetProductResponse>>
    {
        public GetProductQuery(Guid itemId)
        {
            ItemId = itemId;
        }
        public Guid ItemId { get; set; }
    }
}
