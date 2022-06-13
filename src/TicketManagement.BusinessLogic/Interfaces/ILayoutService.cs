using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface ILayoutService
    {
        IEnumerable<Layout> GetAll();
        Task<Layout> GetByIdAsync(int id);
        Task<int> CreateAsync(Layout layout);
        Task UpdateAsync(Layout layout);
        Task DeleteAsync(int id);
    }
}
