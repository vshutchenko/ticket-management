using Microsoft.AspNetCore.Authorization;

namespace TicketManagement.Core.Models
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles)
            : base()
        {
            Roles = string.Join(",", roles);
        }
    }
}
