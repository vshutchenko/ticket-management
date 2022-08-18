using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using TicketManagement.WebApplication.Clients.UserApi;
using TicketManagement.WebApplication.Clients.UserApi.Models;
using TicketManagement.WebApplication.Models.Account;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserClient _userClient;
        private readonly IOptions<RequestLocalizationOptions> _locOptions;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AccountController(IUserClient userClient, ITokenService tokenService, IMapper mapper, IOptions<RequestLocalizationOptions> locOptions)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _locOptions = locOptions ?? throw new ArgumentNullException(nameof(locOptions));
            _userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
        }

        public IActionResult Logout()
        {
            _tokenService.DeleteToken();

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
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            var model = _mapper.Map<LoginModel>(viewModel);

            var token = await _userClient.LoginAsync(model);

            _tokenService.SaveToken(token);

            return RedirectToAction("Index", "Event");
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
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            var model = _mapper.Map<RegisterModel>(viewModel);

            var token = await _userClient.RegisterAsync(model);

            _tokenService.SaveToken(token);

            return RedirectToAction("Index", "Event");
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

            var user = await _userClient.GetByIdAsync(userId, _tokenService.GetToken());

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                Email = user.Email,
                CultureName = user.CultureName!,
                TimeZoneId = user.TimeZoneId!,
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

            var token = await _userClient.UpdateAsync(user, _tokenService.GetToken());

            SetCookies(model.CultureName, model.TimeZoneId);

            _tokenService.SaveToken(token);

            TempData["Message"] = "Profile information was successfully updated!";

            return RedirectToAction("EditUser", new { userId = model.Id });
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddFunds()
        {
            var userId = User.FindFirstValue("id");

            var user = await _userClient.GetByIdAsync(userId, _tokenService.GetToken());

            ViewBag.Balance = user.Balance;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFunds(decimal amount)
        {
            var userId = User.FindFirstValue("id");

            var user = await _userClient.GetByIdAsync(userId, _tokenService.GetToken());

            user.Balance += amount;

            var token = await _userClient.UpdateAsync(user, _tokenService.GetToken());

            _tokenService.SaveToken(token);

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

            var passwordModel = new PasswordModel
            {
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword,
            };

            await _userClient.ChangePassword(userId, passwordModel, _tokenService.GetToken());

            ViewBag.Message = "Profile information was successfully updated!";

            return View("Success");
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
