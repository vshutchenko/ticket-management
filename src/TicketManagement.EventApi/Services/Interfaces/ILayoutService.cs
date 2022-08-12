using TicketManagement.EventApi.Models;

namespace TicketManagement.EventApi.Services.Interfaces
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
