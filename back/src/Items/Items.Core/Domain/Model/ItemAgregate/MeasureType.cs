using CSharpFunctionalExtensions;
using Primitives;

namespace Items.Core.Domain.Model.ItemAgregate
{
    public class MeasureType : ValueObject
    {
        //Название меры измерения
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

        public static Result<MeasureType, Error> CreateFromString(string measureTypeName)
        {
            if (string.IsNullOrWhiteSpace(measureTypeName))
                return new Error("measure.type.cannot.be.empty", "Measure type name cannot be empty");

            var normalizedName = measureTypeName.Trim().ToLowerInvariant();

            return normalizedName switch
            {
                "weight" => MeasureType.Weight,
                "liquid" => MeasureType.Liquid,
                _ => new Error("unknown.measure.type", $"Unknown measure type: {normalizedName}")
            };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name.ToLowerInvariant();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
