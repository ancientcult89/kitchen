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
            //builder.Property(p => p.Name)
            //    .HasConversion(
            //        name => name.Value, // При сохранении: берем строковое значение
            //        value => ProductName.Create(value).Value) // При чтении: создаем Value Object
            //    .HasColumnName("Name")
            //    .HasColumnType("text")
            //    .HasMaxLength(255) // Укажите подходящую максимальную длину
            //    .IsRequired();

            builder.Property(entity => entity.IsArchive)
                .HasColumnName("is_archive")
                .IsRequired();

            // Важно: настраиваем связь и указываем, что MeasureType уже существует
            builder.HasOne(entity => entity.MeasureType)
                .WithMany()
                .HasForeignKey("measure_type_id")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Указываем свойство для внешнего ключа
            builder.Property<int>("measure_type_id")
                .HasColumnName("measure_type_id");

            // Важно: говорим EF, что MeasureType не нужно отслеживать изменения
            builder.Navigation(entity => entity.MeasureType)
                .AutoInclude(false); // Отключаем автоматическую загрузку

            //// Индексы (опционально)
            //builder.HasIndex(p => p.Name)
            //    .HasDatabaseName("IX_Products_Name");

            builder.HasIndex(p => p.IsArchive)
                .HasDatabaseName("IX_Products_IsArchive");
        }
    }
}
