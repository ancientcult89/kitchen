using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace Products.Core.Application.UseCases.Commands.ArchiveProduct
{
    public class ArchiveProductCommand : IRequest<UnitResult<Error>>
    {
        public ArchiveProductCommand(Guid itemId)
        {
            ItemId = itemId;
        }
        public Guid ItemId { get; set; }
    }
}
