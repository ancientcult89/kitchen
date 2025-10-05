using CSharpFunctionalExtensions;
using MediatR;
using Primitives;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Errors.Domain;
using Products.Core.Ports;

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
            var archivedItemResult = await _itemRepository.GetAsync(request.ProductId);

            if (archivedItemResult.HasNoValue)
                return ProductErrors.ProductIsNotExists(request.ProductId);

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
