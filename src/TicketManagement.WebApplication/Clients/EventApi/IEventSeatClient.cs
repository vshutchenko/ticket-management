using RestEase;
using TicketManagement.WebApplication.Clients.EventApi.Models;

namespace TicketManagement.WebApplication.Clients.EventApi
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
