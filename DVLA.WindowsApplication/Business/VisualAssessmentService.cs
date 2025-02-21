using DVLA.WindowsApplication.Data;
using DVLA.WindowsApplication.Enums;
using DVLA.WindowsApplication.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.WindowsApplication.Business
{
    public class VisualAssessmentService
    {

        #region Visual Assessment Result

        private static List<OptometristFirmViewModel> _allOptometristFirms;
        public static async Task<MessageResponse<long>> CreateAsync(VisualAssessmentResult model)
        {
            MessageResponse<long> result = new MessageResponse<long>();
            try
            {
                using (DVLAContext context = new DVLAContext())
                {
                    context.VisualAssessmentResults.Add(model);
                    await context.SaveChangesAsync();
                    result.Message = "Entries saved successfully";
                    result.Success = true;
                }
                result.Result = model.Id;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = "An error occurred while trying to process your request. Please try again later";
            }
            return result;
        }

        public static async Task<MessageResponse<long>> UpdateAsync(VisualAssessmentResult model)
        {
            MessageResponse<long> result = new MessageResponse<long>();
            try
            {
                using (DVLAContext context = new DVLAContext())
                {
                    var entity = await context.VisualAssessmentResults.FirstOrDefaultAsync(x => x.Id == model.Id);
                    entity.AccessType = model.AccessType;
                    //entity.Status = model.Status;
                    entity.PostalAddress = model.PostalAddress;
                    entity.BCV_OD = model.BCV_OD;
                    entity.Unaided_OD = model.Unaided_OD;
                    entity.Unaided_OS = model.Unaided_OS;
                    entity.Unaided_OU = model.Unaided_OU;
                    entity.BCV_OS = model.BCV_OS;
                    entity.BCV_OU = model.BCV_OU;
                    entity.ColourVision_BCV_OU = model.ColourVision_BCV_OU;
                    entity.HX_BCV_OD = model.HX_BCV_OD;
                    entity.HX_BCV_OU = model.HX_BCV_OU;
                    entity.HX_BCV_OS = model.HX_BCV_OS;
                    entity.ContactNumber = model.ContactNumber;
                    entity.ContrastSensitivity_BCV = model.ContrastSensitivity_BCV;
                    entity.DOB = model.DOB;
                    entity.Email = model.Email;
                    entity.FirstName = model.FirstName;
                    entity.OtherName = model.OtherName;
                    entity.Surname = model.Surname;
                    entity.GlareTest_BCV_OD = model.GlareTest_BCV_OD;
                    entity.GlareTest_BCV_OS = model.GlareTest_BCV_OS;
                    entity.GlareTest_BCV_OU = model.GlareTest_BCV_OU;
                    entity.PassOrFail = model.PassOrFail;
                    entity.PassportImageUrl = model.PassportImageUrl;
                    entity.PassResult = model.PassResult;
                    entity.PathologicalRemarks = model.PathologicalRemarks;
                    entity.TaxIdentificationNumber = model.TaxIdentificationNumber;
                    entity.ResultServiceType = model.ResultServiceType;
                    entity.LearnerDriversLicence = model.LearnerDriversLicence;
                    entity.SingleImage_BCV_OU = model.SingleImage_BCV_OU;
                    entity.ResultConclusion = model.ResultConclusion;
                    entity.IsTransmitted = model.IsTransmitted;
                    entity.ModifiedBy = model.CreatedBy;
                    entity.ModifiedDate = DateTime.Now;
                    await context.SaveChangesAsync();
                    result.Message = "Entries saved successfully";
                    result.Success = true;
                    result.Result = model.Id;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = "An error occurred while trying to process your request. Please try again later";
            }
            return result;
        }

        public static async Task<List<AssessmentItemViewModel>> GetAllAsync(string userId)
        {
            List<AssessmentItemViewModel> items = new List<AssessmentItemViewModel>();
            using (DVLAContext context = new DVLAContext())
            {
                items = await context.VisualAssessmentResults.AsNoTracking().Where(x=>x.CreatedBy==userId)
                    .Select(x => new AssessmentItemViewModel
                    {
                        AccessType = x.AccessType,
                        ContactNumber = x.ContactNumber,
                        CreatedDate = x.CreatedDate,
                        FirstName = x.FirstName,
                        Id = x.Id,
                        IsSynchronized = x.IsSynchronized,
                        IsTransmitted = x.IsTransmitted,
                        LearnerDriversLicence = (LearnerDriversLicenceType)x.LearnerDriversLicence.Value,
                        OptometristFirmId = x.OptometristFirmId,
                        PassOrFail = x.PassOrFail,
                        PassportImageUrl = x.PassportImageUrl,
                        ReferenceNumber = x.ReferenceNumber,
                        ResultConclusion = x.ResultConclusion,
                        ResultServiceType = (ResultServiceType)x.ResultServiceType.Value,
                        Status = x.Status,
                        Surname = x.Surname,
                        TestDate = x.TestDate,
                        TestType = x.TestType,
                        TIN = x.TaxIdentificationNumber
                    }).ToListAsync();

                _allOptometristFirms = await GetAllOptometristFirms();

                Parallel.ForEach(items, (i) =>
                {
                    i.OptometristFirmName = _allOptometristFirms.FirstOrDefault(x => x.Id == i.OptometristFirmId)?.BusinessName;
                });

                return items;
            }
        }

        public static async Task<VisualAssessmentResult> GetAsync(long id)
        {
            using (DVLAContext context = new DVLAContext())
            {
                return await context.VisualAssessmentResults.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            }
        }
        #endregion

        #region Optometrist Firm Management
        public static async Task<List<OptometristFirmViewModel>> GetAllOptometristFirms()
        {
            List<OptometristFirmViewModel> model = new List<OptometristFirmViewModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/visualassessment/get-all-optometristfirms");
                    //request.Headers.Add("Authorization", "Bearer sk_test_4410c12527c1882602431956acf855b79f82f6bd");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<List<OptometristFirmViewModel>>(json);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
            return model;
        }

        public static async Task<OptometristFirmViewModel> GetOptometristFirmById(long id)
        {
            OptometristFirmViewModel model = null;
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost/api/visualassessment/get-optometristfirm-by-id/{id}");
                    //request.Headers.Add("Authorization", "Bearer sk_test_4410c12527c1882602431956acf855b79f82f6bd");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<OptometristFirmViewModel>(json);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
            return model;
        }
        #endregion

        #region Transmission

        public static async Task<MessageResponse> Transmit(long id)
        {
            MessageResponse result = null;
            try
            {
                var entity = await GetAsync(id);
                if (entity == null)
                {
                    result.Message = "Data not found";
                    return result;
                }

                string passportFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", entity.PassportImageUrl);

                byte[] imageBytes = File.ReadAllBytes(passportFilePath);


                VisualAssessmentTransmissionModel model = new VisualAssessmentTransmissionModel
                {
                    PassportBase64 = Convert.ToBase64String(imageBytes),
                    AccessType = entity.AccessType,
                    BCV_OD = entity.BCV_OD,
                    BCV_OS = entity.BCV_OS,
                    BCV_OU = entity.BCV_OU,
                    ColourVision_BCV_OU = entity.BCV_OU,
                    ContactNumber = entity.ContactNumber,
                    ContrastSensitivity_BCV = entity.ContrastSensitivity_BCV,
                    CreatedBy = entity.CreatedBy,
                    CreatedDate = entity.CreatedDate,
                    DOB = entity.DOB,
                    DriversLicence = entity.DriversLicence,
                    DVLAReferenceNo = entity.DVLAReferenceNo,
                    Email = entity.Email,
                    FirstName = entity.FirstName,
                    FormNumber = entity.FormNumber,
                    GlareTest_BCV_OD = entity.GlareTest_BCV_OD,
                    GlareTest_BCV_OS = entity.GlareTest_BCV_OS,
                    GlareTest_BCV_OU = entity.GlareTest_BCV_OU,
                    HX_BCV_OD = entity.HX_BCV_OD,
                    HX_BCV_OS = entity.HX_BCV_OS,
                    HX_BCV_OU = entity.HX_BCV_OD,
                    Id = entity.Id,
                    IsActive = entity.IsActive,
                    IsDeleted = entity.IsDeleted,
                    IsRegistration = entity.IsRegistration,
                    IsSynchronized = entity.IsSynchronized,
                    IsTransmitted = entity.IsTransmitted,
                    LearnerDriversLicence = entity.LearnerDriversLicence,
                    ModifiedBy = entity.ModifiedBy,
                    NameTitle = entity.NameTitle,
                    OldDVLAReferenceNo = entity.OldDVLAReferenceNo,
                    OptometristFirmId = entity.OptometristFirmId,
                    OtherName = entity.OtherName,
                    PassOrFail = entity.PassOrFail,
                    PassportImageUrl = entity.PassportImageUrl,
                    PassResult = entity.PassResult,
                    PathologicalRemarks = entity.PathologicalRemarks,
                    PostalAddress = entity.PostalAddress,
                    ReferenceNumber = entity.ReferenceNumber,
                    ResultConclusion = entity.ResultConclusion,
                    ResultServiceType = entity.ResultServiceType,
                    SingleImage_BCV_OU = entity.SingleImage_BCV_OU,
                    Status = entity.Status,
                    Surname = entity.Surname,
                    TaxIdentificationNumber = entity.TaxIdentificationNumber,
                    TestDate = entity.TestDate,
                    TestType = entity.TestType,
                    Unaided_OD = entity.Unaided_OD,
                    Unaided_OS = entity.Unaided_OS,
                    Unaided_OU = entity.Unaided_OU
                };
                string payload = JsonConvert.SerializeObject(model);

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost/api/visualassessment/transmit");
                    //request.Headers.Add("Authorization", "Bearer sk_test_4410c12527c1882602431956acf855b79f82f6bd");
                    var content = new StringContent(payload, null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<MessageResponse>(json);
                    if (result.Success)
                    {
                        using (DVLAContext context = new DVLAContext())
                        {
                            VisualAssessmentResult assessment = await context.VisualAssessmentResults.FirstOrDefaultAsync(x => x.Id == id);
                            assessment.IsTransmitted = true;
                            assessment.TransmittedDate = DateTime.Now;
                            await context.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = ex.Message;
            }
            return result;
        }

        #endregion

    }
}
