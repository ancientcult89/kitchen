using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Core.Domain.Model.ProductAggregate;

namespace Products.Infrastructure.Adapters.Postgres.EntityConfiguration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.HasKey(entity => entity.Id);

            builder.Property(entity => entity.Id)
                .ValueGeneratedNever()
                .HasColumnName("id")
                .IsRequired();

            // Value Object: ProductName - сохраняем как собственную колонку
            builder.OwnsOne(p => p.Name, a =>
            {
                a.Property(u => u.Value)
                    .HasColumnName("name")
                    .HasColumnType("text")
                    .IsRequired();

                // Индекс для owned property
                a.HasIndex(u => u.Value)
                    .HasDatabaseName("IX_products_name");
            });

            builder.Property(entity => entity.IsArchive)
                .HasColumnName("is_archive")
                .IsRequired();

            // Важно: настраиваем связь и указываем, что MeasureType уже существует
            builder.HasOne(entity => entity.MeasureType)
                .WithMany()
                .IsRequired()
                .HasForeignKey("measure_type_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.IsArchive)
                .HasDatabaseName("IX_Products_IsArchive");
        }
    }
}
