using CSharpFunctionalExtensions;
using Primitives;
using Products.Core.Domain.Model.ProductAggregate;

namespace Products.Core.Domain.Model.MeasureAggregate
{
    public class MeasureFullName : ValueObject
    {
        private MeasureFullName() { }
        /// <summary>
        /// Полное название измерения
        /// </summary>
        public string Value { get; private set; }

        private MeasureFullName(string name)
        {
            Value = name;
        }

        public static Result<MeasureFullName, Error> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return GeneralErrors.ValueIsInvalid(nameof(name));

            return new MeasureFullName(name);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
