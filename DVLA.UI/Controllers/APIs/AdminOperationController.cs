using DVLA.Business.Repository;
using DVLA.Data.Models.Auth;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DVLA.UI.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminOperationController : ControllerBase
    {
        private readonly IRepositoryQuery<ApplicationUser> _userRepositoryQuery;
        private readonly IRepositoryQuery<ApplicationRole> _roleRepositoryQuery;
        private readonly IRepositoryQuery<ApplicationUserRole> _userRoleRepositoryQuery;
        private readonly IRepositoryQuery<VisualAcuityScore> _visualAcuityRepositoryQuery;
        private readonly IRepositoryQuery<VisualFieldScore> _visualFieldScoreRepositoryQuery;
        private readonly IRepositoryQuery<ColourVisionScore> _colourVisionScoreRepositoryQuery;
        private readonly IRepositoryQuery<Region> _regionRepositoryQuery;
        private readonly IRepositoryQuery<District> _districtRepositoryQuery;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmRepositoryQuery;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristFirmUserRepositoryQuery;


        public AdminOperationController(IRepositoryQuery<ApplicationUser> userRepositoryQuery, IRepositoryQuery<ApplicationRole> roleRepositoryQuery, IRepositoryQuery<ApplicationUserRole> userRoleRepositoryQuery, IRepositoryQuery<VisualAcuityScore> visualAcuityRepositoryQuery, IRepositoryQuery<VisualFieldScore> visualFieldScoreRepositoryQuery, IRepositoryQuery<ColourVisionScore> colourVisionScoreRepositoryQuery, IRepositoryQuery<Region> regionRepositoryQuery, IRepositoryQuery<District> districtRepositoryQuery, IRepositoryQuery<OptometristFirm> optometristFirmRepositoryQuery, IRepositoryQuery<OptometristFirmUser> optometristFirmUserRepositoryQuery)
        {
            _userRepositoryQuery = userRepositoryQuery;
            _roleRepositoryQuery = roleRepositoryQuery;
            _userRoleRepositoryQuery = userRoleRepositoryQuery;
            _visualAcuityRepositoryQuery = visualAcuityRepositoryQuery;
            _visualFieldScoreRepositoryQuery = visualFieldScoreRepositoryQuery;
            _colourVisionScoreRepositoryQuery = colourVisionScoreRepositoryQuery;
            _regionRepositoryQuery = regionRepositoryQuery;
            _districtRepositoryQuery = districtRepositoryQuery;
            _optometristFirmRepositoryQuery = optometristFirmRepositoryQuery;
            _optometristFirmUserRepositoryQuery = optometristFirmUserRepositoryQuery;
        }

        [HttpGet("fetchusers")]
        public async Task<IActionResult> FetchUsers()
        {
            var query = await _userRepositoryQuery.GetAllAsync();
            return Ok(query);
        }

        [HttpGet("fetchroles")]
        public async Task<IActionResult> FetchRoles()
        {
            var query = await _roleRepositoryQuery.GetAllAsync();
            return Ok(query);
        }

        [HttpGet("fetchuserroles")]
        public async Task<IActionResult> FetchUserRoles()
        {
            var query = await _userRoleRepositoryQuery.GetAllAsync();
            return Ok(query);
        }

        [HttpGet("colorvisionscores")]
        public async Task<IActionResult> FetchColorVisionScores()
        {
            var query = await _colourVisionScoreRepositoryQuery.GetAllAsync();
            return Ok(query);
        }

        [HttpGet("visualacuityscores")]
        public async Task<IActionResult> FetchVisualAcuityScores()
        {
            var query = await _visualAcuityRepositoryQuery.GetAllAsync();
            return Ok(query);
        }

        [HttpGet("visualfieldscores")]
        public async Task<IActionResult> FetchVisualFieldScores()
        {
            var query = await _visualFieldScoreRepositoryQuery.GetAllAsync();
            return Ok(query);
        }

        [HttpGet("regions")]
        public async Task<IActionResult> FetchRegions()
        {
            var query = await _regionRepositoryQuery.GetAllAsync();
            return Ok(query);
        }

        [HttpGet("districts")]
        public async Task<IActionResult> FetchDistricts()
        {
            var query = await _districtRepositoryQuery.GetAllAsync();
            return Ok(query);
        }

        [HttpGet("optometristfirms")]
        public async Task<IActionResult> FetchOptometristFirms()
        {
            var query = await _optometristFirmRepositoryQuery.GetAllAsync();
            return Ok(query);
        }

        [HttpGet("optometristfirmusers")]
        public async Task<IActionResult> FetchOptometristFirmUser()
        {
            var query = await _optometristFirmUserRepositoryQuery.GetAllAsync();
            return Ok(query);
        }
    }
}
