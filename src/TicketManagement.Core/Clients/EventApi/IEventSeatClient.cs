using RestEase;
using TicketManagement.Core.Clients.EventApi.Models;

namespace TicketManagement.Core.Clients.EventApi
{
    [Header("Content-Type", "application/json")]
    public interface IEventSeatClient
    {
        [Get("eventSeats/{id}")]
        public Task<EventSeatModel> GetByIdAsync([Path] int id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Put("eventSeats/{seatId}/state")]
        public Task UpdateStateAsync([Path] int seatId,
            [Body] EventSeatState state,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);
    }
}
