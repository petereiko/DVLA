using System.Collections.Generic;
using System.Threading.Tasks;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data.Models.DataObjects.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.UI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class TransmitResultController : Controller
{
    private readonly IVisualAssessmentResultRepository _visualAssessmentResultRepository;

    public TransmitResultController(IVisualAssessmentResultRepository visualAssessmentResultRepository)
    {
        _visualAssessmentResultRepository = visualAssessmentResultRepository;
    }

    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> Index(string term = null)
    {
        List<VisualAssessmentResultItemViewModel> results = await _visualAssessmentResultRepository.FetchResultAsync(term);

        return View(results);
    }
}
