using System.Collections.Generic;

namespace TaskListGrpcServer.Repositories
{
    public interface IRepository<T>
        where T : class
    {
        List<T> GetAll();

        T GetById(int id);

        void Insert(T obj);

        void RemoveAt(int id);

        void RemoveAll();

        void Update(T executorUpdate);

        void Clear();



    }
}
