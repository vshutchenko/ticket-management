using RestEase;
using TicketManagement.WebApplication.Clients.EventApi.Models;

namespace TicketManagement.WebApplication.Clients.EventApi
{
    [Header("Content-Type", "application/json")]
    public interface IEventSeatClient
    {
        [Get("event-seats/areas/{areaId}")]
        public Task<List<EventSeatModel>> GetByAreaIdAsync([Path] int areaId,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("event-seats/{id}")]
        public Task<EventSeatModel> GetByIdAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Put("event-seats/{seatId}/state")]
        public Task UpdateStateAsync([Path] int seatId,
            [Body] int state,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);
    }
}
