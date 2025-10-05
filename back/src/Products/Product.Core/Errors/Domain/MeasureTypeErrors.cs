using Primitives;
using Products.Core.Domain.Model.SharedKernel;

namespace Products.Core.Errors.Domain
{
    public static class MeasureTypeErrors
    {
        public static Error UnknownType()
        {
            return new Error("unknown.measure.type", $"Possible values for {nameof(MeasureType)}: {string.Join(",", MeasureType.List().Select(s => s.Name))}");
        }
    }
}
