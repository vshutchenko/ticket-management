using System.Globalization;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using TicketManagement.Core.Clients.UserApi;
using TicketManagement.Core.Clients.UserApi.Models;
using TicketManagement.Core.Models;
using TicketManagement.WebApplication.Models.Account;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUserClient _userClient;
        private readonly IOptions<RequestLocalizationOptions> _locOptions;
        private readonly IMapper _mapper;

        public AccountController(
            IUserClient userClient,
            ITokenService tokenService,
            IMapper mapper,
            IOptions<RequestLocalizationOptions> locOptions)
            : base(tokenService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _locOptions = locOptions ?? throw new ArgumentNullException(nameof(locOptions));
            _userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
        }

        [HttpGet]
        [Authorize]
        public IActionResult Logout()
        {
            TokenService.DeleteToken();

            return RedirectToAction(nameof(EventController.Index), TrimController(nameof(EventController)));
        }

        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            var model = _mapper.Map<LoginModel>(viewModel);

            var token = await _userClient.LoginAsync(model);

            TokenService.SaveToken(token);

            return RedirectToAction(nameof(EventController.Index), TrimController(nameof(EventController)));
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
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            var model = _mapper.Map<RegisterModel>(viewModel);

            var token = await _userClient.RegisterAsync(model);

            TokenService.SaveToken(token);

            return RedirectToAction(nameof(EventController.Index), TrimController(nameof(EventController)));
        }

        [HttpGet]
        [Authorize]
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

            var user = await _userClient.GetByIdAsync(userId, TokenService.GetToken());

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
        [Authorize]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = _mapper.Map<UserModel>(model);

            var token = await _userClient.UpdateAsync(user, TokenService.GetToken());

            TokenService.SaveToken(token);

            TempData["Message"] = "Profile information was successfully updated!";

            return RedirectToAction(nameof(EditUser), new { userId = model.Id });
        }

        [HttpGet]
        [AuthorizeRoles(Roles.User)]
        public async Task<IActionResult> AddFunds()
        {
            var userId = User.FindFirstValue("id");

            var user = await _userClient.GetByIdAsync(userId, TokenService.GetToken());

            ViewBag.Balance = user.Balance;

            return View();
        }

        [HttpPost]
        [AuthorizeRoles(Roles.User)]
        public async Task<IActionResult> AddFunds(decimal amount)
        {
            var userId = User.FindFirstValue("id");

            var user = await _userClient.GetByIdAsync(userId, TokenService.GetToken());

            user.Balance += amount;

            var token = await _userClient.UpdateAsync(user, TokenService.GetToken());

            TokenService.SaveToken(token);

            return RedirectToAction(nameof(AddFunds));
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userId = User.FindFirstValue("id");

            var passwordModel = new PasswordModel
            {
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword,
            };

            await _userClient.ChangePasswordAsync(userId, passwordModel, TokenService.GetToken());

            ViewBag.Message = "Profile information was successfully updated!";

            return View("Success");
        }
    }
}
