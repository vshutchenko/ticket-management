using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.UserApi.Models;
using TicketManagement.UserApi.Services.Interfaces;

namespace TicketManagement.UserApi.Controllers
{
    [Route("users")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly ITokenService _tokenService;

        public UserController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userService.FindByEmailAsync(model.Email!);

            await _userService.CheckPasswordAsync(user.Email, model.Password!);

            var userRoles = await _userService.GetRolesAsync(user.Id);

            var token = _tokenService.GetToken(user, userRoles);

            return Ok(token);
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new UserModel
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CultureName = model.CultureName,
                TimeZoneId = model.TimeZoneId,
            };

            await _userService.CreateAsync(user, model.Password!);

            var createdUser = await _userService.FindByEmailAsync(user.Email!);

            var roles = await _userService.GetRolesAsync(createdUser.Id);

            return Ok(_tokenService.GetToken(createdUser, roles));
        }

        [HttpGet("validate")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult Validate(string token)
        {
            return _tokenService.ValidateToken(token) ? Ok() : Forbid();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.FindByIdAsync(id);

            return Ok(user);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel user)
        {
            await _userService.UpdateAsync(user);

            var updatedUser = await _userService.FindByEmailAsync(user.Email);

            var roles = await _userService.GetRolesAsync(updatedUser.Id);

            return Ok(_tokenService.GetToken(updatedUser, roles));
        }

        [HttpPut("{id}/password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] PasswordModel model)
        {
            await _userService.ChangePasswordAsync(id, model.CurrentPassword!, model.NewPassword!);

            var roles = await _userService.GetRolesAsync(id);

            var user = await _userService.FindByIdAsync(id);

            return Ok(_tokenService.GetToken(user, roles));
        }
    }
}