using MediatR;
using Primitives;

namespace Products.Core.Errors.Domain
{
    public static class ProductErrors
    {
        public static Error ProductIsNotExists(Guid productId)
        {
            if (productId == Guid.Empty) throw new ArgumentException(productId.ToString());

            return new Error("product.is.not.exists", $"The product with ID: {productId} is not exists");
        }
    }
}
