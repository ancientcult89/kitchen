using CSharpFunctionalExtensions;
using Items.Core.Domain.Model.ItemAgregate;
using Items.Core.Ports;
using MediatR;
using Primitives;

namespace Items.Core.Application.UseCases.Commands.UnArchiveItem
{
    public class UnArchiveItemCommandHandler : IRequestHandler<UnArchiveItemCommand, UnitResult<Error>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UnArchiveItemCommandHandler(IItemRepository itemRepository, IUnitOfWork unitOfWork)
        {
            _itemRepository = itemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UnitResult<Error>> Handle(UnArchiveItemCommand request, CancellationToken cancellationToken)
        {
            if (Guid.Empty == request.ItemId)
                return GeneralErrors.ValueIsRequired(nameof(request.ItemId));

            var archivedItemResult = await _itemRepository.GetAsync(request.ItemId);

            if (archivedItemResult.HasNoValue)
                return new Error("no.such.item", $"There is no items with ID: {request.ItemId}");

            Item archivedItem = archivedItemResult.Value;

            var result = archivedItem.MakeUnArchive();

            if (!result.IsSuccess)
                return result.Error;

            _itemRepository.Update(archivedItem);

            await _unitOfWork.SaveChangesAsync();

            return UnitResult.Success<Error>();
        }
    }
}
