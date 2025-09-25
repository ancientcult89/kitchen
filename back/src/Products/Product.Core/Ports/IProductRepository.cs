using CSharpFunctionalExtensions;
using Products.Core.Application.UseCases.Commands.AddProduct;
using Products.Core.Domain.Model.ProductAggregate;

namespace Products.Core.Ports
{
    public interface IProductRepository
    {
        /// <summary>
        /// Добавить новую продуктовую позицию
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task AddAsync(Product item);

        /// <summary>
        /// Проверить на дублирование продуктовую позицию
        /// </summary>
        /// <param name="request"></param>
        /// <returns>истина, если есть аналогичный</returns>
        bool CheckDuplicate(AddProductCommand request);

        /// <summary>
        /// Получить продуктовую единицу
        /// </summary>
        /// <param name="courierId"></param>
        /// <returns>продуктовая единица</returns>
        Task<Maybe<Product>> GetAsync(Guid productId);

        /// <summary>
        /// Обновить продуктовую единицу
        /// </summary>
        /// <param name="item"></param>
        void Update(Product item);

        /// <summary>
        /// Получает все продуктовые единицы
        /// </summary>
        /// <param name="courierId"></param>
        /// <returns>Все продуктовые единицы</returns>
        Task<Maybe<List<Product>>> GetAllAsync();
    }
}
