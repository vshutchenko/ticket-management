using System.Collections.Generic;
using System.Threading.Tasks;
using ThirdPartyEventEditor.Models;

namespace ThirdPartyEventEditor.Services.Interfaces
{
    public interface IEventStorage
    {
        Task<List<ThirdPartyEvent>> GetEventsAsync();

        Task SaveEventsAsync(List<ThirdPartyEvent> events);
    }
}
