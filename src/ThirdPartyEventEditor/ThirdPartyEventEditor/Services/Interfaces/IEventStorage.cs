using System.Collections.Generic;
using System.Threading.Tasks;
using ThirdPartyEventEditor.Models;

namespace ThirdPartyEventEditor.Services.Interfaces
{
    public interface IEventStorage
    {
        Task CreateAsync(ThirdPartyEvent @event);

        Task UpdateAsync(ThirdPartyEvent @event);

        Task DeleteAsync(string id);

        Task<List<ThirdPartyEvent>> GetAllAsync();

        Task<ThirdPartyEvent> GetByIdAsync(string id);
    }
}
