using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.WebApplication.Extensions;
using TicketManagement.WebApplication.Models.Account;

namespace TicketManagement.WebApplication.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<RequestLocalizationOptions> _locOptions;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AccountController(HttpClient httpClient, IConfiguration configuration, IMapper mapper, IOptions<RequestLocalizationOptions> locOptions)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _locOptions = locOptions ?? throw new ArgumentNullException(nameof(locOptions));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri(configuration["UserApi:BaseAddress"]);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Token");

            SetCookies("en-US", TimeZoneInfo.Local.Id);

            return RedirectToAction("Index", "Event");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/User/Login", model, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            var token = await response.Content.ReadAsStringAsync();

            HttpContext.Session.SetString("Token", token);

            return RedirectToAction("Index", "Event");
        }

        private void SetCookies(string culture, string timeZone)
        {
            var cookieOptions = new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddYears(1),
            };

            Response.Cookies.Append("timezoneId", timeZone, cookieOptions);

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }
    }
}
