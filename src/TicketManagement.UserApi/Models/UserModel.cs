using Microsoft.AspNetCore.Identity;

namespace TicketManagement.UserApi.Models
{
    public class UserModel : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? CultureName { get; set; }

        public decimal Balance { get; set; }

        public string? TimeZoneId { get; set; }
    }
}
