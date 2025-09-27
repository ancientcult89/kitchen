using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Products.Core.Application.UseCases.Commands.AddProduct;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Ports;

namespace Products.Infrastructure.Adapters.Postgres.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task AddAsync(Product item)
        {
            await _dbContext.Products.AddAsync(item);
        }

        public bool CheckDuplicate(AddProductCommand request)
        {
            var normalizedRequestName = request.Name.Trim();
            var measureTypeId = request.MeasureTypeId;

            // Один EF запрос с проверкой MeasureType на стороне БД
            return _dbContext.Products
                .Where(i => EF.Functions.ILike(i.Name, normalizedRequestName) &&
                            i.MeasureType.Id == measureTypeId)
                .Any();
        }

        public async Task<Maybe<List<Product>>> GetAllAsync()
        {
            return await _dbContext.Products.Include(i => i.MeasureType).ToListAsync();
        }

        public async Task<Maybe<Product>> GetAsync(Guid itemId)
        {
            if (itemId == Guid.Empty)
                return null;

            var item = await _dbContext
                .Products
                .Include(i => i.MeasureType)
                .SingleOrDefaultAsync(i => i.Id == itemId);

            return item;
        }

        public void Update(Product item)
        {
            _dbContext.Products.Update(item);
        }
    }
}
