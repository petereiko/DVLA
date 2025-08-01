using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.Data.Models.DataObjects.ViewModels;
using Microsoft.Extensions.Logging;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Microsoft.Extensions.Configuration;
using DVLA.Data.Models.Enumerables;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using System.Net.Http;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DVLA.Business.ReportModule
{
    public class ReportService : IReportRepository
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<ReportService> _logger;
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        
        public ReportService(DVLADbContext context, ILogger<ReportService> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<List<CustomerReportViewModel>> GetCustomerSynchronizationReport(SynchronizationReportFilterViewModel model)
        {
            var result = new List<CustomerReportViewModel>();
            try
            {
                var regionId = model.RegionId ?? System.Data.SqlTypes.SqlInt64.Null;
                string startDate = model.StartDate == null ? "" : model.StartDate;
                string endDate = model.EndDate == null ? "" : model.EndDate;
                if (!string.IsNullOrEmpty(startDate))
                {
                    var startDateArr = startDate.Split("/");
                    startDate = $"{startDateArr[2]}-{startDateArr[1]}-{startDateArr[0]}"; 
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    var endDateArr = endDate.Split("/");
                    endDate = $"{endDateArr[2]}-{endDateArr[1]}-{endDateArr[0]}";
                }
                string centerCode = model.CenterCode == null ? "" : model.CenterCode;
                var optometristFirmId = model.OptometristFirmId ?? System.Data.SqlTypes.SqlInt64.Null;
                int passOrFail = (int)model.Result;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetSynchronizationReport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IsAdministrator", model.IsAdministrator);
                        cmd.Parameters.AddWithValue("@RegionId", regionId);
                        cmd.Parameters.AddWithValue("@OptometristFirmId", optometristFirmId);
                        cmd.Parameters.AddWithValue("@CenterCode", centerCode);
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);
                        cmd.Parameters.AddWithValue("@Result", passOrFail);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new CustomerReportViewModel
                                {
                                    BCV_OD = reader.GetString("BCV_OD"),
                                    BCV_OU = reader.GetString("BCV_OU"),
                                    BCV_OS = reader.GetString("BCV_OS"),
                                    ColourVision_BCV_OU = reader.GetString("ColourVision_BCV_OU"),
                                    ContrastSensitivity_BCV = reader.GetString("ContrastSensitivity_BCV"),
                                    CreatedOn = reader.GetString("CreatedOn"),
                                    DriversLicence = reader.GetString("DriversLicence"),
                                    DVLAReferenceNo = reader.GetString("DVLAReferenceNo"),
                                    Email = reader.GetString("Email"),
                                    FullName = reader.GetString("FullName"),
                                    GlareTest_BCV_OD = reader.GetString("GlareTest_BCV_OD"),
                                    GlareTest_BCV_OS = reader.GetString("GlareTest_BCV_OS"),
                                    GlareTest_BCV_OU = reader.GetString("GlareTest_BCV_OU"),
                                    Grade = reader.GetString("Grade"),
                                    HX_BCV_OD = reader.GetString("HX_BCV_OD"),
                                    HX_BCV_OS = reader.GetString("HX_BCV_OS"),
                                    HX_BCV_OU = reader.GetString("HX_BCV_OU"),
                                    PathologicalRemarks = reader.GetString("PathologicalRemarks"),
                                    ReferenceNumber = reader.GetString("ReferenceNumber"),
                                    ResultConclusion = reader.GetString("ResultConclusion"),
                                    SingleImage_BCV_OU = reader.GetString("SingleImage_BCV_OU"),
                                    TaxIdentificationNumber = reader.GetString("TaxIdentificationNumber"),
                                    Unaided_OD = reader.GetString("Unaided_OD"),
                                    Unaided_OS = reader.GetString("Unaided_OS"),
                                    Unaided_OU = reader.GetString("Unaided_OU")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }


        public async Task<List<SynchronizationReportViewModel>> GetSynchronizationReport(SynchronizationReportFilterViewModel model)
        {
            var result = new List<SynchronizationReportViewModel>();
            try
            {
                var regionId = model.RegionId ?? System.Data.SqlTypes.SqlInt64.Null;
                string startDate = model.StartDate == null ? "" : model.StartDate;
                string endDate = model.EndDate == null ? "" : model.EndDate;
                string centerCode = model.CenterCode == null ? "" : model.CenterCode;
                var optometristFirmId = model.OptometristFirmId ?? System.Data.SqlTypes.SqlInt64.Null;
                int passOrFail = (int)model.Result;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetSynchronizationReport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IsAdministrator", model.IsAdministrator);
                        cmd.Parameters.AddWithValue("@RegionId", regionId);
                        cmd.Parameters.AddWithValue("@OptometristFirmId", optometristFirmId);
                        cmd.Parameters.AddWithValue("@CenterCode", centerCode);
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);
                        cmd.Parameters.AddWithValue("@Result", passOrFail);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new SynchronizationReportViewModel
                                {
                                    BCV_OD = reader.GetString("BCV_OD"),
                                    BCV_OU = reader.GetString("BCV_OU"),
                                    BCV_OS = reader.GetString("BCV_OS"),
                                    ColourVision_BCV_OU = reader.GetString("ColourVision_BCV_OU"),
                                    ContrastSensitivity_BCV = reader.GetString("ContrastSensitivity_BCV"),
                                    CreatedOn = reader.GetString("CreatedOn"),
                                    DriversLicence = reader.GetString("DriversLicence"),
                                    DVLAReferenceNo = reader.GetString("DVLAReferenceNo"),
                                    Email = reader.GetString("Email"),
                                    FullName = reader.GetString("FullName"),
                                    GlareTest_BCV_OD = reader.GetString("GlareTest_BCV_OD"),
                                    GlareTest_BCV_OS = reader.GetString("GlareTest_BCV_OS"),
                                    GlareTest_BCV_OU = reader.GetString("GlareTest_BCV_OU"),
                                    Grade = reader.GetString("Grade"),
                                    HX_BCV_OD = reader.GetString("HX_BCV_OD"),
                                    HX_BCV_OS = reader.GetString("HX_BCV_OS"),
                                    HX_BCV_OU = reader.GetString("HX_BCV_OU"),
                                    PathologicalRemarks = reader.GetString("PathologicalRemarks"),
                                    ReferenceNumber = reader.GetString("ReferenceNumber"),
                                    ResultConclusion = reader.GetString("ResultConclusion"),
                                    SingleImage_BCV_OU = reader.GetString("SingleImage_BCV_OU"),
                                    TaxIdentificationNumber = reader.GetString("TaxIdentificationNumber"),
                                    Unaided_OD = reader.GetString("Unaided_OD"),
                                    Unaided_OS = reader.GetString("Unaided_OS"),
                                    Unaided_OU = reader.GetString("Unaided_OU"),
                                    AccreditationNumber = reader.GetString("AccreditationNumber"),
                                    BusinessAddress = reader.GetString("BusinessAddress"),
                                    BusinessName = reader.GetString("BusinessName"),
                                    CentreCode = reader.GetString("CentreCode"),
                                    ContactEmailAddress = reader.GetString("ContactEmailAddress"),
                                    ContactFirstName = reader.GetString("ContactFirstName"),
                                    ContactLastName = reader.GetString("ContactLastName"),
                                    ContactNumber = reader.GetString("ContactNumber"),
                                    ContactPhoneNumber = reader.GetString("ContactPhoneNumber"),
                                    DigitalAddress = reader.GetString("DigitalAddress"),
                                    MobileNumber = reader.GetString("MobileNumber"),
                                    PostalAddress = reader.GetString("PostalAddress"),
                                    RegionName = reader.GetString("RegionName"),
                                    RegistrationNumber = reader.GetString("RegistrationNumber"),
                                    TelephoneNumber = reader.GetString("TelephoneNumber"),
                                    Town = reader.GetString("Town")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }

        public List<ClientSearchModel> FetchClientSearchOld(ClientSearchParameter searchParameter, Int64? optometristFirmId = null)
        {
            searchParameter = searchParameter == null ? new ClientSearchParameter() : searchParameter;
            var result = new List<ClientSearchModel>();
            try
            {
                SqlParameter[] parameters =
                {
                     new SqlParameter("@ApplicantName",searchParameter.ApplicantName??System.Data.SqlTypes.SqlString.Null),
                     new SqlParameter("@DriversLicenceNumber",searchParameter.DriversLicenceNumber??System.Data.SqlTypes.SqlString.Null),
                     new SqlParameter("@DVLANumber",searchParameter.DVLANumber??System.Data.SqlTypes.SqlString.Null),
                     new SqlParameter("@TestCenter",searchParameter.TestCenter??System.Data.SqlTypes.SqlString.Null),
                      new SqlParameter("@OptometristFirmId",optometristFirmId??System.Data.SqlTypes.SqlInt64.Null)
                };
                result = new();// _context.Database.SqlQuery<ClientSearchModel>("ClientSearch @ApplicantName, @DriversLicenceNumber, @DVLANumber, @TestCenter,@OptometristFirmId", parameters).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }

        public async Task<List<ClientModel>> FetchClientSearch(ClientSearchParameter searchParameter, string optometristAdminId = null, string optometristId = null)
        {
            searchParameter = searchParameter == null ? new ClientSearchParameter() : searchParameter;
            var result = new List<ClientModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("ClientSearch", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ApplicantName", searchParameter.ApplicantName ?? System.Data.SqlTypes.SqlString.Null);
                        cmd.Parameters.AddWithValue("@DriversLicenceNumber", searchParameter.DriversLicenceNumber ?? System.Data.SqlTypes.SqlString.Null);
                        cmd.Parameters.AddWithValue("@DVLANumber", searchParameter.DVLANumber ?? System.Data.SqlTypes.SqlString.Null);
                        cmd.Parameters.AddWithValue("@TestCenter", searchParameter.TestCenter ?? System.Data.SqlTypes.SqlString.Null);
                        cmd.Parameters.AddWithValue("@OptometristAdminId", optometristAdminId ?? System.Data.SqlTypes.SqlString.Null);
                        cmd.Parameters.AddWithValue("@OptometristId", optometristId ?? System.Data.SqlTypes.SqlString.Null);
                        cmd.Parameters.AddWithValue("@ReferenceNumber", searchParameter.ReferenceNumber ?? System.Data.SqlTypes.SqlString.Null);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new ClientModel
                                {
                                    DriversLicence = reader.GetString("DriversLicence"),
                                    DVLAReferenceNo = reader.GetString("DVLAReferenceNo"),
                                    Email = reader.GetString("Email"),
                                    FullName = reader.GetString("FullName"),
                                    ReferenceNumber = reader.GetString("ReferenceNumber"),
                                    ContactNumber = reader.GetString("ContactNumber"),
                                    OptometristCenter = reader.GetString("OptometristCenter"),
                                    PostalAddress = reader.GetString("PostalAddress"),
                                    Region = reader.GetString("Region"),
                                    TestDate = reader.GetDateTime("TestDate")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }

        public async Task<List<SlotReductionModel>> FetchSlotReductionLogs(SlotReductionLogSearchParameter search)
        {
            search = search == null ? new SlotReductionLogSearchParameter() : search;
            var result = new List<SlotReductionModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchSlotReductionLogs", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OptometristFirmId", search.OptometristFirmId ?? System.Data.SqlTypes.SqlInt64.Null);
                        cmd.Parameters.AddWithValue("@StartDate", search.StartDate ?? System.Data.SqlTypes.SqlDateTime.Null);
                        cmd.Parameters.AddWithValue("@EndDate", search.EndDate ?? System.Data.SqlTypes.SqlDateTime.Null);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new SlotReductionModel
                                {
                                    Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString("Comment"),
                                    CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                                    CreatedByFullName = reader.IsDBNull(reader.GetOrdinal("CreatedByFullName")) ? null : reader.GetString("CreatedByFullName"),
                                    DateCreated = reader.GetDateTime("CreatedDate"),
                                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                                    OptometristFirm = reader.IsDBNull(reader.GetOrdinal("OptometristFirm")) ? null : reader.GetString("OptometristFirm"),
                                    OptometristFirmId = reader.GetInt32("OptometristFirmId"),
                                     AccessType= (AccessType)reader.GetInt32("AccessType"),
                                      Quantity= reader.GetInt32("Quantity")
                                });
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }

        public async Task<List<OptometristFirmModel>> FetchAllOptometristFirms(int? region, int? district)
        {
            var result = new List<OptometristFirmModel>();
            try
            {

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchAllOptometristFirms", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Region", region ?? System.Data.SqlTypes.SqlInt32.Null));
                        cmd.Parameters.Add(new SqlParameter("@District", district ?? System.Data.SqlTypes.SqlInt32.Null));

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new OptometristFirmModel
                                {
                                    SlotBalance = reader.GetInt32(reader.GetOrdinal("SlotBalance")),
                                    AccreditationNumber = reader.IsDBNull(reader.GetOrdinal("AccreditationNumber")) ? null : reader.GetString("AccreditationNumber"),
                                    CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                                    BusinessAddress = reader.IsDBNull(reader.GetOrdinal("BusinessAddress")) ? null : reader.GetString("BusinessAddress"),
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    BusinessName = reader.IsDBNull(reader.GetOrdinal("BusinessName")) ? null : reader.GetString("BusinessName"),
                                    CentreCode = reader.IsDBNull(reader.GetOrdinal("CentreCode")) ? null : reader.GetString("CentreCode"),
                                    ContactEmailAddress = reader.IsDBNull(reader.GetOrdinal("ContactEmailAddress")) ? null : reader.GetString("ContactEmailAddress"),
                                    ContactFirstName = reader.IsDBNull(reader.GetOrdinal("ContactFirstName")) ? null : reader.GetString("ContactFirstName"),
                                    ContactLastName = reader.IsDBNull(reader.GetOrdinal("ContactLastName")) ? null : reader.GetString("ContactLastName"),
                                    ContactPhoneNumber = reader.IsDBNull(reader.GetOrdinal("ContactPhoneNumber")) ? null : reader.GetString("ContactPhoneNumber"),
                                    DigitalAddress = reader.IsDBNull(reader.GetOrdinal("DigitalAddress")) ? null : reader.GetString("DigitalAddress"),
                                    DistrictId = reader.GetInt32("DistrictId"),
                                    DistrictName = reader.IsDBNull(reader.GetOrdinal("DistrictName")) ? null : reader.GetString("DistrictName"),
                                    IsActive = reader.GetBoolean("IsActive"),
                                    IsDeleted = reader.GetBoolean("IsDeleted"),
                                    RegionId = reader.GetInt32("RegionId"),
                                    RegionName = reader.IsDBNull(reader.GetOrdinal("RegionName")) ? null : reader.GetString("RegionName"),
                                    RegistrationNumber = reader.IsDBNull(reader.GetOrdinal("RegistrationNumber")) ? null : reader.GetString("RegistrationNumber"),
                                    ReorderLevel = reader.GetInt32("ReorderLevel"),
                                    TelephoneNumber = reader.IsDBNull(reader.GetOrdinal("TelephoneNumber")) ? null : reader.GetString("TelephoneNumber"),
                                    Town = reader.IsDBNull(reader.GetOrdinal("Town")) ? null : reader.GetString("Town"),
                                    MobileNumber = reader.IsDBNull(reader.GetOrdinal("MobileNumber")) ? null : reader.GetString("MobileNumber")
                                });
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }

        public async Task<TransmissionGridDto> FetchDataAsync(TransmissionGridDto model)
        {
            try
            {
                DbContextOptions<VerificationDbContext> sourceOptions = new DbContextOptionsBuilder<VerificationDbContext>()
                .UseSqlServer(model.RequestDto.SourceConnectionString, sqlOptions =>
                {
                    sqlOptions.CommandTimeout(18000);
                })
                .Options;

                DbContextOptions<VerificationDbContext> destinationOptions = new DbContextOptionsBuilder<VerificationDbContext>()
                .UseSqlServer(model.RequestDto.DestinationConnectionString, sqlOptions =>
                {
                    sqlOptions.CommandTimeout(18000);
                })
                .Options;

                IQueryable<VerificationVisualAssessmentResult> query = null;
                List<VerificationVisualAssessmentResult> items = new();

                IQueryable<VerificationVisualAssessmentResult> destinationQuery = null;

                using (var sourceContext = new VerificationDbContext(sourceOptions))
                using (var destinationContext = new VerificationDbContext(destinationOptions))
                {
                    if (sourceContext.Database.CanConnect())
                    {
                        // Proceed to main form, pass the DbContext or options
                         query = sourceContext.Database.SqlQueryRaw<VerificationVisualAssessmentResult>(model.RequestDto.SqlQuery);
                        items = await query.ToListAsync();
                        if (items.Count > 5000)
                        {
                            model.ErrorMessage = "You cannot load more than 5000 records at once. Adjust the query time";
                            return model;
                        }
                    }
                    else
                    {
                        model.ErrorMessage = $"Unable to connect. Please check your internet connection or your connection string.";
                        return model;
                    }
                    if (destinationContext.Database.CanConnect())
                    {
                        // Proceed to main form, pass the DbContext or options
                        destinationQuery = destinationContext.VisualAssessmentResults;
                    }
                    else
                    {
                        model.ErrorMessage = $"Unable to connect. Please check your internet connection or the Destination Connection String.";
                        return model;
                    }

                    items = items.Where(x => !destinationQuery.Select(x => x.ReferenceNumber).Contains(x.ReferenceNumber)).ToList();

                    if (items.Count > 0)
                    {
                        _httpContextAccessor.HttpContext.Session.SetString(AppConstants.TRANSMISSIONDATA, JsonConvert.SerializeObject(items));
                        model.Results = items;
                        model.SuccessMessage = $"{items.Count} records found";
                    }
                    else
                    {
                        model.ErrorMessage = "No new record was found";
                        return model;
                    }
                    
                }

                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                model.ErrorMessage = ex.Message;
            }
            return model;
        }

        public async Task<MessageResponse> PushDataAsync(long? id, string sourceConnString, string destConnString)
        {
            MessageResponse result = new();
            int i = 0;
            //Store records in Session Object
            try
            {
                if (sourceConnString == destConnString)
                {
                    result.Message = "Both Source nd Destination Connection String cannot be the same";
                    return result;
                }

                DbContextOptions<VerificationDbContext> options = new DbContextOptionsBuilder<VerificationDbContext>()
                .UseSqlServer(destConnString, sqlOptions =>
                {
                    sqlOptions.CommandTimeout(18000); // timeout in seconds (e.g., 3 minutes)
                })
                .Options;

                using (var context = new VerificationDbContext(options))
                {
                    if (await context.Database.CanConnectAsync())
                    {
                         string session = _httpContextAccessor.HttpContext.Session.GetString(AppConstants.TRANSMISSIONDATA);
                        IEnumerable<VerificationVisualAssessmentResult> records = JsonConvert.DeserializeObject<IEnumerable<VerificationVisualAssessmentResult>>(session);
                        records = id.HasValue ? records.Where(x => x.Id == id.Value) : records;
                        
                        records =  records.Select(x => new VerificationVisualAssessmentResult
                        {
                            AccessType = x.AccessType,
                            BCV_OD = x.BCV_OD,
                            BCV_OS = x.BCV_OS,
                            BCV_OU = x.BCV_OU,
                            ColourVision_BCV_OU = x.ColourVision_BCV_OU,
                            ContactNumber = x.ContactNumber,
                            ContrastSensitivity_BCV = x.ContrastSensitivity_BCV,
                            CreatedBy = x.CreatedBy,
                            CreatedDate = x.CreatedDate,
                            DOB = x.DOB,
                            Email = x.Email,
                            FirstName = x.FirstName,
                            Gender = x.Gender,
                            GlareTest_BCV_OD = x.GlareTest_BCV_OD,
                            GlareTest_BCV_OS = x.GlareTest_BCV_OS,
                            GlareTest_BCV_OU = x.GlareTest_BCV_OU,
                            HX_BCV_OD = x.HX_BCV_OD,
                            HX_BCV_OS = x.HX_BCV_OD,
                            HX_BCV_OU = x.HX_BCV_OD,
                            IsRegistration = x.IsRegistration,
                            IsVerified = x.IsVerified,
                            Nationality = x.Nationality,
                            OptometristFirmId = x.OptometristFirmId,
                            OptometristFirmName = x.OptometristFirmName,
                            OptometristName = x.OptometristName,
                            OtherName = x.OtherName,
                            PassOrFail = x.PassOrFail,
                            PassportImageUrl = x.PassportImageUrl,
                            PassResult = x.PassResult,
                            PathologicalRemarks = x.PathologicalRemarks,
                            PostalAddress = x.PostalAddress,
                            ReferenceNumber = x.ReferenceNumber,
                            ResultConclusion = x.ResultConclusion,
                            ResultServiceType = x.ResultServiceType,
                            SingleImage_BCV_OU = x.SingleImage_BCV_OU,
                            Status = x.Status,
                            Surname = x.Surname,
                            TestDate = x.TestDate,
                            TestType = x.TestType,
                            TransmittedDate = x.TransmittedDate,
                            Unaided_OD = x.Unaided_OD,
                            Unaided_OS = x.Unaided_OS,
                            Unaided_OU = x.Unaided_OU,
                            VerifiedDate = x.VerifiedDate,
                            VisualAssessmentResultId = x.VisualAssessmentResultId
                        });
                        
                        foreach (var item in records)
                        {
                            i++;
                            bool exist = context.VisualAssessmentResults.Any(x => x.ReferenceNumber == item.ReferenceNumber);
                            if (exist) continue;

                            context.VisualAssessmentResults.Add(item);
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        result.Message = "Unable to connect. Please check your connection string.";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                _logger.LogError(ex.Message, ex);
                return result;
            }
            result.Message = $"{i} records transmitted successfully";
            result.Success = i > 0;
            return result;
        }

        public List<VisualAssessmentResultDto> FetchAllPendingTransmissions()
        {
            var result = new List<VisualAssessmentResultDto>();
            try
            {

                string SafeGetString(SqlDataReader r, string col) =>
    r.IsDBNull(r.GetOrdinal(col)) ? null : r.GetString(col);

                //string? SafeGetNullableDate(SqlDataReader r, string col) =>
                 //   r.IsDBNull(r.GetOrdinal(col)) ? null : r.GetDateTime(col).ToString();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchPendingTransmissions", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new VisualAssessmentResultDto
                                {
                                    VisualAssessmentResultId = reader.GetInt64(reader.GetOrdinal("Id")),
                                    Surname = SafeGetString(reader, "Surname"),
                                    CreatedBy = SafeGetString(reader, "CreatedBy"),
                                    BCV_OD = SafeGetString(reader, "BCV_OD"),
                                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                                    BCV_OS = SafeGetString(reader, "BCV_OS"),
                                    BCV_OU = SafeGetString(reader, "BCV_OU"),
                                    ColourVision_BCV_OU = SafeGetString(reader, "ColourVision_BCV_OU"),
                                    ContactNumber = SafeGetString(reader, "ContactNumber"),
                                    ContrastSensitivity_BCV = SafeGetString(reader, "ContrastSensitivity_BCV"),
                                    Email = SafeGetString(reader, "Email"),
                                    FirstName = SafeGetString(reader, "FirstName"),
                                    GlareTest_BCV_OD = SafeGetString(reader, "GlareTest_BCV_OD"),
                                    GlareTest_BCV_OS = SafeGetString(reader, "GlareTest_BCV_OS"),
                                    GlareTest_BCV_OU = SafeGetString(reader, "GlareTest_BCV_OU"),
                                    PassOrFail = reader.IsDBNull(reader.GetOrdinal("PassOrFail")) ? null : (PassOrFail)reader.GetInt32("PassOrFail"),
                                    AccessType = reader.IsDBNull(reader.GetOrdinal("AccessType")) ? null : (AccessType)reader.GetInt32("AccessType"),
                                    PassResult = reader.IsDBNull(reader.GetOrdinal("PassResult")) ? null : (PassResult)reader.GetInt32("PassResult"),
                                    ResultServiceType = reader.IsDBNull(reader.GetOrdinal("ResultServiceType")) ? null : (ResultServiceType)reader.GetInt32("ResultServiceType"),
                                    Gender = reader.IsDBNull(reader.GetOrdinal("Gender")) ? null : (Gender)reader.GetInt32("Gender"),
                                    IsRegistration = reader.IsDBNull(reader.GetOrdinal("IsRegistration")) ? null : reader.GetBoolean("IsRegistration"),
                                    Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : (Status)reader.GetInt32("Status"),
                                    TestType = (TestType)reader.GetByte(reader.GetOrdinal("TestType")),
                                    DOB = reader.IsDBNull(reader.GetOrdinal("DOB")) ? null : reader.GetDateTime(reader.GetOrdinal("DOB")),
                                    TestDate = reader.IsDBNull(reader.GetOrdinal("TestDate")) ? null : reader.GetDateTime(reader.GetOrdinal("TestDate")),
                                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                    HX_BCV_OD = SafeGetString(reader, "HX_BCV_OD"),
                                    HX_BCV_OS = SafeGetString(reader, "HX_BCV_OS"),
                                    HX_BCV_OU = SafeGetString(reader, "HX_BCV_OU"),
                                    Nationality = SafeGetString(reader, "Nationality"),
                                    OptometristFirmId = reader.GetInt32(reader.GetOrdinal("OptometristFirmId")),
                                    OptometristFirmName = SafeGetString(reader, "OptometristFirmName"),
                                    ReferenceNumber = SafeGetString(reader, "ReferenceNumber"),
                                    OptometristName = SafeGetString(reader, "OptometristName"),
                                    OtherName = SafeGetString(reader, "OtherName"),
                                    PathologicalRemarks = SafeGetString(reader, "PathologicalRemarks"),
                                    Unaided_OD = SafeGetString(reader, "Unaided_OD"),
                                    Unaided_OS = SafeGetString(reader, "Unaided_OS"),
                                    ResultConclusion = SafeGetString(reader, "ResultConclusion"),
                                    PostalAddress = SafeGetString(reader, "PostalAddress"),
                                    SingleImage_BCV_OU = SafeGetString(reader, "SingleImage_BCV_OU"),
                                    PassportImageUrl = SafeGetString(reader, "PassportImageUrl"),
                                    Unaided_OU = SafeGetString(reader, "Unaided_OU"),
                                    PassportNumber = SafeGetString(reader, "PassportNumber"),
                                    NationalID = SafeGetString(reader, "NationalID"),
                                    DvlaLicenseNumber = SafeGetString(reader, "DvlaLicenseNumber")
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }

        public List<UpdateDocRequestDto> FetchAllPendingAuthDocUpdate()
        {
            var result = new List<UpdateDocRequestDto>();
            try
            {

                string SafeGetString(SqlDataReader r, string col) =>
    r.IsDBNull(r.GetOrdinal(col)) ? null : r.GetString(col);

                //string? SafeGetNullableDate(SqlDataReader r, string col) =>
                //   r.IsDBNull(r.GetOrdinal(col)) ? null : r.GetDateTime(col).ToString();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchPendingAuthDocUpdateTransmissions", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new UpdateDocRequestDto
                                {
                                    VisualAssessmentResultId = reader.GetInt64(reader.GetOrdinal("Id")),
                                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                                    ReferenceNumber = SafeGetString(reader, "ReferenceNumber"),
                                    OptometristName = SafeGetString(reader, "OptometristName")
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }


        public byte[] WriteToExcel(string extension, DataTable dt)
        {
            byte[] report = null;
            // dll referred NPOI.dll and NPOI.OOXML 

            IWorkbook workbook;

            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else if (extension == "xls")
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                throw new Exception("This format is not supported");
            }

            ISheet sheet1 = workbook.CreateSheet("Sheet 1");

            var headerStyle = workbook.CreateCellStyle(); //Formatting
            var headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);

            //make a header row 
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row1.CreateCell(j);

                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
            }


            //loops through data 
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                }
            }

            using (var exportData = new MemoryStream())
            {
                workbook.Write(exportData);
                workbook.Close();
                report = exportData.ToArray();
            }

            return report;
        }

    }
}
