using CSharpFunctionalExtensions;
using MediatR;

namespace Items.Core.Application.UseCases.Query.GetItem
{
    public class GetItemQuery : IRequest<Maybe<GetItemResponse>>
    {
        public GetItemQuery(Guid itemId)
        {
            ItemId = itemId;
        }
        public Guid ItemId { get; set; }
    }
}
