using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace Products.Core.Application.UseCases.Commands.UnArchiveProduct
{
    public class UnArchiveProductCommand : IRequest<UnitResult<Error>>
    {
        private UnArchiveProductCommand(Guid productId)
        {
            ProductId = productId;
        }

        public static Result<UnArchiveProductCommand, Error> Create(Guid productId)
        {
            if (productId == Guid.Empty)
                return GeneralErrors.ValueIsRequired(nameof(productId));

            return new UnArchiveProductCommand(productId);
        }
        public Guid ProductId { get; set; }
    }
}
