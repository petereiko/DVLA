using DVLA.Business.Repository;
using DVLA.Data.Models.Auth;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DVLA.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
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

        public AdminOperationController(
            IRepositoryQuery<ApplicationUser> userRepositoryQuery,
            IRepositoryQuery<ApplicationRole> roleRepositoryQuery,
            IRepositoryQuery<ApplicationUserRole> userRoleRepositoryQuery,
            IRepositoryQuery<VisualAcuityScore> visualAcuityRepositoryQuery,
            IRepositoryQuery<VisualFieldScore> visualFieldScoreRepositoryQuery,
            IRepositoryQuery<ColourVisionScore> colourVisionScoreRepositoryQuery,
            IRepositoryQuery<Region> regionRepositoryQuery,
            IRepositoryQuery<District> districtRepositoryQuery,
            IRepositoryQuery<OptometristFirm> optometristFirmRepositoryQuery,
            IRepositoryQuery<OptometristFirmUser> optometristFirmUserRepositoryQuery)
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
        public async Task<IActionResult> FetchUsers() => Ok(await _userRepositoryQuery.GetAllAsync());

        [HttpGet("fetchroles")]
        public async Task<IActionResult> FetchRoles() => Ok(await _roleRepositoryQuery.GetAllAsync());

        [HttpGet("fetchuserroles")]
        public async Task<IActionResult> FetchUserRoles() => Ok(await _userRoleRepositoryQuery.GetAllAsync());

        [HttpGet("colorvisionscores")]
        public async Task<IActionResult> FetchColorVisionScores() => Ok(await _colourVisionScoreRepositoryQuery.GetAllAsync());

        [HttpGet("visualacuityscores")]
        public async Task<IActionResult> FetchVisualAcuityScores() => Ok(await _visualAcuityRepositoryQuery.GetAllAsync());

        [HttpGet("visualfieldscores")]
        public async Task<IActionResult> FetchVisualFieldScores() => Ok(await _visualFieldScoreRepositoryQuery.GetAllAsync());

        [HttpGet("regions")]
        public async Task<IActionResult> FetchRegions() => Ok(await _regionRepositoryQuery.GetAllAsync());

        [HttpGet("districts")]
        public async Task<IActionResult> FetchDistricts() => Ok(await _districtRepositoryQuery.GetAllAsync());

        [HttpGet("optometristfirms")]
        public async Task<IActionResult> FetchOptometristFirms() => Ok(await _optometristFirmRepositoryQuery.GetAllAsync());

        [HttpGet("optometristfirmusers")]
        public async Task<IActionResult> FetchOptometristFirmUser() => Ok(await _optometristFirmUserRepositoryQuery.GetAllAsync());
    }
}
