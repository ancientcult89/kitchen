using CSharpFunctionalExtensions;
using Products.Core.Errors.Domain;

namespace Primitives
{
    public abstract class ArchivableAggregate <TId> : Aggregate<TId> where TId : IComparable<TId>
    {
        public bool IsArchive { get; protected set; }
        protected ArchivableAggregate(TId id) : base(id)
        {
        }

        protected ArchivableAggregate()
        {
        }

        public virtual UnitResult<Error> MakeArchive()
        {
            if (this.IsArchive)
            {
                var childTypeName = GetChildTypeName();
                return ArchivationErrors.ProductIsAlreadyArchived(this.Id.ToString(), childTypeName);
            }

            this.IsArchive = true;

            return UnitResult.Success<Error>();
        }

        public virtual UnitResult<Error> MakeUnArchive()
        {
            if (!this.IsArchive)
            {
                var childTypeName = GetChildTypeName();
                return ArchivationErrors.ProductIsAlreadyUnArchived(this.Id.ToString(), childTypeName);
            }

            this.IsArchive = false;
            return UnitResult.Success<Error>();
        }

        protected string GetChildTypeName()
        {
            // Получаем фактический тип объекта (наследника)
            var actualType = GetType();

            // Если тип является прокси-классом (например, от Entity Framework),
            // получаем базовый тип
            var type = actualType;
            if (IsProxyType(actualType) && actualType.BaseType != null)
            {
                type = actualType.BaseType;
            }

            return type.Name;
        }

        private static bool IsProxyType(Type type)
        {
            return type.Name.Contains("Proxy") ||
                   type.Namespace?.Contains("DynamicProxies") == true ||
                   type.Assembly.FullName?.Contains("EntityFramework") == true;
        }
    }
}
