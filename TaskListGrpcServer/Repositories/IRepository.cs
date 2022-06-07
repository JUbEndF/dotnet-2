using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskListGrpcServer.Repositories
{
    /// <summary>
    /// repository interface
    /// </summary>
    /// <typeparam name="T">storage class</typeparam>
    public interface IRepository<T>
        where T : class
    {
        /// <summary>
        /// getting all elements
        /// </summary>
        /// <returns>all elements</returns>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Getting an element by its unique id
        /// </summary>
        /// <param name="id">element id</param>
        /// <returns>search element</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Inserting an element
        /// </summary>
        /// <param name="obj">add element</param>
        void Insert(T obj);

        /// <summary>
        /// Remove an element by its id
        /// </summary>
        /// <param name="id">id remove element</param>
        void RemoveAtAsync(int id);

        /// <summary>
        /// remove all element list
        /// </summary>
        void RemoveAllAsync();

        /// <summary>
        /// Update an element
        /// </summary>
        /// <param name="executorUpdate">Updated Item</param>
        Task<bool> UpdateAsync(T executorUpdate);
    }
}
