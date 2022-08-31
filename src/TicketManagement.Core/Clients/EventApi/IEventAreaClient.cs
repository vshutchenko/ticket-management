using RestEase;
using TicketManagement.Core.Clients.EventApi.Models;

namespace TicketManagement.Core.Clients.EventApi
{
    [Header("Content-Type", "application/json")]
    public interface IEventAreaClient
    {
        [Get("eventAreas/{id}")]
        public Task<EventAreaModel> GetByIdAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Put("eventAreas/{areaId}/price")]
        public Task UpdatePriceAsync([Path] int areaId,
            [Body] decimal price,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("eventAreas/{id}/seats")]
        public Task<List<EventSeatModel>> GetSeatsByAreaIdAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);
    }
}
