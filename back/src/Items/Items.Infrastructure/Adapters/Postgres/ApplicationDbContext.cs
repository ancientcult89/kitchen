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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply Configuration
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
        }
    }
}
