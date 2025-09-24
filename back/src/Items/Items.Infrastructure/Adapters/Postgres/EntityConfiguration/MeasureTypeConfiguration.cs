using Items.Core.Domain.Model.ItemAgregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Items.Infrastructure.Adapters.Postgres.EntityConfiguration
{
    public class MeasureTypeConfiguration : IEntityTypeConfiguration<MeasureType>
    {
        public void Configure(EntityTypeBuilder<MeasureType> builder)
        {
            builder.ToTable("measure_types"); // или другое имя таблицы

            builder.HasKey(mt => mt.Id);

            builder.Property(mt => mt.Id)
                .ValueGeneratedNever() // ID задаются вручную через статические поля
                .HasColumnName("id")
                .IsRequired();

            builder.Property(mt => mt.Name)
                .HasColumnName("name")
                .HasMaxLength(50)
                .IsRequired();

            // Заполняем таблицу предопределенными значениями
            builder.HasData(
                MeasureType.Weight,
                MeasureType.Liquid
            );
        }
    }
}
