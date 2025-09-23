using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace Items.Core.Application.UseCases.Commands.UnArchiveItem
{
    public class UnArchiveItemCommand : IRequest<UnitResult<Error>>
    {
        public UnArchiveItemCommand(Guid itemId)
        {
            ItemId = itemId;
        }
        public Guid ItemId { get; set; }
    }
}
