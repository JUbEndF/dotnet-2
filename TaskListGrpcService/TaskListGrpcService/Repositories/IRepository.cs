using System.Collections.Generic;

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
        List<T> GetAll();

        /// <summary>
        /// Getting an element by its unique id
        /// </summary>
        /// <param name="id">element id</param>
        /// <returns>search element</returns>
        T GetById(int id);

        /// <summary>
        /// Inserting an element
        /// </summary>
        /// <param name="obj">add element</param>
        void Insert(T obj);

        /// <summary>
        /// Remove an element by its id
        /// </summary>
        /// <param name="id">id remove element</param>
        void RemoveAt(int id);

        /// <summary>
        /// remove all element list
        /// </summary>
        void RemoveAll();

        /// <summary>
        /// Update an element
        /// </summary>
        /// <param name="executorUpdate">Updated Item</param>
        void Update(T executorUpdate);
    }
}
