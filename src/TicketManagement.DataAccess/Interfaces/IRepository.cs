using System.Collections.Generic;

namespace TicketManagement.DataAccess.Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        int Create(T item);
        void Update(T item);
        void Delete(int id);
    }
}
