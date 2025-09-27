using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace Products.Core.Application.UseCases.Query.GetProduct
{
    public class GetProductQuery : IRequest<Maybe<GetProductResponse>>
    {
        private GetProductQuery(Guid productId)
        {
            ProductId = productId;
        }

        public static Result<GetProductQuery, Error> Create(Guid productId)
        {
            if (productId == Guid.Empty)
                return GeneralErrors.ValueIsRequired(nameof(productId));

            return new GetProductQuery(productId);
        }
        public Guid ProductId { get; set; }
    }
}
