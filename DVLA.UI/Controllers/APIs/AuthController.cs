using DVLA.Business.UserModule;
using DVLA.Data.Models.DataObjects.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DVLA.UI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("test")]
        public IActionResult TestGet()
        {
            return Ok("success");
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]LoginViewModel model)
        {
            var result = await _userService.Authenticate(model);
            return Ok(result); 
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordViewModel model)
        {
            var result = await _userService.ResetPassword(model);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordToken([FromBody] ForgotPasswordViewModel model)
        {
            var result = await _userService.SendResetPasswordToken(model);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            var result = _userService.GetRoles();
            return Ok(result);
        }
    }
}
