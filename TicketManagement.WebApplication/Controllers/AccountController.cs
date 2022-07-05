using System.Globalization;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.WebApplication.Models.Account;

namespace TicketManagement.WebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly IOptions<RequestLocalizationOptions> _locOptions;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public AccountController(IIdentityService identityService, IMapper mapper, IOptions<RequestLocalizationOptions> locOptions)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _locOptions = locOptions ?? throw new ArgumentNullException(nameof(locOptions));
        }

        [HttpGet]
        public async Task<IActionResult> AddFunds()
        {
            var userId = User.FindFirstValue("id");

            var user = await _identityService.GetUserAsync(userId);

            ViewBag.Balance = user.Balance;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFunds(decimal amount)
        {
            var userId = User.FindFirstValue("id");

            var user = await _identityService.GetUserAsync(userId);

            user.Balance += amount;

            await _identityService.UpdateUser(user);

            return RedirectToAction("AddFunds");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userId = User.FindFirstValue("id");

            await _identityService.ChangePassword(userId, model.CurrentPassword, model.NewPassword);

            ViewBag.Message = "Profile information was successfully updated!";

            return View("Success");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            var cultures = _locOptions.Value.SupportedCultures?
                .Select(c => new SelectListItem(c.DisplayName, c.Name))
                .ToList();

            cultures ??= new List<SelectListItem>
            {
                new SelectListItem { Text = CultureInfo.CurrentCulture.DisplayName.ToString(), Value = CultureInfo.CurrentCulture.Name },
            };

            var timeZones = TimeZoneInfo
                .GetSystemTimeZones()
                .Select(z => new SelectListItem(z.DisplayName, z.Id))
                .ToList();

            var user = await _identityService.GetUserAsync(userId);

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CultureName = user.CultureName,
                TimeZoneId = user.TimeZoneId,
                Balance = user.Balance,
                Cultures = cultures,
                TimeZones = timeZones,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = _mapper.Map<UserModel>(model);

            await _identityService.UpdateUser(user);

            SetCultureCookie(model.CultureName);
            await SetClaimsAsync(user.Id);

            TempData["Message"] = "Profile information was successfully updated!";

            return RedirectToAction("EditUser", new { userId = model.Id });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            SetCultureCookie("en-US");

            return RedirectToAction("Index", "Home");
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
            await AuthenticateAsync(model.Email, model.Password);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var cultures = _locOptions.Value.SupportedCultures?
                .Select(c => new SelectListItem(c.DisplayName, c.Name))
                .ToList();

            cultures ??= new List<SelectListItem>
            {
                new SelectListItem { Text = CultureInfo.CurrentCulture.DisplayName.ToString(), Value = CultureInfo.CurrentCulture.Name },
            };

            cultures.First().Selected = true;

            var timeZones = TimeZoneInfo
                .GetSystemTimeZones()
                .Select(z => new SelectListItem(z.DisplayName, z.Id))
                .ToList();

            timeZones.First().Selected = true;

            var viewModel = new RegisterViewModel
            {
                Cultures = cultures,
                TimeZones = timeZones,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var user = _mapper.Map<UserModel>(model);

            await _identityService.CreateAsync(user, model.Password);
            await AuthenticateAsync(model.Email, model.Password);

            return RedirectToAction("Index", "Home");
        }

        private async Task AuthenticateAsync(string email, string password)
        {
            var user = await _identityService.AuthenticateAsync(email, password);

            await SetClaimsAsync(user.Id);

            SetCultureCookie(user.CultureName);
        }

        private async Task SetClaimsAsync(string userId)
        {
            var user = await _identityService.GetUserAsync(userId);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("id", user.Id),
                new Claim("timezoneId", user.TimeZoneId),
                new Claim("culture", user.CultureName),
            };

            var roles = await _identityService.GetRolesAsync(user.Id);

            claims.AddRange(roles
                .Select(r => new Claim(ClaimTypes.Role, r))
                .ToList());

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        private void SetCultureCookie(string culture)
        {
            var cookieOptions = new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddYears(1),
            };

            string timeZone = User.FindFirst("timezoneId") is null ? TimeZoneInfo.Local.Id : User.FindFirst("timezoneId")!.Value;

            Response.Cookies.Append("timezoneId", timeZone, cookieOptions);

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }
    }
}
