using Primitives;

namespace Products.Core.Errors.Domain
{
    public class ArchivationErrors
    {
        public static Error ProductIsAlreadyArchived(string entityId, string entityName)
        {
            if (string.IsNullOrWhiteSpace(entityId)) throw new ArgumentException(entityId.ToString());
            if (string.IsNullOrWhiteSpace(entityName)) throw new ArgumentException(entityId.ToString());

            return new Error("record.is.already.archived", $"The {entityName} with ID: {entityId.ToString()} is already archived");
        }

        public static Error ProductIsAlreadyUnArchived(string entityId, string entityName)
        {
            if (string.IsNullOrWhiteSpace(entityId)) throw new ArgumentException(entityId.ToString());
            if (string.IsNullOrWhiteSpace(entityName)) throw new ArgumentException(entityId.ToString());

            return new Error("record.is.already.unarchived", $"The {entityName} with ID: {entityId.ToString()} is already unarchived");
        }
    }
}
