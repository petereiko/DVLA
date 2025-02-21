using DVLA.Data;
using DVLA.Data.Models.DataObjects.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace DVLA.UI.Areas.Admin.Controllers
{
    [Authorize(Roles = AppRoles.SYSTEMADMIN)]
    public class PassportConverterController : Controller
    {
        private readonly DVLADbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<PassportConverterController> _logger;

        public PassportConverterController(DVLADbContext context, IWebHostEnvironment environment, ILogger<PassportConverterController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        // GET: Admin/PassportConverter
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(VisualAssessmentResultViewModel model)
        {
            try
            {
                var count = 3000;// context.VisualAssessmentResults.Where(x => x.PassportImageUrl.Contains(".png") == false && x.PassportImageUrl != null && x.PassportImageUrl != "").Count();

                int iteration = 1;
                while (count > (iteration - 1) * 1000)
                {
                    var visualAssessmentResult = _context.VisualAssessmentResults.Where(x => x.PassportImageUrl.Contains(".png") == false && x.PassportImageUrl != null && x.PassportImageUrl != "").OrderBy(x => x.Id).Skip((iteration - 1) * 1000).Take(1000).AsEnumerable();
                    foreach (var item in visualAssessmentResult)
                    {
                        if (!string.IsNullOrEmpty(item.PassportImageUrl) && !item.PassportImageUrl.Contains(".png"))
                        {
                            string base64 = item.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);
                            byte[] imageBytes = Convert.FromBase64String(base64);

                            string filename = Guid.NewGuid().ToString() + ".png";
                            var path = Path.Combine(_environment.ContentRootPath, "Passports", filename);
                            System.IO.File.WriteAllBytes(path, imageBytes);
                            item.PassportImageUrl = filename;
                        }


                        //_visualAssessmentResultRepositoryCommand.Update(item);
                        //_visualAssessmentResultRepositoryCommand.SaveChanges();
                    }

                    _context.SaveChanges();
                    iteration += 1;
                }

                TempData["SuccessMessage"] = "Record saved successfully";
               // AddAudit(Activities.CREATE_VISUAL_ASSESSMENT_RESULT, "Create Visual Assessment Result");


                return RedirectToAction("Index");


            }
            catch (Exception ex)
            {
                model.Errors.Add(ex.Message);
                _logger.LogError(ex.Message, ex);
                                                                                                    }
            return View(model);
        }
    }
}