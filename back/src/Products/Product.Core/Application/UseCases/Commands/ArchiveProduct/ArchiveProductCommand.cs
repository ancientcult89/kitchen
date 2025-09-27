using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace Products.Core.Application.UseCases.Commands.ArchiveProduct
{
    public class ArchiveProductCommand : IRequest<UnitResult<Error>>
    {
        private ArchiveProductCommand(Guid productId)
        {
            ProductId = productId;
        }

        public static Result<ArchiveProductCommand, Error> Create(Guid productId)
        {
            if (productId == Guid.Empty)
                return GeneralErrors.ValueIsRequired(nameof(productId));

            return new ArchiveProductCommand(productId);
        }
        public Guid ProductId { get; set; }
    }
}
