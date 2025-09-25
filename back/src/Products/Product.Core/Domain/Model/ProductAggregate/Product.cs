using CSharpFunctionalExtensions;
using Primitives;
using Products.Core.Domain.Model.SharedKernel;

namespace Products.Core.Domain.Model.ProductAggregate
{
    /// <summary>
    /// Продукт
    /// </summary>
    public class Product : Aggregate<Guid>
    {
        /// <summary>
        /// название продукта
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Способ измерения продукта (в чём мерить массу или объём если жидкость)
        /// </summary>
        public MeasureType MeasureType { get; private set; }

        public bool IsArchive { get; private set; }

        private Product() { }
        private Product(Guid id, string name, MeasureType measureType) : this()
        {
            Id = id;
            Name = name;
            MeasureType = measureType;
            IsArchive = false;
        }

        /// <summary>
        /// Создать продукт
        /// </summary>
        /// <param name="name"></param>
        /// <param name="measureType"></param>
        /// <returns></returns>
        public static Result<Product, Error> Create(string name, MeasureType measureType)
        {
            if (string.IsNullOrWhiteSpace(name))
                return GeneralErrors.ValueIsInvalid(nameof(name));

            if (measureType == null)
                return GeneralErrors.ValueIsRequired(nameof(measureType));

            Guid id = Guid.NewGuid();

            return new Product(id, name, measureType);
        }

        public UnitResult<Error> MakeArchive()
        {
            if (this.IsArchive)
                return Errors.ItemIsAlreadyArchived(this.Id);

            this.IsArchive = true;

            return UnitResult.Success<Error>();
        }

        public UnitResult<Error> MakeUnArchive()
        {
            if (!this.IsArchive)
                return Errors.ItemIsAlreadyUnArchived(this.Id);

            this.IsArchive = false;
            return UnitResult.Success<Error>();
        }

        public static class Errors
        {
            public static Error ItemIsAlreadyArchived(Guid itemId)
            {
                if (itemId == Guid.Empty) throw new ArgumentException(itemId.ToString());

                return new Error("item.is.already.archived", $"The Item {itemId.ToString()} is already archived");
            }

            public static Error ItemIsAlreadyUnArchived(Guid itemId)
            {
                if (itemId == Guid.Empty) throw new ArgumentException(itemId.ToString());

                return new Error("item.is.already.unarchived", $"The Item {itemId.ToString()} is already unarchived");
            }
        }
    }
}
