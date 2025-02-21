using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Web;

namespace DVLA.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.SYSTEMADMIN)]
    public class SmsTemplateController : Controller
    {
        private readonly IRepositoryQuery<SmsTemplate> _smsTemplateRepositoryQuery;
        private readonly IAuditRepo _AuditRepo;
        private readonly ILogger<SmsTemplateController> _logger;
        public SmsTemplateController(IRepositoryQuery<SmsTemplate> smsTemplateRepositoryQuery, IAuditRepo AuditRepo, ILogger<SmsTemplateController> logger)
        {
            _smsTemplateRepositoryQuery = smsTemplateRepositoryQuery;
            _AuditRepo = AuditRepo;
            _logger = logger;
        }
        // GET: Admin/EmailTemplate
        public ActionResult Index()
        {
            //Session["Module"] = "SETUP";
            //ViewBag.Controller = HttpContext.Request.RequestContext.RouteData.Values["controller"];
            //ViewBag.Action = HttpContext.Request.RequestContext.RouteData.Values["action"];
            //ViewBag.Id = HttpContext.Request.RequestContext.RouteData.Values["id"];

            var model = _smsTemplateRepositoryQuery.GetAll().ToList();
            _AuditRepo.AddAudit(Activities.VIEW_SMS_TEMPLATE, "View Sms Template");
            return View(model);
        }

        public ActionResult Edit(string smsCode)
        {
            if (string.IsNullOrEmpty(smsCode))
            {
                TempData["ErrorMessage"] = "Invalid Data";
                return RedirectToAction("Index", "SmsTemplate");
            }

            int ID = Convert.ToInt32(Utility.Decrypt(smsCode));


            SmsTemplateDto vwModel = new SmsTemplateDto();

            try
            {

                var smsTemplate = _smsTemplateRepositoryQuery.GetAll().Where(u => u.Id == ID).FirstOrDefault();

                if (smsTemplate == null)
                {
                    vwModel.Errors.Add("Invalid Sms Code");
                   return View(vwModel);
                }

                vwModel.Name = smsTemplate.Name;

                vwModel.Body = HttpUtility.HtmlDecode(smsTemplate.Body);

                vwModel.Subject = smsTemplate.Subject;

                vwModel.Id = ID;

                return View(vwModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                vwModel.Errors.Add($"{ex.Message}");
                return View(vwModel);
            }

        }

        [HttpPost]
        // 
        public ActionResult Update(SmsTemplateDto vwModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var existingEntry = _smsTemplateRepositoryQuery.GetAll().Where(u => u.Id == vwModel.Id).FirstOrDefault();


                    if (existingEntry != null)
                    {
                        existingEntry.Body = HttpUtility.HtmlEncode(vwModel.Body);

                        existingEntry.Name = vwModel.Name;

                        existingEntry.Subject = vwModel.Subject;

                        _smsTemplateRepositoryQuery.Update(existingEntry);

                        var mDesc = "Sms Template Updated Successfully";

                        TempData["SuccessMessage"] = mDesc;

                        _AuditRepo.AddAudit(Activities.UPDATE_SMS_TEMPLATE, "Update Sms Template");

                        return RedirectToAction("Index", "SmsTemplate");
                    }

                    return RedirectToAction("Index", "SmsTemplate");
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
                vwModel.Errors.Add(ex.Message);
                return View(vwModel);
            }
        }
    }
}