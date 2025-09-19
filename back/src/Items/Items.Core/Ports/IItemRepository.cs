using CSharpFunctionalExtensions;
using Items.Core.Application.UseCases.Commands.AddItem;
using Items.Core.Domain.Model.ItemAgregate;
using System.Diagnostics.Metrics;

namespace Items.Core.Ports
{
    public interface IItemRepository
    {
        /// <summary>
        /// Добавить новую продуктовую позицию
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task AddAsync(Item item);

        /// <summary>
        /// Проверить на дублирование продуктовую позицию
        /// </summary>
        /// <param name="request"></param>
        /// <returns>истина, если есть аналогичный</returns>
        bool CheckDuplicate(AddItemCommand request);

        /// <summary>
        /// Получить продуктовую единицу
        /// </summary>
        /// <param name="courierId"></param>
        /// <returns>продуктовая единица</returns>
        Task<Maybe<Item>> GetAsync(Guid courierId);

        /// <summary>
        /// Обновить продуктовую единицу
        /// </summary>
        /// <param name="item"></param>
        void Update(Item item);

        /// <summary>
        /// Получает все продуктовые единицы
        /// </summary>
        /// <param name="courierId"></param>
        /// <returns>Все продуктовые единицы</returns>
        Task<Maybe<List<Item>>> GetAllAsync();
    }
}
