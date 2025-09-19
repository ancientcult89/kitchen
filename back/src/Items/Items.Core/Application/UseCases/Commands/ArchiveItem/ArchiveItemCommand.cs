using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace Items.Core.Application.UseCases.Commands.ArchiveItem
{
    public class ArchiveItemCommand : IRequest<UnitResult<Error>>
    {
        public Guid ItemId { get; set; }
    }
}
