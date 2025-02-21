using DVLA.Business.NotificationModule;
using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using DVLA.UI.Areas.Customer.Controllers;
using DVLA.UI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DVLA.UI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisualAssessmentController : ControllerBase
    {
        private readonly IRepositoryQuery<VisualAcuityScore> _visualAcuityScoreRepositoryQuery;
        private readonly IRepositoryQuery<VisualFieldScore> _visualFieldScoreRepositoryQuery;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmQuery;
        private IVisualAssessmentResultRepository _visualAssessmentResultRepository;
        public VisualAssessmentController(IRepositoryQuery<VisualAcuityScore> visualAcuityScoreRepositoryQuery, IRepositoryQuery<VisualFieldScore> visualFieldScoreRepositoryQuery, IVisualAssessmentResultRepository visualAssessmentResultRepository, IRepositoryQuery<OptometristFirm> optometristFirmQuery)
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
            
            var resultServiceTypes = Enum.GetValues(typeof(ResultServiceType))
                           .Cast<ResultServiceType>()
                           .Select(e => new IdNameModel<int>
                           {
                               Name = e.ToString(),
                               Id = (int)(object)e
                           })
                           .ToList(); 
            var learnerDriversLicenceTypes = Enum.GetValues(typeof(LearnerDriversLicenceType))
                           .Cast<LearnerDriversLicenceType>()
                           .Select(e => new IdNameModel<int>
                           {
                               Name = e.ToString(),
                               Id = (int)(object)e
                           })
                           .ToList();

            var passOrFail = Enum.GetValues(typeof(PassOrFail))
                           .Cast<PassOrFail>()
                           .Select(e => new IdNameModel<int>
                           {
                               Name = e.ToString(),
                               Id = (int)(object)e
                           })
                           .ToList();

            CreateVisualAssessmentResultDependencyModel model = new CreateVisualAssessmentResultDependencyModel
            {
                ColourVisionScores = colourVisionScores.Select(x => new Data.Models.DataObjects.UtilityObjects.IdNameModel<long>
                {
                    Id = x.Id,
                    Name = x.Value.ToString()
                }).ToList(),
                ResultConclusions = resultConclusion.Select(x => new Data.Models.DataObjects.UtilityObjects.IdNameModel<string>
                {
                    Id = x.Value,
                    Name = x.Value
                }).ToList(),
                ResultServiceTypes = resultServiceTypes,
                SingleImage = singleImage.Select(x => new Data.Models.DataObjects.UtilityObjects.IdNameModel<long>
                {
                    Id = x.Id,
                    Name = x.Score
                }).ToList(),
                VisualAcuity = visualAcuitys.Select(x => new Data.Models.DataObjects.UtilityObjects.IdNameModel<long>
                {
                    Id = x.Id,
                    Name = x.Score
                }).ToList(),
                VisualFieldScores = visualFieldScores.Select(x => new Data.Models.DataObjects.UtilityObjects.IdNameModel<long>
                {
                    Id = x.Id,
                    Name = x.Score
                }).ToList(),
                LearnerDriversLicenceType = learnerDriversLicenceTypes,
                PassOrFail = passOrFail
            };

            //ViewBag.VisualAcuity = new SelectList(visualAcuitys, "Score", "Score");
            //ViewBag.VisualFieldScores = new SelectList(visualFieldScores, "Score", "Score");
            //ViewBag.ColourVisionScores = new SelectList(colourVisionScores, "Id", "Value");
            //ViewBag.SingleImage = new SelectList(visualAcuitys.Where(x => x.Id > 4), "Score", "Score");
            //ViewBag.ResultConclusions = new SelectList(_visualAssessmentResultRepository.ResultConclusion(), "Value", "Text");
            return Ok(model);
        }


        [HttpGet("get-all-optometristfirms")]
        public async Task<IActionResult> GetAllOptometristFirms()
        {
            var query = await _optometristFirmQuery.GetAllIncludeAsync(x => x.District, y => y.Region);
            var firms = query.Select(x => new OptometristFirmViewModel
            {
                AccreditationNumber = x.AccreditationNumber,
                BusinessAddress = x.BusinessAddress,
                BusinessName = x.BusinessName,
                CentreCode = x.CentreCode,
                ContactEmail = x.ContactEmail,
                ContactFirstName = x.ContactFirstName,
                ContactLastName = x.ContactLastName,
                ContactPhoneNumber = x.ContactPhoneNumber,
                CreatedDate = x.CreatedDate,
                DigitalAddress = x.DigitalAddress,
                DistrictId = x.DistrictId,
                DistrictName = x.District.Name,
                Id = x.Id,
                IsActive = x.IsActive,
                IsSynchronized = x.IsSynchronized,
                MobileNumber = x.MobileNumber,
                RegionId = x.RegionId,
                RegionName = x.Region.Name,
                RegistrationNumber = x.RegistrationNumber,
                ReorderLevel = x.ReorderLevel,
                TelephoneNumber = x.TelephoneNumber,
                Town = x.Town
            }).ToList();
            return Ok(firms); 
        }

        [HttpGet("get-optometristfirm-by-id/{id}")]
        public async Task<IActionResult> GetOptometristFirm([FromRoute]long id)
        {
            var query = await _optometristFirmQuery.GetAllIncludeAsync(x => x.District, y => y.Region);
            var firm = query.Select(x => new OptometristFirmViewModel
            {
                AccreditationNumber = x.AccreditationNumber,
                BusinessAddress = x.BusinessAddress,
                BusinessName = x.BusinessName,
                CentreCode = x.CentreCode,
                ContactEmail = x.ContactEmail,
                ContactFirstName = x.ContactFirstName,
                ContactLastName = x.ContactLastName,
                ContactPhoneNumber = x.ContactPhoneNumber,
                CreatedDate = x.CreatedDate,
                DigitalAddress = x.DigitalAddress,
                DistrictId = x.DistrictId,
                DistrictName = x.District.Name,
                Id = x.Id,
                IsActive = x.IsActive,
                IsSynchronized = x.IsSynchronized,
                MobileNumber = x.MobileNumber,
                RegionId = x.RegionId,
                RegionName = x.Region.Name,
                RegistrationNumber = x.RegistrationNumber,
                ReorderLevel = x.ReorderLevel,
                TelephoneNumber = x.TelephoneNumber,
                Town = x.Town
            }).FirstOrDefault(x => x.Id == id);
            return Ok(firm);
        }

        [HttpPost("transmit")]
        public async Task<IActionResult> Transmit([FromBody] VisualAssessmentTransmissionModel model)
        {
            MessageResponse result = await _visualAssessmentResultRepository.Transmit(model);
            return Ok(result);
        }

        [HttpPost("bulk-transmit")]
        public async Task<IActionResult> Transmit([FromBody] List<VisualAssessmentTransmissionModel> model)
        {
            MessageResponse result = await _visualAssessmentResultRepository.LogBulkTransmission(model);
            return Ok(result);
        }
    }
}
