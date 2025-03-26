using DVLA.Data;
using DVLA.Data.Models.DataObjects.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.LocationModule
{
    public class LocationService : ILocationService
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<LocationService> _logger;
        private readonly IHostingEnvironment _environment;
        public LocationService(DVLADbContext context, ILogger<LocationService> logger, IHostingEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }
        public async Task<List<SelectListItem>> GetAllDistricts()
        {
            return await _context.Districts.AsNoTracking().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToListAsync();
        }

        public List<SelectListItem> GetCountries()
        {
            string file = Path.Combine(_environment.WebRootPath, "AppFile", "Resources", "Countries.txt");
            string content = File.ReadAllText(file);
            List<CountryViewModel> countries = JsonConvert.DeserializeObject<List<CountryViewModel>>(content);
            return countries.Select(x => new SelectListItem
            {
                Text = x.name,
                Value = x.name
            }).ToList();
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
