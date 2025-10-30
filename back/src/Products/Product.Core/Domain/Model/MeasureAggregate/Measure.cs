using CSharpFunctionalExtensions;
using Primitives;
using Products.Core.Domain.Model.ProductAggregate;
using Products.Core.Domain.Model.SharedKernel;

namespace Products.Core.Domain.Model.MeasureAggregate
{
    /// <summary>
    /// Единица измерения (л, кг и т.д.)
    /// </summary>
    public class Measure : ArchivableAggregate<Guid>
    {
        /// <summary>
        /// Полное наименование единицы измерения
        /// </summary>
        public MeasureFullName FullName { get; private set; }

        /// <summary>
        /// Краткое наименование единицы измерения
        /// </summary>
        public MeasureShortName ShortName { get; private set; }

        /// <summary>
        /// Способ измерения продукта (в чём мерить массу или объём если жидкость)
        /// </summary>
        public MeasureType MeasureType { get; private set; }

        private Measure() { }
        private Measure(Guid id, MeasureFullName fullName, MeasureShortName shortName, MeasureType measureType) : this()
        {
            Id = id;
            FullName = fullName;
            ShortName = shortName;
            MeasureType = measureType;
            IsArchive = false;
        }

        public static Result<Measure, Error> Create(MeasureFullName fullName, MeasureShortName shortName, MeasureType measureType)
        {
            if(fullName == null)
                return GeneralErrors.ValueIsRequired(nameof(fullName));

            if (shortName == null)
                return GeneralErrors.ValueIsRequired(nameof(shortName));

            if (measureType == null)
                return GeneralErrors.ValueIsRequired(nameof(measureType));

            return new Measure(Guid.NewGuid(), fullName, shortName, measureType);
        }
    }
}
