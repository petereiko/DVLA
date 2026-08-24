using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.Enumerables;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Business.OptometristFirmModule;
using System.Numerics;
using DVLA.Business.UserModule;
using DVLA.DATA.Domains;

namespace DVLA.Business.ApplicantModule
{
    public class ApplicantService : IApplicantService
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<ApplicantService> _logger;
        private readonly IHostingEnvironment _environment;
        private readonly IOptometristService _optometristService;
        private readonly IUserService _userService;
        private readonly IAuthUser _authUser;
        public ApplicantService(DVLADbContext context, ILogger<ApplicantService> logger, IHostingEnvironment environment, IOptometristService optometristService, IUserService userService, IAuthUser authUser)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
            _optometristService = optometristService;
            _userService = userService;
            _authUser = authUser;
        }


        public ApplicantModel Get(long id)
        {
            ApplicantModel model = null;
            try
            {
                Applicant applicant = _context.Applicants.FirstOrDefault(x => x.Id == id);
                if (applicant == null) return model;
                
                model.Id = applicant.Id;
                //model.NameTitle = applicant.NameTitle;
                model.Surname = applicant.Surname;
                //model.DriversLicence = applicant.DriversLicence;
                //model.DVLAReferenceNo = applicant.DVLAReferenceNo;
                model.FirstName = applicant.FirstName;
                model.OtherName = applicant.OtherName;
                model.DOB = (DateTime)applicant.DOB;
                model.PostalAddress = applicant.PostalAddress;
                model.ContactNumber = applicant.ContactNumber;
                model.Nationality = applicant.TaxIdentificationNumber;
                model.Email = applicant.Email;
                model.ResultServiceType = applicant.ResultServiceType;
                model.PassportImageUrl = applicant.PassportImageUrl;
                model.Status = applicant.Status;
                model.OptometristFirmId = applicant.OptometristFirmId;
                //model.FormNumber = applicant.FormNumber;
                model.TestType = (TestType)applicant.TestType;
                model.IsActive = applicant.IsActive;
                model.CreatedBy = applicant.CreatedBy;
                model.IsDeleted = applicant.IsDeleted;
                model.UpdatedBy = applicant.ModifiedBy;
                model.IsRegistration = applicant.IsRegistration;
                model.ReferenceNumber = applicant.ReferenceNumber;

                if (!string.IsNullOrEmpty(applicant.PassportImageUrl) && applicant.PassportImageUrl.Contains(".png"))
                {
                    var path = Path.Combine(_environment.ContentRootPath, "Passports", applicant.PassportImageUrl);

                    if (System.IO.File.Exists(path))
                    {
                        byte[] imageArray = File.ReadAllBytes(path);
                        model.PassportImageUrl = Convert.ToBase64String(imageArray);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return model;
        }

        public MessageResponse Update(ApplicantModel model, string Id)
        {
            MessageResponse response = new();
            try
            {
                if (string.IsNullOrEmpty(model.PassportImageUrl))
                {
                    response.Message = "Please capture/upload passport";
                    return response;
                }


                if (model.ResultServiceType == null)
                {
                    response.Message = "Please select service type";
                    return response;
                }

                if (string.IsNullOrEmpty(model.Surname))
                {
                    response.Message = "Please enter surname";
                    return response;
                }

                if (string.IsNullOrEmpty(model.FirstName))
                {
                    response.Message = "Please enter first name";
                    return response;
                }

                if (model.DOB == null)
                {
                    response.Message = "Please select DOB";
                    return response;
                }

                if (string.IsNullOrEmpty(model.PostalAddress))
                {
                    response.Message = "Please enter postal address";
                    return response;
                }

                if (model.OptometristFirmId == 0)
                {
                    response.Message = "Please select Optometrist Firm";
                    return response;
                }

                if (string.IsNullOrEmpty(model.ContactNumber))
                {
                    response.Message = "Please enter contact number";
                    return response;
                }

                //var userData = _userService.GetUserData();

                Int64 applicanId = Convert.ToInt64(Utility.Decrypt(Id));
                Applicant applicant = _context.Applicants.FirstOrDefault(x => x.Id == applicanId);

                string[] dob = model.DateOfBirth != null ? model.DateOfBirth.Split('-') : null;
                model.DOB = dob != null ? new DateTime(Convert.ToInt32(dob[0]), Convert.ToInt32(dob[1]), Convert.ToInt32(dob[2])) : model.DOB;
                model.PassportImageUrl = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);


                string base64 = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);
                byte[] imageBytes = Convert.FromBase64String(base64);
                string filename = Guid.NewGuid().ToString();
                // store the file inside ~/project folder(Img)  
                var path = Path.Combine(_environment.ContentRootPath, "Passports", filename + ".png");
                var contents = new MemoryStream(imageBytes);

                //resize image
                Utility.ResizePicture(contents, path);
                //System.IO.File.WriteAllBytes(path, imageBytes);
                model.PassportImageUrl = filename + ".png";

                string passFile = applicant.PassportImageUrl;

                applicant.Surname = model.Surname;
                applicant.FirstName = model.FirstName;
                applicant.OtherName = model.OtherName;
                applicant.DOB = (DateTime)model.DOB;
                applicant.PostalAddress = model.PostalAddress;
                applicant.ContactNumber = model.ContactNumber;
                applicant.TaxIdentificationNumber = model.Nationality;
                applicant.Email = model.Email;
                applicant.ResultServiceType = model.ResultServiceType;
                applicant.PassportImageUrl = model.PassportImageUrl;
                applicant.TestType = (TestType)model.TestType;
                applicant.ModifiedBy = _authUser.UserId;

                _context.SaveChanges();

                if (!string.IsNullOrEmpty(passFile) && passFile.Contains(".png"))
                {
                    var deleteFilePath = Path.Combine(_environment.ContentRootPath, "Passports", passFile);
                    File.Delete(deleteFilePath);
                }

                response.Message = "Record saved successfully";
                response.Success = true;
                //AddAudit(Activities.UPPDATE_APPLICANT_REGISTRATION, "Update applicant");
                return response; //RedirectToAction("BiodataUpdate");
            }
            catch (Exception ex)
            {
                response.Message = "Kindly try again later";
                //TempData["MESSAGE"] = new AlertMessage { Message = "Kindly try again later", MessageType = MessageType.ErrorMessage };
                _logger.LogError(ex.Message, ex);
            }
            return response;
        }
    }
}
