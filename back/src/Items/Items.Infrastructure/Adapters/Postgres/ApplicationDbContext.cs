using Items.Core.Domain.Model.ItemAgregate;
using Items.Infrastructure.Adapters.Postgres.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Items.Infrastructure.Adapters.Postgres
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<MeasureType> MeasureTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply Configuration
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new MeasureTypeConfiguration());
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