using CSharpFunctionalExtensions;
using Primitives;
using Products.Core.Domain.Model.SharedKernel;

namespace Products.Core.Domain.Model.MeasureAggregate
{
    /// <summary>
    /// Единица измерения (л, кг и т.д.)
    /// </summary>
    public class Measure : Aggregate<Guid>
    {
        /// <summary>
        /// Полное наименование единицы измерения
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Краткое наименование единицы измерения
        /// </summary>
        public string ShortName { get; private set; }

        /// <summary>
        /// Признак архивности записи
        /// </summary>
        public bool IsArchive { get; private set; }

        /// <summary>
        /// Способ измерения продукта (в чём мерить массу или объём если жидкость)
        /// </summary>
        public MeasureType MeasureType { get; private set; }

        private Measure() { }
        private Measure(Guid id, string fullName, string shortName, MeasureType measureType) : this()
        {
            Id = id;
            FullName = fullName;
            ShortName = shortName;
            MeasureType = measureType;
            IsArchive = false;
        }

        public static Result<Measure, Error> Create(string fullName, string shortName, MeasureType measureType)
        {
            throw new NotImplementedException();
        }
    }
}
