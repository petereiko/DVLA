using DVLA.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.LocationModule
{
    public class LocationService : ILocationService
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<LocationService> _logger;
        public LocationService(DVLADbContext context, ILogger<LocationService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<SelectListItem>> GetAllDistricts()
        {
            return await _context.Districts.AsNoTracking().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetAllRegions()
        {
            return await _context.Regions.AsNoTracking().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDistrictsByRegion(int regionId)
        {
            return await _context.Districts.AsNoTracking().Where(x=>x.RegionId==regionId).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDistrictsByRegionWithFacilities(int regionId)
        {
            return await _context.Districts.Include(x=>x.OptometristFirms).AsNoTracking().Where(x => x.RegionId == regionId && x.OptometristFirms.Count>0).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                
            }).ToListAsync();
        }
    }
}
