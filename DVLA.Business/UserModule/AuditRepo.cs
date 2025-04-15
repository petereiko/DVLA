using DVLA.DATA.Domains;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.Domains;
using DVLA.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.Extensions.Configuration;
using DVLA.Data.Models.Enumerables;

namespace DVLA.Business.UserModule
{
    public class AuditRepo : IAuditRepo
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<AuditRepo> _logger;
        private readonly string _connectionString;
        private readonly IUserService _userService;
        private readonly IAuthUser _authUser;

        private static object initLock = new object();

        public AuditRepo(DVLADbContext context, ILogger<AuditRepo> logger, IConfiguration configuration, IUserService userService, IAuthUser authUser)
        {
            _context = context;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userService = userService;
            _authUser = authUser;
        }

        public void AddAudit(long moduleActionId, string description)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == _authUser.UserId);

            var auditLog = new ActivityLog
            {
                NameOfUser = user?.LastName == null ? user?.LastName : user?.FirstName,
                ModuleActionId = moduleActionId,
                CreatedBy = _authUser.UserId,
                Description = description,
                CreatedDate = DateTime.Now
            };
            _context.ActivityLogs.Add(auditLog);
            _context.SaveChanges();
        }


        public async Task<List<ActivityModel>> GetAuditAsync(AuditFilterModel model)
        {
            var result = new List<ActivityModel>();
            try
            {
                DateTime? startDate = model.StartDate == null ? null : Utility.StartOfDay(model.StartDate.Value);
                DateTime? endDate = model.EndDate == null ? null : Utility.EndOfDay(model.EndDate.Value);

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetAudits", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 300000;
                        cmd.Parameters.AddWithValue("@OptometristFirmId", _authUser.OptometristFirmId ?? System.Data.SqlTypes.SqlInt32.Null);
                        cmd.Parameters.AddWithValue("@ResultConclusion", model.ResultConclusion ?? System.Data.SqlTypes.SqlString.Null);
                        cmd.Parameters.AddWithValue("@Gender", model.Gender ?? System.Data.SqlTypes.SqlInt32.Null);
                        cmd.Parameters.AddWithValue("@PassOrFail", model.PassOrFail ?? System.Data.SqlTypes.SqlInt32.Null);
                        cmd.Parameters.AddWithValue("@StartDate", startDate ?? System.Data.SqlTypes.SqlDateTime.Null);
                        cmd.Parameters.AddWithValue("@EndDate", endDate ?? System.Data.SqlTypes.SqlDateTime.Null);
                        cmd.Parameters.AddWithValue("@Nationality", model.Nationalitiy ?? System.Data.SqlTypes.SqlString.Null);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new ActivityModel
                                {
                                    Applicant = reader.GetSafeString("Applicant"),
                                    BusinessName = reader.GetSafeString("BusinessName"),
                                    Gender = reader.GetSafeInt32("Gender") == null ? "N/A" : ((Gender)reader.GetSafeInt32("Gender")).ToString(),
                                    Nationality = reader.GetSafeString("Nationality"),
                                    Id = reader.GetInt64("Id"),
                                    ResultServiceType = reader.GetSafeInt32("ResultServiceType") == null ? "N/A" : ((ResultServiceType)reader.GetSafeInt32("ResultServiceType")).ToString(),
                                    ResultConclusion = reader.GetSafeString("ResultConclusion"),
                                    TestDate = reader.GetSafeDateTime("TestDate"),
                                    Age = reader.GetSafeDateTime("DOB").HasValue ? $"{DateTime.UtcNow.Year - reader.GetSafeDateTime("DOB").Value.Year}" : "N/A"
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


        public async Task<List<VisualAssessmentExportDto>> GetAuditExportAsync(AuditFilterModel model)
        {
            var result = new List<VisualAssessmentExportDto>();
            try
            {
                DateTime? startDate = model.StartDate == null ? null : Utility.StartOfDay(model.StartDate.Value);
                DateTime? endDate = model.EndDate == null ? null : Utility.EndOfDay(model.EndDate.Value);

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetAuditExport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 300000;
                        cmd.Parameters.AddWithValue("@OptometristFirmId", _authUser.OptometristFirmId ?? System.Data.SqlTypes.SqlInt32.Null);
                        cmd.Parameters.AddWithValue("@ResultConclusion", model.ResultConclusion ?? System.Data.SqlTypes.SqlString.Null);
                        cmd.Parameters.AddWithValue("@Gender", model.Gender ?? System.Data.SqlTypes.SqlInt32.Null);
                        cmd.Parameters.AddWithValue("@PassOrFail", model.PassOrFail ?? System.Data.SqlTypes.SqlInt32.Null);
                        cmd.Parameters.AddWithValue("@StartDate", startDate ?? System.Data.SqlTypes.SqlDateTime.Null);
                        cmd.Parameters.AddWithValue("@EndDate", endDate ?? System.Data.SqlTypes.SqlDateTime.Null);
                        cmd.Parameters.AddWithValue("@Nationality", model.Nationalitiy ?? System.Data.SqlTypes.SqlString.Null);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.Add(new VisualAssessmentExportDto
                                {
                                    BCV_OD = reader.GetSafeString("BCV_OD"),
                                    BCV_OS = reader.GetSafeString("BCV_OS"),
                                    BCV_OU = reader.GetSafeString("BCV_OU"),
                                    GlareTest_BCV_OD = reader.GetSafeString("GlareTest_BCV_OD"),
                                    GlareTest_BCV_OS = reader.GetSafeString("GlareTest_BCV_OS"),
                                    ColourVision_BCV_OU = reader.GetSafeString("ColourVision_BCV_OU"),
                                    ContrastSensitivity_BCV = reader.GetSafeString("ContrastSensitivity_BCV"),
                                    GlareTest_BCV_OU = reader.GetSafeString("GlareTest_BCV_OU"),
                                    HX_BCV_OD = reader.GetSafeString("HX_BCV_OD"),
                                    HX_BCV_OS = reader.GetSafeString("HX_BCV_OS"),
                                    HX_BCV_OU = reader.GetSafeString("HX_BCV_OU"),
                                    SingleImage_BCV_OU = reader.GetSafeString("SingleImage_BCV_OU"),
                                    Unaided_OD = reader.GetSafeString("Unaided_OD"),
                                    Unaided_OS = reader.GetSafeString("Unaided_OS"),
                                    Unaided_OU = reader.GetSafeString("Unaided_OU"),
                                    ContactNumber = reader.GetSafeString("ContactNumber"),
                                    Email = reader.GetSafeString("Email"),
                                    FirstName = reader.GetSafeString("FirstName"),
                                    OptometristFirm = reader.GetSafeString("OptometristFirm"),
                                    Surname = reader.GetSafeString("Surname"),
                                    OtherName = reader.GetSafeString("OtherName"),
                                    DOB = reader.GetSafeDateTime("DOB"),
                                    ResultServiceType = reader.GetSafeInt32("ResultServiceType") == null ? "N/A" : ((ResultServiceType)reader.GetSafeInt32("ResultServiceType")).ToString(),
                                    PassResult = reader.GetSafeInt32("PassResult") == null ? "N/A" : ((PassResult)reader.GetSafeInt32("PassResult")).ToString(),
                                    AccessType = reader.GetSafeInt32("AccessType") == null ? "N/A" : ((AccessType)reader.GetSafeInt32("AccessType")).ToString(),
                                    Gender = reader.GetSafeInt32("Gender") == null ? "N/A" : ((Gender)reader.GetSafeInt32("Gender")).ToString(),
                                    Nationality = reader.GetSafeString("Nationality"),
                                    Id = reader.GetInt64("Id"),
                                    PassOrFail = reader.GetSafeInt32("PassOrFail") == null ? "N/A" : ((PassResult)reader.GetSafeInt32("PassOrFail")).ToString(),
                                    ResultConclusion = reader.GetSafeString("ResultConclusion"),
                                    Status = reader.GetSafeInt32("Status") == null ? "N/A" : ((Status)reader.GetSafeInt32("Status")).ToString(),
                                    TestDate = reader.GetSafeDateTime("TestDate"),
                                    PassportImageUrl = $"{_authUser.BaseUrl}/Passports/{reader.GetSafeString("PassportImageUrl")}",
                                    PathologicalRemarks = reader.GetSafeString("PathologicalRemarks"),
                                    PostalAddress = reader.GetSafeString("PostalAddress"),
                                    ReferenceNumber = reader.GetSafeString("ReferenceNumber")
                                    //TestType = reader.GetSafeByte("TestType") == null ? "N/A" : ((TestType)reader.GetSafeByte("TestType")).ToString()
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

    }
}
