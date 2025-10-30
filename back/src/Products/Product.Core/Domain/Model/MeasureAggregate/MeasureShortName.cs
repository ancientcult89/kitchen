using CSharpFunctionalExtensions;
using Primitives;

namespace Products.Core.Domain.Model.MeasureAggregate
{
    public class MeasureShortName : ValueObject
    {
        private MeasureShortName() { }
        /// <summary>
        /// Полное название измерения
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Максимальная длинна короткого наименования измерения
        /// </summary>
        private const int MaxLenght = 6;

        private MeasureShortName(string name)
        {
            Value = name;
        }

        public static Result<MeasureShortName, Error> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return GeneralErrors.ValueIsInvalid(nameof(name));

            if (name.Length > MaxLenght)
                return GeneralErrors.ValueIsTooLong(MaxLenght, name);

            return new MeasureShortName(name);
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
