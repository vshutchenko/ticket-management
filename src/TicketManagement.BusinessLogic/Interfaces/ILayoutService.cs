using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Models;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface ILayoutService
    {
        IEnumerable<LayoutModel> GetAll();

        Task<LayoutModel> GetByIdAsync(int id);

        Task<int> CreateAsync(LayoutModel layoutModel);

        Task UpdateAsync(LayoutModel layoutModel);

        Task DeleteAsync(int id);
    }
}
