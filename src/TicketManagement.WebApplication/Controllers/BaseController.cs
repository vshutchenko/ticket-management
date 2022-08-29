using Microsoft.AspNetCore.Mvc;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    public class BaseController : Controller
    {
        protected BaseController(ITokenService tokenService)
        {
            TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        protected ITokenService TokenService { get; }

        protected string TrimController(string value)
        {
            string result = value.Replace("Controller", "");

            return result;
        }
    }
}
