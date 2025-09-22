using CSharpFunctionalExtensions;
using MediatR;

namespace Items.Core.Application.UseCases.Query.GetItem
{
    public class GetItemQuery : IRequest<Maybe<GetItemResponse>>
    {
        public Guid ItemId { get; set; }
    }
}
