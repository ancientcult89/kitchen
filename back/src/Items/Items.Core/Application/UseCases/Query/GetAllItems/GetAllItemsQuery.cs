using CSharpFunctionalExtensions;
using MediatR;

namespace Items.Core.Application.UseCases.Query.GetAllItems
{
    public class GetAllItemsQuery : IRequest<Maybe<GetAllItemsResponse>>
    {
    }
}
