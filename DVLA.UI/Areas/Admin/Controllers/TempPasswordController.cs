using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DVLA.Business.TempPasswordModule;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace DVLA.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =AppRoles.SYSTEMADMIN)]
    public class TempPasswordController : Controller
    {
        private readonly ITempPasswordService _tempPasswordService;
        private readonly IUserService _userService;

        public TempPasswordController(ITempPasswordService tempPasswordService, IUserService userService)
        {
            _tempPasswordService = tempPasswordService;
            _userService = userService;
        }

        //[HttpGet]
        //public async Task<IActionResult> Create(string userId)
        //{
        //    TempPasswordDto model = new();
        //    model.UserId = userId;
        //    return View(model);
        //}

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            //    if (!ModelState.IsValid) 
            //    {
            //        model.Errors.AddRange(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            //    }
            MessageResponse response = await _tempPasswordService.Create(new() { UserId = userId });
            if (response.Success)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index", "UserManagement");
            }
            TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("Index", "UserManagement");
        }
    }
}
