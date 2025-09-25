using CSharpFunctionalExtensions;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Ports;
using MediatR;
using Primitives;

namespace Products.Core.Application.UseCases.Commands.ArchiveProduct
{
    public class ArchiveProductCommandHandler : IRequestHandler<ArchiveProductCommand, UnitResult<Error>>
    {
        private readonly IProductRepository _itemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ArchiveProductCommandHandler(IProductRepository itemRepository, IUnitOfWork unitOfWork)
        {
            _itemRepository = itemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UnitResult<Error>> Handle(ArchiveProductCommand request, CancellationToken cancellationToken)
        {
            if (Guid.Empty == request.ItemId)
                return GeneralErrors.ValueIsRequired(nameof(request.ItemId));

            var archivedItemResult = await _itemRepository.GetAsync(request.ItemId);

            if (archivedItemResult.HasNoValue)
                return new Error("no.such.item", $"There is no items with ID: {request.ItemId}");

            Product archivedItem = archivedItemResult.Value;
            var result = archivedItem.MakeArchive();

            if (!result.IsSuccess)
                return result.Error;

            _itemRepository.Update(archivedItem);

            await _unitOfWork.SaveChangesAsync();

            return UnitResult.Success<Error>();
        }
    }
}
