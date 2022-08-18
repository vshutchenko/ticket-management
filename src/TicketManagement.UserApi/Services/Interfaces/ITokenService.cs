using TicketManagement.DataAccess.Entities;

namespace TicketManagement.UserApi.Services.Interfaces
{
    public interface ITokenService
    {
        string GetToken(User user, IList<string> roles);
        bool ValidateToken(string token);
    }
}
