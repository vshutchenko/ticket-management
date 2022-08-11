using System.Security.Claims;

namespace TicketManagement.UserApi.Services.Interfaces
{
    public interface ITokenService
    {
        string GetToken(string key, string audience, string issuer, List<Claim> claims);
        bool IsValidToken(string key, string issuer, string audience, string token);
    }
}
