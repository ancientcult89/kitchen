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

            modelBuilder.Entity<MeasureType>(e =>
                e.HasData(
                    MeasureType.Weight,
                    MeasureType.Liquid));
        }
    }
}