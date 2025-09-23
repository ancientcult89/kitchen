using CSharpFunctionalExtensions;
using Items.Core.Application.UseCases.Commands.AddItem;
using Items.Core.Domain.Model.ItemAgregate;
using Items.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace Items.Infrastructure.Adapters.Postgres.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ItemRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task AddAsync(Item item)
        {
            await _dbContext.Items.AddAsync(item);
        }

        public bool CheckDuplicate(AddItemCommand request)
        {
            var normalizedRequestName = request.Name.Trim();
            var normalizedRequestMeasureType = request.MeasureType.Trim().ToLowerInvariant();

            // Один EF запрос с проверкой MeasureType на стороне БД
            return _dbContext.Items
                .Where(i => EF.Functions.ILike(i.Name, normalizedRequestName) &&
                            EF.Functions.ILike(i.MeasureType.Name, normalizedRequestMeasureType))
                .Any();
        }

        public async Task<Maybe<List<Item>>> GetAllAsync()
        {
            return await _dbContext.Items.ToListAsync();
        }

        public async Task<Maybe<Item>> GetAsync(Guid itemId)
        {
            if (itemId == Guid.Empty)
                return null;

            var item = await _dbContext
                .Items
                .SingleOrDefaultAsync(i => i.Id == itemId);

            return item;
        }

        public void Update(Item item)
        {
            _dbContext.Items.Update(item);
        }
    }
}
