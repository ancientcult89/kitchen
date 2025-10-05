using CSharpFunctionalExtensions;
using Primitives;

namespace Products.Core.Domain.Model.ProductAggregate
{
    public class ProductName : ValueObject
    {
        private ProductName() { }
        /// <summary>
        /// Имя продукта
        /// </summary>
        public string Value { get; private set; }

        private ProductName(string name)
        {
            Value = name;
        }

        public static Result<ProductName, Error> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return GeneralErrors.ValueIsInvalid(nameof(name));

            return new ProductName(name);
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
