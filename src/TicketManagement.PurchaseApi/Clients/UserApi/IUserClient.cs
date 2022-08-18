using RestEase;

namespace TicketManagement.PurchaseApi.Clients.UserApi
{
    [Header("Content-Type", "application/json")]
    public interface IUserClient
    {
        [Get("users/validate")]
        public Task ValidateTokenAsync([Query] string token,
            CancellationToken cancellationToken = default);
    }
}
