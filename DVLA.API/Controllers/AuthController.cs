using DVLA.API.Models;
using DVLA.API.Services;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.Auth;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DVLA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DVLADbContext _context;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(
            IUserService userService,
            UserManager<ApplicationUser> userManager,
            DVLADbContext context,
            IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _userManager = userManager;
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public IActionResult TestGet()
        {
            return Ok("success");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [EnableRateLimiting("Auth")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new { success = false, message = "Email and password are required." });
            }

            var authResult = await _userService.Authenticate(model);
            if (!authResult.Success)
            {
                return Unauthorized(authResult);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { success = false, message = "Invalid Email/Password" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var optometristFirmUser = await _context.OptometristFirmUsers
                .AsNoTracking()
                .Include(x => x.OptometristFirm)
                .FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);

            var accessToken = _jwtTokenService.CreateToken(user, roles, optometristFirmUser, out var expiresAtUtc);

            return Ok(new
            {
                success = true,
                message = authResult.Message,
                result = new AuthResponse
                {
                    AccessToken = accessToken,
                    ExpiresAtUtc = expiresAtUtc,
                    User = authResult.Result
                }
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [EnableRateLimiting("Auth")]
        public async Task<IActionResult> Register([FromBody] UserViewModel model)
        {
            if (model == null)
            {
                return BadRequest(new { success = false, message = "User details are required." });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _userService.OnboardUser(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        [EnableRateLimiting("Auth")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string encodedToken, [FromQuery] string userid)
        {
            if (string.IsNullOrWhiteSpace(encodedToken) || string.IsNullOrWhiteSpace(userid))
            {
                return BadRequest(new { success = false, message = "Token and user id are required." });
            }

            var result = await _userService.ConfirmEmail(encodedToken, userid);
            if (!result)
            {
                return BadRequest(new { success = false, message = "Email confirmation failed." });
            }

            return Ok(new { success = true, message = "Your account has been successfully activated." });
        }

        [Authorize]
        [HttpPost("change-password")]
        [EnableRateLimiting("SensitiveWrite")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (model == null)
            {
                return BadRequest(new { success = false, message = "Change password details are required." });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _userService.ChangePasswordAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok(await _userService.Logout());
        }

        [Authorize]
        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            return Ok(_userService.GetRoles());
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [EnableRateLimiting("Auth")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest(new { success = false, message = "Email is required." });
            }

            var result = await _userService.SendResetPasswordToken(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        [EnableRateLimiting("Auth")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (model == null)
            {
                return BadRequest(new { success = false, message = "Reset password details are required." });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (string.IsNullOrWhiteSpace(model.Id) || string.IsNullOrWhiteSpace(model.ResetToken))
            {
                return BadRequest(new { success = false, message = "User id and reset token are required." });
            }

            var result = await _userService.ResetPassword(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new CurrentUserResponse
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = User.FindFirstValue(ClaimTypes.Email),
                FullName = User.FindFirstValue("FullName"),
                OptometristFirmId = User.FindFirstValue("OptometristFirmId"),
                Roles = User.FindAll(ClaimTypes.Role).Select(x => x.Value)
            });
        }
    }
}
