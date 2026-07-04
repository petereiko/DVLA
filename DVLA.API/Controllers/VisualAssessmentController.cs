using DVLA.API.Models;
using DVLA.Business.Repository;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DVLA.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VisualAssessmentController : ControllerBase
    {
        private readonly IRepositoryQuery<VisualAcuityScore> _visualAcuityScoreRepositoryQuery;
        private readonly IRepositoryQuery<VisualFieldScore> _visualFieldScoreRepositoryQuery;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmQuery;
        private readonly IVisualAssessmentResultRepository _visualAssessmentResultRepository;

        public VisualAssessmentController(
            IRepositoryQuery<VisualAcuityScore> visualAcuityScoreRepositoryQuery,
            IRepositoryQuery<VisualFieldScore> visualFieldScoreRepositoryQuery,
            IVisualAssessmentResultRepository visualAssessmentResultRepository,
            IRepositoryQuery<OptometristFirm> optometristFirmQuery)
        {
            _visualAcuityScoreRepositoryQuery = visualAcuityScoreRepositoryQuery;
            _visualFieldScoreRepositoryQuery = visualFieldScoreRepositoryQuery;
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _optometristFirmQuery = optometristFirmQuery;
        }

        [HttpGet("get-visual-assessment-dependencies")]
        public IActionResult CreateDependencies()
        {
            var visualAcuitys = _visualAcuityScoreRepositoryQuery.Filter(x => x.IsActive).ToList();
            var visualFieldScores = _visualFieldScoreRepositoryQuery.Filter(x => x.IsActive).ToList();
            var colourVisionScores = _visualAssessmentResultRepository.GetColorVisionScores();
            var singleImage = visualAcuitys.Where(x => x.Id > 4).ToList();
            var resultConclusion = _visualAssessmentResultRepository.ResultConclusion();

            return Ok(new CreateVisualAssessmentResultDependencyModel
            {
                ColourVisionScores = colourVisionScores.Select(x => new IdNameModel<long> { Id = x.Id, Name = x.Value.ToString() }).ToList(),
                ResultConclusions = resultConclusion.Select(x => new IdNameModel<string> { Id = x.Value, Name = x.Value }).ToList(),
                ResultServiceTypes = ToIdNameModels<ResultServiceType>(),
                SingleImage = singleImage.Select(x => new IdNameModel<long> { Id = x.Id, Name = x.Score }).ToList(),
                VisualAcuity = visualAcuitys.Select(x => new IdNameModel<long> { Id = x.Id, Name = x.Score }).ToList(),
                VisualFieldScores = visualFieldScores.Select(x => new IdNameModel<long> { Id = x.Id, Name = x.Score }).ToList(),
                LearnerDriversLicenceType = ToIdNameModels<LearnerDriversLicenceType>(),
                PassOrFail = ToIdNameModels<PassOrFail>()
            });
        }

        [HttpGet("get-all-optometristfirms")]
        public async Task<IActionResult> GetAllOptometristFirms()
        {
            var query = await _optometristFirmQuery.GetAllIncludeAsync(x => x.District, y => y.Region);
            return Ok(query.Select(ToViewModel).ToList());
        }

        [HttpGet("get-optometristfirm-by-id/{id}")]
        public async Task<IActionResult> GetOptometristFirm([FromRoute] long id)
        {
            var query = await _optometristFirmQuery.GetAllIncludeAsync(x => x.District, y => y.Region);
            return Ok(query.Select(ToViewModel).FirstOrDefault(x => x.Id == id));
        }

        [HttpPost("transmit")]
        public async Task<IActionResult> Transmit([FromBody] VisualAssessmentTransmissionModel model)
        {
            return Ok(await _visualAssessmentResultRepository.Transmit(model));
        }

        [HttpPost("bulk-transmit")]
        public async Task<IActionResult> Transmit([FromBody] List<VisualAssessmentTransmissionModel> model)
        {
            return Ok(await _visualAssessmentResultRepository.LogBulkTransmission(model));
        }

        private static List<IdNameModel<int>> ToIdNameModels<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new IdNameModel<int> { Name = e.ToString(), Id = Convert.ToInt32(e) })
                .ToList();
        }

        private static OptometristFirmViewModel ToViewModel(OptometristFirm firm)
        {
            return new OptometristFirmViewModel
            {
                AccreditationNumber = firm.AccreditationNumber,
                BusinessAddress = firm.BusinessAddress,
                BusinessName = firm.BusinessName,
                CentreCode = firm.CentreCode,
                ContactEmail = firm.ContactEmail,
                ContactFirstName = firm.ContactFirstName,
                ContactLastName = firm.ContactLastName,
                ContactPhoneNumber = firm.ContactPhoneNumber,
                CreatedDate = firm.CreatedDate,
                DigitalAddress = firm.DigitalAddress,
                DistrictId = firm.DistrictId,
                DistrictName = firm.District?.Name,
                Id = firm.Id,
                IsActive = firm.IsActive,
                IsSynchronized = firm.IsSynchronized,
                MobileNumber = firm.MobileNumber,
                RegionId = firm.RegionId,
                RegionName = firm.Region?.Name,
                RegistrationNumber = firm.RegistrationNumber,
                ReorderLevel = firm.ReorderLevel,
                TelephoneNumber = firm.TelephoneNumber,
                Town = firm.Town
            };
        }
    }
}
