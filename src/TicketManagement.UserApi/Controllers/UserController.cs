using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.DataAccess.Entities;
using TicketManagement.UserApi.Models;
using TicketManagement.UserApi.Services.Interfaces;

namespace TicketManagement.UserApi.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        private readonly ITokenService _tokenService;

        public UserController(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                return NotFound(new { error = "User with such email does not exists." });
            }

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var token = _tokenService.GetToken(user, userRoles);

                return Ok(token);
            }

            return Unauthorized(new { error = "Wrong password." });
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                return BadRequest(new { error = "User already exists." });
            }

            var user = new User
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CultureName = model.CultureName,
                TimeZoneId = model.TimeZoneId,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.User);

                var roles = await _userManager.GetRolesAsync(user);
                return Ok(_tokenService.GetToken(user, roles));
            }

            return BadRequest(new { error = "Cannot create user." });
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
        [Authorize]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                return NotFound(new { error = "User was not found." });
            }

            return Ok(user);
        }

        [HttpPut]
        [Authorize(Roles = "Event manager,User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);

            if (existingUser is null)
            {
                return NotFound(new { error = "User was not found." });
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(user.Email);

            if (userWithSameEmail != null && userWithSameEmail.Id != existingUser.Id)
            {
                return BadRequest(new { error = "This email is already taken." });
            }

            existingUser.Email = user.Email;
            existingUser.UserName = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.TimeZoneId = user.TimeZoneId;
            existingUser.CultureName = user.CultureName;
            existingUser.Balance = user.Balance;

            var result = await _userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
            {
                return BadRequest(new { error = "Cannot update user." });
            }

            var roles = await _userManager.GetRolesAsync(existingUser);

            return Ok(_tokenService.GetToken(existingUser, roles));
        }

        [HttpPut("{id}/password")]
        [Authorize(Roles = "Event manager,User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] PasswordModel model)
        {
            var existingUser = await _userManager.FindByIdAsync(id);

            if (existingUser is null)
            {
                return NotFound(new { error = "User was not found." });
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(existingUser, model.CurrentPassword);

            if (!isValidPassword)
            {
                return BadRequest(new { error = "Not valid current password." });
            }

            var result = await _userManager.ChangePasswordAsync(existingUser, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new { error = "Cannot change password." });
            }

            var roles = await _userManager.GetRolesAsync(existingUser);

            return Ok(_tokenService.GetToken(existingUser, roles));
        }
    }
}