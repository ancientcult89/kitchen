using Items.Core.Domain.Model.ItemAgregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Adapters.Postgres.EntityConfiguration
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable("items");

            entityTypeBuilder.HasKey(entity => entity.Id);

            entityTypeBuilder
                .Property(entity => entity.Id)
                .ValueGeneratedNever()
                .HasColumnName("id")
                .IsRequired();

            entityTypeBuilder
                .Property(entity => entity.Name)
                .HasColumnName("name")
                .IsRequired();

            entityTypeBuilder
                .Property(entity => entity.IsArchive)
                .HasColumnName("is_archive")
                .IsRequired();

            entityTypeBuilder
                .OwnsOne(entity => entity.MeasureType, a =>
                {
                    a.Property(c => c.Name).HasColumnName("measure_type").IsRequired();
                    a.WithOwner();
                });
            entityTypeBuilder.Navigation(entity => entity.MeasureType).IsRequired();
        }
    }
}
