using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TicketManagement.IntegrationTests")]
[assembly: InternalsVisibleTo("TicketManagement.UnitTests")]

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
