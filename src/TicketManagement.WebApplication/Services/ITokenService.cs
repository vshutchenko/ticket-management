namespace TicketManagement.WebApplication.Services
{
    public interface ITokenService
    {
        string Scheme { get; }

        public string GetToken();

        public void SaveToken(string token);

        public void DeleteToken();
    }
}
