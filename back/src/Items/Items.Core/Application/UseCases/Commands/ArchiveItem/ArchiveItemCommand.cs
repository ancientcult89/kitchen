using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace Items.Core.Application.UseCases.Commands.ArchiveItem
{
    public class ArchiveItemCommand : IRequest<UnitResult<Error>>
    {
        public ArchiveItemCommand(Guid itemId)
        {
            ItemId = itemId;
        }
        public Guid ItemId { get; set; }
    }
}
