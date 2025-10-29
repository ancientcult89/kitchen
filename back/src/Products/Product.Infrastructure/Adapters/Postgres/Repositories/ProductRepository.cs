using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Primitives;
using Products.Core.Application.UseCases.Commands.AddProduct;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Errors.Application;
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
        public async Task AddAsync(Product product)
        {
            if (product.MeasureType != null) _dbContext.Attach(product.MeasureType);

            await _dbContext.Products.AddAsync(product);
        }

        public Result<bool, Error> IsDuplicate(AddProductCommand request)
        {
            if (request == null)
                return CQSErrors.IncorrectCommand();

            return _dbContext.Products
                .Include(p => p.MeasureType)
                .Where(p => p.Name.Value.ToLower() == request.Name.Value.ToLowerInvariant() &&
                            p.MeasureType == request.MeasureType)
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

        public void Update(Product product)
        {
            if (product.MeasureType != null) _dbContext.Attach(product.MeasureType);

            _dbContext.Products.Update(product);
        }
    }
}
