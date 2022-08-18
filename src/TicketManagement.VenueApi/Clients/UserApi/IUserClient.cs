using RestEase;

namespace TicketManagement.VenueApi.Clients.UserApi
{
    public interface IUserClient
    {
        [Get("users/validate")]
        public Task ValidateTokenAsync([Query] string token,
            CancellationToken cancellationToken = default);
    }
}
