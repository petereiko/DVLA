using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.DATA.Domains;
using DVLA.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DVLA.UI.Areas.Registration.Controllers
{
    [Area("Registration")]
    [AllowAnonymous]
    public class FacilityVerificationController : Controller
    {
        private readonly IAuditRepo _AuditRepo;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmQuery;
        private readonly IRepositoryQuery<Region> _regionQuery;
        private readonly IRepositoryQuery<District> _districtQuery;
        private readonly ILogger<FacilityVerificationController> _logger;
        public FacilityVerificationController(IAuditRepo AuditRepo, IRepositoryQuery<OptometristFirm> optometristFirmQuery,
            IRepositoryQuery<Region> regionQuery, IRepositoryQuery<District> districtQuery, ILogger<FacilityVerificationController> logger)
        {
            _optometristFirmQuery = optometristFirmQuery;
            _regionQuery = regionQuery;
            _districtQuery = districtQuery;
            _AuditRepo = AuditRepo;
            _logger = logger;
        }
        // GET: Registration/FacilityVerification

        [HttpGet]
        public ActionResult Index()
        {
            FacilitySearchModel model = new();
            return View();
        }

        [HttpPost]
        public ActionResult Index(FacilitySearchModel model)
        {
            try
            {
                // var obj = _optometristFirmQuery.GetAll().Join(_regionQuery.GetAll().Include(x => x.Districts),
                //o => o.RegionId, r => r.Id, (o, r) => new { o, r })
                //.Select(p => new OptometristFirmModel
                //{
                //    AccreditationNumber = p.o.AccreditationNumber,
                //    BusinessAddress = p.o.BusinessAddress,
                //    BusinessName = p.o.BusinessName,
                //    CentreCode = p.o.CentreCode,
                //    ContactEmailAddress = p.o.ContactEmail,
                //    ContactFirstName = p.o.ContactFirstName,
                //    ContactLastName = p.o.ContactLastName,
                //    ContactPhoneNumber = p.o.ContactPhoneNumber,
                //    CreatedBy = p.o.CreatedBy,
                //    DigitalAddress = p.o.DigitalAddress,
                //    Id = p.o.Id,
                //    IsActive = p.o.IsActive,
                //    IsDeleted = p.o.IsDeleted,
                //    MobileNumber = p.o.MobileNumber,
                //    RegionId = p.o.RegionId,
                //    DistrictId = p.o.DistrictId,
                //    DistrictName = p.r.Districts.FirstOrDefault(x => x.Id == p.o.DistrictId).Name,
                //    RegionName = p.r.Name,
                //    RegistrationNumber = p.o.RegistrationNumber,
                //    ReorderLevel = p.o.ReorderLevel,
                //    TelephoneNumber = p.o.TelephoneNumber,
                //    Town = p.o.Town,
                //    UpdatedBy = p.o.UpdatedBy
                //}).ToList();



                // //AddAudit(Activities.VIEW_OPTOMETRIST_FIRM, "View Optometrist Firm");
                // return View(obj);
                if (!string.IsNullOrEmpty(model.SearchParameter))
                {
                    var obj = _optometristFirmQuery.GetAll().Join(_regionQuery.GetAllInclude(x=>x.Districts),
                        o => o.RegionId, r => r.Id, (o, r) => new { o, r }).Where(x => x.o.BusinessName.Contains(model.SearchParameter) ||
                        x.o.Town.Contains(model.SearchParameter) ||
                        x.r.Name.Contains(model.SearchParameter));
                    if (obj != null)
                    {
                        var result = obj.Select(p => new OptometristFirmModel
                        {
                            AccreditationNumber = p.o.AccreditationNumber,
                            BusinessAddress = p.o.BusinessAddress,
                            BusinessName = p.o.BusinessName,
                            CentreCode = p.o.CentreCode,
                            Town = p.o.Town,
                            RegionName = p.r.Name,
                            DistrictName = p.r.Districts.FirstOrDefault(x => x.Id == p.o.DistrictId).Name,
                            DigitalAddress = p.o.DigitalAddress,
                            TelephoneNumber = p.o.TelephoneNumber,
                            IsActive = p.o.IsActive,
                            Id = p.o.Id
                        }).OrderBy(x => x.BusinessName).ToList();
                        model.facilities = result;
                        return View(model);
                    }
                }
                
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            model.facilities = new List<OptometristFirmModel>();
            return View(model);
        }

              

        public ActionResult Detail(string Id)
        {
            try
            {

                ViewBag.Regions = _regionQuery.GetAll().ToList();


                int optometristId = Convert.ToInt32(Utility.Decrypt(Id));
                var optometrist = _optometristFirmQuery.Filter(x => x.Id == optometristId).FirstOrDefault();
                var region = _regionQuery.Filter(r => r.Id == optometrist.RegionId).FirstOrDefault();
                var district = _districtQuery.Filter(r => r.Id == optometrist.DistrictId).FirstOrDefault();



                var model = new OptometristFirmModel();

                model.Id = optometrist.Id;
                model.RegionId = optometrist.RegionId;
                model.DistrictId = optometrist.DistrictId;
                model.AccreditationNumber = optometrist.AccreditationNumber;
                model.BusinessAddress = optometrist.BusinessAddress;
                model.BusinessName = optometrist.BusinessName;
                model.RegionName = region.Name;
                model.DigitalAddress = optometrist.DigitalAddress;
                model.RegistrationNumber = optometrist.RegistrationNumber;
                model.ContactEmailAddress = optometrist.ContactEmail;
                model.ContactFirstName = optometrist.ContactFirstName;
                model.ContactLastName = optometrist.ContactLastName;
                model.ContactPhoneNumber = optometrist.ContactPhoneNumber;
                model.MobileNumber = optometrist.MobileNumber;
                model.TelephoneNumber = optometrist.TelephoneNumber;
                model.Town = optometrist.Town;
                model.IsActive = optometrist.IsActive;
                model.CreatedBy = optometrist.CreatedBy;
                model.IsDeleted = optometrist.IsDeleted;
                model.UpdatedBy = optometrist.ModifiedBy;


                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return View(new OptometristFirmModel());
        }
    }
}