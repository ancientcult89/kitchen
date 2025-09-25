using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace Products.Core.Application.UseCases.Commands.UnArchiveProduct
{
    public class UnArchiveProductCommand : IRequest<UnitResult<Error>>
    {
        public UnArchiveProductCommand(Guid itemId)
        {
            ItemId = itemId;
        }
        public Guid ItemId { get; set; }
    }
}
