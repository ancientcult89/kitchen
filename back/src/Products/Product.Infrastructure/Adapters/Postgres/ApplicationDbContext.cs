using Microsoft.EntityFrameworkCore;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;
using Products.Infrastructure.Adapters.Postgres.Entities;
using Products.Infrastructure.Adapters.Postgres.EntityConfiguration;

namespace Products.Infrastructure.Adapters.Postgres
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<MeasureType> MeasureTypes { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply Configuration
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new MeasureTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OutboxConfiguration());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetMeasureTypesAsUnchanged();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetMeasureTypesAsUnchanged();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void SetMeasureTypesAsUnchanged()
        {
            var measureTypeEntries = ChangeTracker.Entries<MeasureType>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in measureTypeEntries)
            {
                // Проверяем, что это один из предопределенных типов
                if (entry.Entity.Id == 1 || entry.Entity.Id == 2)
                {
                    entry.State = EntityState.Unchanged;
                }
            }
        }
    }
}