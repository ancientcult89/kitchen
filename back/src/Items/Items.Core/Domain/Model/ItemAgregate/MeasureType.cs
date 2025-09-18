using CSharpFunctionalExtensions;
using System.Diagnostics.CodeAnalysis;

namespace Items.Core.Domain.Model.ItemAgregate
{
    [ExcludeFromCodeCoverage]
    public class MeasureType : ValueObject
    {
        //название меры измерения
        public string Name { get; private set; }

        /// <summary>
        /// Единицы измерения по массе
        /// </summary>
        public static MeasureType Weight => new(nameof(Weight).ToLowerInvariant());

        /// <summary>
        /// Единицы измерения для жидкости
        /// </summary>
        public static MeasureType Liquid => new(nameof(Liquid).ToLowerInvariant());
        private MeasureType() { }
        private MeasureType(string name) : this()
        {
            Name = name;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}
