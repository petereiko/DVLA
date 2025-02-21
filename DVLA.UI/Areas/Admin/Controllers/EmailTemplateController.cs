using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DVLA.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.SYSTEMADMIN)]
    public class EmailTemplateController : Controller
    {
        private readonly IRepositoryQuery<EmailTemplate> _emailTemplateRepositoryQuery;
        private readonly IAuditRepo _AuditRepo;
        private readonly ILogger<EmailTemplateController> _logger;
        public EmailTemplateController(IRepositoryQuery<EmailTemplate> emailTemplateRepositoryQuery, IAuditRepo AuditRepo, ILogger<EmailTemplateController> logger)
        {
            _emailTemplateRepositoryQuery = emailTemplateRepositoryQuery;
            _AuditRepo = AuditRepo;
            _logger = logger;
        }
        // GET: Admin/EmailTemplate
        public ActionResult Index()
        {
            var model = _emailTemplateRepositoryQuery.GetAllAsync().Result.ToList();
            _AuditRepo.AddAudit(Activities.VIEW_EMAIL_TEMPLATE, "View Email Template");
            return View(model);
        }

        public ActionResult Edit(string emailCode)
        {
            if (string.IsNullOrEmpty(emailCode))
            {
                //showMessage = new AlertMessage
                //{
                //    Message = "Invalid Data",
                //    MessageType = MessageType.ErrorMessage
                //};

                //Message = showMessage;
                TempData["ErrorMessage"] = "Invalid Data";

                return RedirectToAction("Index", "EmailTemplate");
            }

            int ID = Convert.ToInt32(Utility.Decrypt(emailCode));


            EmailTemplateDto vwModel = new EmailTemplateDto();

            try
            {

                var emailTemplate = _emailTemplateRepositoryQuery.GetAllAsync().Result.Where(u => u.Id == ID).FirstOrDefault();

                if (emailTemplate == null)
                {
                    //showMessage = new AlertMessage
                    //{
                    //    Message = "Invalid Email Code",
                    //    MessageType = MessageType.ErrorMessage
                    //};

                    //Message = showMessage;
                    TempData["ErrorMessage"] = "Invalid Data";
                }

                vwModel.EmailName = emailTemplate.EmailName;

                vwModel.EmailBody = HttpUtility.HtmlDecode(emailTemplate.EmailBody);

                vwModel.EmailSubject = emailTemplate.EmailSubject;

                vwModel.Id = ID;

                return View(vwModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return View("Error");
            }

        }

        [HttpPost]
        
        public async Task<ActionResult> Update(EmailTemplateDto vwModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var existingEntry = _emailTemplateRepositoryQuery.GetAllAsync().Result.Where(u => u.Id == vwModel.Id).FirstOrDefault();


                    if (existingEntry != null)
                    {
                        existingEntry.EmailBody = HttpUtility.HtmlEncode(vwModel.EmailBody);

                        existingEntry.EmailName = vwModel.EmailName;

                        existingEntry.EmailSubject = vwModel.EmailSubject;

                        await _emailTemplateRepositoryQuery.UpdateAsync(existingEntry);

                        var mDesc = "Email Template Updated Successfully";

                        //showMessage = new AlertMessage
                        //{
                        //    Message = mDesc,
                        //    MessageType = MessageType.SuccessMessage
                        //};

                        //Message = showMessage;
                        TempData["SuccessMessage"] = "Email Template Updated Successfully";

                        _AuditRepo.AddAudit(Activities.UPDATE_EMAIL_TEMPLATE, "Update Email Template");

                        return RedirectToAction("Index", "EmailTemplate");
                    }

                    return RedirectToAction("Index", "EmailTemplate");
                }
                else
                {
                    ViewBag.StatusCode = 2;
                    vwModel.Errors.AddRange(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));

                    return View(vwModel);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

        }

    }
}