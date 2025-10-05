using Primitives;
using Products.Core.Domain.Model.SharedKernel;

namespace Products.Core.Errors.Application
{
    public static class AddProductErrors
    {
        public static Error ItemWithSameNameAndMeasureTypeExists(string name, MeasureType measureType)
        {
            return new Error("product.unique.violation", $"Product with name '{name}' and measure type '{measureType}' already exists");
        }
    }
}
