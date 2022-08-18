namespace TicketManagement.WebApplication.Services
{
    internal class TokenService : ITokenService
    {
        private readonly string _scheme = "Bearer";

        private string _token = string.Empty;

        public string Scheme => _scheme;

        public void DeleteToken() => _token = string.Empty;

        public string GetToken() => _token;

        public void SaveToken(string token)
        {
            _token = token ?? throw new ArgumentNullException(nameof(token));
        }
    }
}
