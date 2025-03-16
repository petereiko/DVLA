using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data.Models.DataObjects.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System;
using System.Threading.Tasks;
using DVLA.Business.Repository;
using DVLA.DATA.Domains;
using System.Linq;
using DVLA.Business.NotificationModule;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.UI.Areas.Customer.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using DVLA.Data.Models.DataObjects.DTOs;

namespace DVLA.UI.Components
{
    public class ResultViewComponent : ViewComponent
    {
        private readonly IRepositoryQuery<VisualAssessmentResult> _visualAssessmentResultRepositoryQuery;
        private readonly IRepositoryQuery<VisualAcuityScore> _visualAcuityScoreRepositoryQuery;
        private readonly IRepositoryQuery<VisualFieldScore> _visualFieldScoreRepositoryQuery;
        private IVisualAssessmentResultRepository _visualAssessmentResultRepository;
        private readonly ILogger<ResultViewComponent> _logger;
        private readonly IWebHostEnvironment _environment;

        public ResultViewComponent(IRepositoryQuery<VisualAssessmentResult> visualAssessmentResultRepositoryQuery,
            IVisualAssessmentResultRepository visualAssessmentResultRepository,
            IRepositoryQuery<VisualAcuityScore> visualAcuityScoreRepositoryQuery,
            IRepositoryQuery<VisualFieldScore> visualFieldScoreRepositoryQuery, ILogger<ResultViewComponent> logger, IWebHostEnvironment environment)
        {
            _visualAcuityScoreRepositoryQuery = visualAcuityScoreRepositoryQuery;
            _visualFieldScoreRepositoryQuery = visualFieldScoreRepositoryQuery;
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _visualAssessmentResultRepositoryQuery = visualAssessmentResultRepositoryQuery;
            _logger = logger;
            _environment = environment;
        }


        public async Task<IViewComponentResult> InvokeAsync(string token)
        {
            VisualAssessmentResultModel model = new();
            try
            {
                var visualAcuitys = _visualAcuityScoreRepositoryQuery.Filter(x => x.IsActive).ToList();
                var visualFieldScores = _visualFieldScoreRepositoryQuery.FilterAsync(x => x.IsActive).Result.ToList();
                var colourVisionScores = _visualAssessmentResultRepository.GetColorVisionScores();

                ViewBag.VisualAcuity = new SelectList(visualAcuitys, "Score", "Score");
                ViewBag.VisualFieldScores = new SelectList(visualFieldScores, "Score", "Score");
                ViewBag.ColourVisionScores = new SelectList(colourVisionScores, "Id", "Value");
                ViewBag.SingleImage = new SelectList(visualAcuitys.Where(x => x.Id > 4), "Score", "Score");
                ViewBag.ResultConclusions = new SelectList(_visualAssessmentResultRepository.ResultConclusion(), "Value", "Text");

                long id = 0;



                id = long.Parse(Utility.Decrypt(token));
                var result = await _visualAssessmentResultRepositoryQuery.GetByIdAsync(id);
                if (result != null)
                {

                    model = _visualAssessmentResultRepository.FetchAssessmentResult(result.ReferenceNumber);
                    if (model != null)
                    {
                        if (!string.IsNullOrEmpty(model.PassportImageUrl))
                        {
                            if (model.PassportImageUrl.Contains(".png"))
                            {
                                var path = Path.Combine(_environment.ContentRootPath, "wwwroot", "Passports", model.PassportImageUrl);

                                byte[] imageArray = File.ReadAllBytes(path);
                                model.PassportImageUrl = Convert.ToBase64String(imageArray);

                            }
                        }
                    }
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return View(model);
        }
    }
}
