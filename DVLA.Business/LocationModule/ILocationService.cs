using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.LocationModule
{
    public interface ILocationService
    {
        Task<List<SelectListItem>> GetAllRegions();
        Task<List<SelectListItem>> GetAllDistricts();
        Task<List<SelectListItem>> GetDistrictsByRegion(int regionId);
    }
}
