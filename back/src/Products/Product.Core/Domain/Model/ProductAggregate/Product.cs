using CSharpFunctionalExtensions;
using Primitives;
using Products.Core.Domain.Model.SharedKernel;
using Products.Core.Errors.Domain;

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
        public ProductName Name { get; private set; }

        /// <summary>
        /// Способ измерения продукта (в чём мерить массу или объём если жидкость)
        /// </summary>
        public MeasureType MeasureType { get; private set; }

        public bool IsArchive { get; private set; }

        private Product() { }
        private Product(Guid id, ProductName name, MeasureType measureType) : this()
        {
            Id = id;
            Name = name;
            MeasureType = measureType;
            IsArchive = false;
        }

        /// <summary>
        /// Создать продукт
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="measureType"></param>
        /// <returns></returns>
        public static Result<Product, Error> Create(ProductName productName, MeasureType measureType)
        {
            if(productName == null)
                return GeneralErrors.ValueIsRequired(nameof(productName));

            if (measureType == null)
                return GeneralErrors.ValueIsRequired(nameof(measureType));

            Guid id = Guid.NewGuid();

            return new Product(id, productName, measureType);
        }

        public UnitResult<Error> MakeArchive()
        {
            if (this.IsArchive)
                return ProductErrors.ProductIsAlreadyArchived(this.Id);

            this.IsArchive = true;

            return UnitResult.Success<Error>();
        }

        public UnitResult<Error> MakeUnArchive()
        {
            if (!this.IsArchive)
                return ProductErrors.ProductIsAlreadyUnArchived(this.Id);

            this.IsArchive = false;
            return UnitResult.Success<Error>();
        }
    }
}
