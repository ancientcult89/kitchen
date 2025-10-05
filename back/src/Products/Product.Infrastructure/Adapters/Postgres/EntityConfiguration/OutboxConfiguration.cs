using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Infrastructure.Adapters.Postgres.Entities;

namespace Products.Infrastructure.Adapters.Postgres.EntityConfiguration
{
    public class OutboxConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> entityTypeBuilder)
        {
            entityTypeBuilder
                .ToTable("outbox");

            entityTypeBuilder
                .Property(entity => entity.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entityTypeBuilder
                .Property(entity => entity.Type)
                .HasColumnName("type")
                .IsRequired();

            entityTypeBuilder
                .Property(entity => entity.Content)
                .HasColumnName("content")
                .IsRequired();

            entityTypeBuilder
                .Property(entity => entity.OccurredOnUtc)
                .HasColumnName("occurred_on_utc")
                .IsRequired();

            entityTypeBuilder
                .Property(entity => entity.ProcessedOnUtc)
                .HasColumnName("processed_on_utc")
                .IsRequired(false);
        }
    }
}
