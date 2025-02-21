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

namespace DVLA.Business.ReportModule
{
    public class ReportService : IReportRepository
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<ReportService> _logger;
        private readonly string _connectionString;
        public ReportService(DVLADbContext context, ILogger<ReportService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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
                                    OptometristFirmId = reader.GetInt32("OptometristFirmId")
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

        public async Task<List<OptometristFirmModel>> FetchAllOptometristFirms()
        {
            var result = new List<OptometristFirmModel>();
            try
            {

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchAllOptometristFirms", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

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
