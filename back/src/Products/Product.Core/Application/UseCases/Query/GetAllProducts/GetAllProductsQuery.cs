using CSharpFunctionalExtensions;
using MediatR;

namespace Products.Core.Application.UseCases.Query.GetAllProducts
{
    public class GetAllProductsQuery : IRequest<Maybe<GetAllProductsResponse>>
    {
    }
}
