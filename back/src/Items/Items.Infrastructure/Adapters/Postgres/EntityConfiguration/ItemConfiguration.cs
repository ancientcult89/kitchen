using Items.Core.Domain.Model.ItemAgregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Adapters.Postgres.EntityConfiguration
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("items");

            builder.HasKey(entity => entity.Id);

            builder.Property(entity => entity.Id)
                .ValueGeneratedNever()
                .HasColumnName("id")
                .IsRequired();

            builder.Property(entity => entity.Name)
                .HasColumnName("name")
                .IsRequired();

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
        }
    }
}
