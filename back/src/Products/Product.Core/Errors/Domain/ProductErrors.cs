using MediatR;
using Primitives;

namespace Products.Core.Errors.Domain
{
    public static class ProductErrors
    {
        public static Error ProductIsAlreadyArchived(Guid productId)
        {
            if (productId == Guid.Empty) throw new ArgumentException(productId.ToString());

            return new Error("product.is.already.archived", $"The Producr {productId.ToString()} is already archived");
        }

        public static Error ProductIsAlreadyUnArchived(Guid productId)
        {
            if (productId == Guid.Empty) throw new ArgumentException(productId.ToString());

            return new Error("product.is.already.unarchived", $"The Product {productId.ToString()} is already unarchived");
        }

        public static Error ProductIsNotExists(Guid productId)
        {
            if (productId == Guid.Empty) throw new ArgumentException(productId.ToString());

            return new Error("product.is.not.exists", $"The product with ID: {productId} is not exists");
        }
    }
}
