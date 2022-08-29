namespace TicketManagement.WebApplication.Services
{
    public interface ITokenService
    {
        public string GetToken();

        public void SaveToken(string tokenString);

        public void DeleteToken();
    }
}
