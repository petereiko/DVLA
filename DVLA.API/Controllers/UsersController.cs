using DVLA.Business.UserModule;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;

namespace DVLA.API.Controllers
{
    [Authorize]
    [EnableRateLimiting("AuthenticatedRead")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public UsersController(IUserService userService, IUserRepository userRepository)
        {
            _userService = userService;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationRequestModel model)
        {
            model ??= new PaginationRequestModel();
            return Ok(await _userService.GetUsersAsync(model));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsers());
        }

        [HttpGet("by-role/{roleName}")]
        public async Task<IActionResult> GetUsersInRole(string roleName)
        {
            return Ok(await _userService.GetUsersInRole(roleName));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            return Ok(await _userService.GetUserById(id));
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            return Ok(await _userService.GetUserByEmail(email));
        }

        [HttpGet("details/{id}")]
        public IActionResult GetUserDetails(string id)
        {
            return Ok(_userRepository.GetUserDetails(id));
        }

        [HttpGet("by-optometrist-firm/{optometristFirmId:int}")]
        public IActionResult GetUsersByOptometristFirm(int optometristFirmId)
        {
            return Ok(_userRepository.GetUsersByOptometristFirm(optometristFirmId));
        }

        [HttpPost]
        [EnableRateLimiting("SensitiveWrite")]
        public async Task<IActionResult> Create([FromBody] UserViewModel model)
        {
            var result = await _userService.OnboardUser(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        [EnableRateLimiting("SensitiveWrite")]
        public async Task<IActionResult> Update(string id, [FromBody] UserViewModel model)
        {
            model.Id = id;
            var result = await _userService.EditUser(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/reset-password")]
        [EnableRateLimiting("SensitiveWrite")]
        public async Task<IActionResult> AdminResetPassword(string id, [FromBody] ResetPasswordViewModel model)
        {
            var result = await _userService.AdminResetPasswordAsync(model.Password, id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/change-status")]
        [EnableRateLimiting("SensitiveWrite")]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { success = false, message = "User does not exist." });
            }

            user.IsActive = !user.IsActive;
            var result = await _userService.EditUser(user);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
