using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WinApp.Data;
using WinApp.Models;

namespace WinApp.Services
{
    public class VisualAssessmentService
    {

        #region Visual Assessment Result

        #endregion



        #region Transmission

        public static async Task<MessageResponse> Transmit(long id)
        {
            MessageResponse result = null;
            try
            {
                VisualAssessmentResult entity = null;
                byte[] imageBytes = null;
                using (DVLADBContext context = new DVLADBContext())
                {
                    entity = await context.VisualAssessmentResults.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    if (entity == null)
                    {
                        result.Message = "Data not found";
                        return result;
                    }

                    string passportFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", entity.PassportImageUrl);

                    imageBytes = File.ReadAllBytes(passportFilePath);
                }



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
                        using (DVLADBContext context = new DVLADBContext())
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

        public static async Task<Tuple<int, int>> LoadDependencies()
        {
            Tuple<int, int> result = new Tuple<int, int>(0, 0);
            try
            {
                using (DVLADBContext context=new DVLADBContext())
                {
                    var query = context.VisualAssessmentResults.AsNoTracking();
                    int total = await query.CountAsync(x => !x.IsTransmitted);
                    int transmission = await query.CountAsync(x => !x.IsTransmitted && x.Status == 1);
                    result = new Tuple<int, int>(total, transmission);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
            return result;
        }

        public static async Task<MessageResponse> TransmitBulk()
        {
            MessageResponse result = new MessageResponse();
            try
            {
                List<VisualAssessmentResult> assessmentResults = new List<VisualAssessmentResult>();
                string payload = "";
                using (DVLADBContext context = new DVLADBContext())
                {
                    assessmentResults = await context.VisualAssessmentResults.Where(x => x.Status == 1 && !x.IsTransmitted).ToListAsync();
                    if (assessmentResults.Count == 0)
                    {
                        result.Message = "Data not found for transmission";
                        return result;
                    }

                    SystemAdmin sysAdmin = await context.SystemAdmins.AsNoTracking().FirstOrDefaultAsync();
                    if (string.IsNullOrEmpty(sysAdmin.PassportPath))
                    {
                        result.Message = "You have not specified the Passport Path";
                        return result;
                    }

                    List<VisualAssessmentTransmissionModel> models = new List<VisualAssessmentTransmissionModel>();
                    string passportFilePath = string.Empty;
                    byte[] imageBytes = null;
                    foreach (var assessmentResult in assessmentResults)
                    {
                        passportFilePath = Path.Combine(sysAdmin.PassportPath, assessmentResult.PassportImageUrl);
                        imageBytes = File.ReadAllBytes(passportFilePath);

                        models.Add(new VisualAssessmentTransmissionModel()
                        {
                            PassportBase64 = Convert.ToBase64String(imageBytes),
                            AccessType = assessmentResult.AccessType,
                            BCV_OD = assessmentResult.BCV_OD,
                            BCV_OS = assessmentResult.BCV_OS,
                            BCV_OU = assessmentResult.BCV_OU,
                            ColourVision_BCV_OU = assessmentResult.BCV_OU,
                            ContactNumber = assessmentResult.ContactNumber,
                            ContrastSensitivity_BCV = assessmentResult.ContrastSensitivity_BCV,
                            CreatedBy = assessmentResult.CreatedBy,
                            CreatedDate = assessmentResult.CreatedDate,
                            DOB = assessmentResult.DOB,
                            DriversLicence = assessmentResult.DriversLicence,
                            DVLAReferenceNo = assessmentResult.DVLAReferenceNo,
                            Email = assessmentResult.Email,
                            FirstName = assessmentResult.FirstName,
                            FormNumber = assessmentResult.FormNumber,
                            GlareTest_BCV_OD = assessmentResult.GlareTest_BCV_OD,
                            GlareTest_BCV_OS = assessmentResult.GlareTest_BCV_OS,
                            GlareTest_BCV_OU = assessmentResult.GlareTest_BCV_OU,
                            HX_BCV_OD = assessmentResult.HX_BCV_OD,
                            HX_BCV_OS = assessmentResult.HX_BCV_OS,
                            HX_BCV_OU = assessmentResult.HX_BCV_OD,
                            Id = assessmentResult.Id,
                            IsActive = assessmentResult.IsActive,
                            IsDeleted = assessmentResult.IsDeleted,
                            IsRegistration = assessmentResult.IsRegistration,
                            IsSynchronized = assessmentResult.IsSynchronized,
                            IsTransmitted = assessmentResult.IsTransmitted,
                            LearnerDriversLicence = assessmentResult.LearnerDriversLicence,
                            ModifiedBy = assessmentResult.ModifiedBy,
                            NameTitle = assessmentResult.NameTitle,
                            OldDVLAReferenceNo = assessmentResult.OldDVLAReferenceNo,
                            OptometristFirmId = assessmentResult.OptometristFirmId,
                            OtherName = assessmentResult.OtherName,
                            PassOrFail = assessmentResult.PassOrFail,
                            PassportImageUrl = assessmentResult.PassportImageUrl,
                            PassResult = assessmentResult.PassResult,
                            PathologicalRemarks = assessmentResult.PathologicalRemarks,
                            PostalAddress = assessmentResult.PostalAddress,
                            ReferenceNumber = assessmentResult.ReferenceNumber,
                            ResultConclusion = assessmentResult.ResultConclusion,
                            ResultServiceType = assessmentResult.ResultServiceType,
                            SingleImage_BCV_OU = assessmentResult.SingleImage_BCV_OU,
                            Status = assessmentResult.Status,
                            Surname = assessmentResult.Surname,
                            TaxIdentificationNumber = assessmentResult.TaxIdentificationNumber,
                            TestDate = assessmentResult.TestDate,
                            TestType = assessmentResult.TestType,
                            Unaided_OD = assessmentResult.Unaided_OD,
                            Unaided_OS = assessmentResult.Unaided_OS,
                            Unaided_OU = assessmentResult.Unaided_OU
                        });
                    }

                    payload = JsonConvert.SerializeObject(models);

                }

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7112/api/visualassessment/bulk-transmit");
                    //request.Headers.Add("Authorization", "Bearer sk_test_4410c12527c1882602431956acf855b79f82f6bd");
                    var content = new StringContent(payload, null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<MessageResponse>(json);
                    if (result.Success)
                    {
                        using (DVLADBContext context = new DVLADBContext())
                        {
                            foreach (var item in assessmentResults)
                            {
                               var entity = context.VisualAssessmentResults.FirstOrDefault(x => x.Id == item.Id);
                                entity.TransmittedDate = DateTime.Now;
                                entity.IsTransmitted = true;
                            }
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
