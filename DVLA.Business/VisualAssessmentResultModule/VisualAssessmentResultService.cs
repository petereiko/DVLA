using Azure;
using DVLA.Business.NotificationModule;
using DVLA.Business.Repository;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Domains;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.VisualAssessmentResultModule
{
    public class VisualAssessmentResultService : IVisualAssessmentResultRepository
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<VisualAssessmentResultService> _logger;
        private readonly IHostingEnvironment _environment;
        private readonly IRepositoryQuery<Slot> _slotRepositoryQuery;
        static readonly object transactionLock = new object();
        private readonly string _connectionString;
        private readonly AppSettings _appSettings;
        private readonly ISmsRepository _smsRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IRepositoryQuery<VisualAssessmentResult> _visualAssessmentResultRepository;


        public VisualAssessmentResultService(DVLADbContext context, ILogger<VisualAssessmentResultService> logger, IOptions<AppSettings> options, IConfiguration configuration, IHostingEnvironment environment, IRepositoryQuery<Slot> slotRepositoryQuery, ISmsRepository smsRepository, INotificationRepository notificationRepository, IRepositoryQuery<VisualAssessmentResult> visualAssessmentResultRepository)
        {
            _context = context;
            _logger = logger;
            _appSettings = options.Value;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _environment = environment;
            _slotRepositoryQuery = slotRepositoryQuery;
            _smsRepository = smsRepository;
            _notificationRepository = notificationRepository;
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
        }



        private long GenerateSerialNumber()
        {
            var count = 0L;

            lock (transactionLock)
            {
                try
                {
                    var serialNumber = _context.SerialNumbers.FirstOrDefault(x => x.SerialType == 1);
                    if (serialNumber != null && serialNumber.CreatedDate.Year == DateTime.Now.Year)
                    {
                        count = serialNumber.LastCount;
                    }
                    else
                    {
                        serialNumber = new()
                        {
                            CreatedBy = "System",
                            CreatedDate = DateTime.Now,
                            LastCount = 0,
                            SerialType = 1
                        };
                        _context.SerialNumbers.Add(serialNumber);
                        _context.SaveChanges();
                        count = serialNumber.LastCount;
                    }
                    count = count + 1;
                    serialNumber.LastCount = count;
                    serialNumber.CreatedDate = DateTime.Now;
                    _context.SaveChanges();

                    return count;

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private long GenerateFormNumber()
        {
            var count = 0L;

            lock (transactionLock)
            {
                try
                {
                    var serialNumber = _context.SerialNumbers.FirstOrDefault(x => x.SerialType == 2);
                    if (serialNumber != null && serialNumber.CreatedDate.Year == DateTime.Now.Year)
                    {
                        count = serialNumber.LastCount;
                    }
                    else
                    {
                        serialNumber = new()
                        {
                            CreatedBy = "System",
                            LastCount = 0,
                            SerialType = 2
                        };
                        _context.SerialNumbers.Add(serialNumber);
                        _context.SaveChanges();
                    }
                    count = count + 1;

                    serialNumber.LastCount = count;

                    serialNumber.CreatedDate = DateTime.Now;
                    _context.SerialNumbers.Attach(serialNumber);
                    _context.Entry(serialNumber).State = EntityState.Modified;
                    _context.SaveChanges();

                    return count;

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public string GenerateReferenceNo(int optometristFirmId, Status status)
        {
            string result = null;
            if (status == Status.InProgress) return result;
            lock (transactionLock)
            {
                //var count = GenerateSerialNumber();
                var optometristFirm = _context.OptometristFirms.Include(x => x.Region).FirstOrDefault(x => x.Id == optometristFirmId);
                var regionPrefix = optometristFirm.Region.PrefixName;
                int applicantCount = _context.VisualAssessmentResults.Count();
                string countString = applicantCount.ToString().PadLeft(4, '0');
                var accrdArry = optometristFirm.AccreditationNumber.Split('/');
                string serial = accrdArry[1];
                result = $"DS{serial}{DateTime.Now.ToString("yy")}{optometristFirm.Region.PrefixName}{countString}";
                //DS2022  25ASH0915

                while (_context.VisualAssessmentResults.Any(x => x.ReferenceNumber == result))
                {
                    applicantCount++;
                    countString = applicantCount.ToString().PadLeft(4, '0');
                    result = $"DS{serial}{DateTime.Now.ToString("yy")}{optometristFirm.Region.PrefixName}{countString}";
                }

            }

            return result;
        }

        public string GenerateFormNo()
        {
            var count = GenerateFormNumber();
            var result = string.Format("DVLA/{0}{1}", DateTime.Today.ToString("yy"), count.ToString().PadLeft(7, '0'));
            return result;
        }

        public async Task<IEnumerable<VisualAssessmentResultDto>> GetPendingTransmissions()
        {
            IEnumerable<VisualAssessmentResultDto> results=Enumerable.Empty<VisualAssessmentResultDto>();
            try
            {
                _visualAssessmentResultRepository.Filter(x => x.HasTransmissionError == false && x.IsTransmitted == false).OrderByDescending(x => x.Id).Take(1000)
                    .Select(x => new VisualAssessmentResultDto
                    {
                        AccessType = x.AccessType,
                        Id = x.Id,
                        ContactNumber = x.ContactNumber,

                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return results;
        }



        public void Dispose()
        {
            _context.Dispose();
        }

        public VisualAssessmentResultModel FetchAssessmentResult(string ReferenceNumber)
        {
            VisualAssessmentResultModel record = null;//_context.Database.SqlQuery<VisualAssessmentResultModel>("FetchVisualAssessmentResultWithPassport @DriversLicence, @DvlaReferenceNo, @VasReferenceNo", parameters).FirstOrDefault();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchVisualAssessmentResultWithPassport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ReferenceNumber", ReferenceNumber ?? System.Data.SqlTypes.SqlString.Null);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                record = new VisualAssessmentResultModel
                                {
                                    //DriversLicence = !reader.IsDBNull(reader.GetOrdinal("DriversLicence")) ? reader.GetString("DriversLicence") : null,
                                    //DVLAReferenceNo = !reader.IsDBNull(reader.GetOrdinal("DVLAReferenceNo")) ? reader.GetString("DVLAReferenceNo") : null,
                                    Email = !reader.IsDBNull(reader.GetOrdinal("Email")) ? reader.GetString("Email") : null,
                                    AccreditationNumber = !reader.IsDBNull(reader.GetOrdinal("AccreditationNumber")) ? reader.GetString("AccreditationNumber") : null,
                                    ReferenceNumber = !reader.IsDBNull(reader.GetOrdinal("ReferenceNumber")) ? reader.GetString("ReferenceNumber") : null,
                                    ContactNumber = !reader.IsDBNull(reader.GetOrdinal("ContactNumber")) ? reader.GetString("ContactNumber") : null,
                                    PostalAddress = !reader.IsDBNull(reader.GetOrdinal("PostalAddress")) ? reader.GetString("PostalAddress") : null,
                                    TestDate = !reader.IsDBNull(reader.GetOrdinal("TestDate")) ? reader.GetDateTime("TestDate") : (DateTime?)null,
                                    BusinessAddress = !reader.IsDBNull(reader.GetOrdinal("BusinessAddress")) ? reader.GetString("BusinessAddress") : null,
                                    BusinessName = !reader.IsDBNull(reader.GetOrdinal("BusinessName")) ? reader.GetString("BusinessName") : null,
                                    Id = !reader.IsDBNull(reader.GetOrdinal("Id")) ? reader.GetInt64("Id") : 0,
                                    //CreatedByFullName = !reader.IsDBNull(reader.GetOrdinal("Fullname")) ? reader.GetString("Fullname") : null,
                                    BCV_OD = !reader.IsDBNull(reader.GetOrdinal("BCV_OD")) ? reader.GetString("BCV_OD") : null,
                                    BCV_OS = !reader.IsDBNull(reader.GetOrdinal("BCV_OS")) ? reader.GetString("BCV_OS") : null,
                                    BCV_OU = !reader.IsDBNull(reader.GetOrdinal("BCV_OU")) ? reader.GetString("BCV_OU") : null,
                                    CentreCode = !reader.IsDBNull(reader.GetOrdinal("CentreCode")) ? reader.GetString("CentreCode") : null,
                                    ColourVision_BCV_OU = !reader.IsDBNull(reader.GetOrdinal("ColourVision_BCV_OU")) ? reader.GetString("ColourVision_BCV_OU") : null,
                                    ContactEmail = !reader.IsDBNull(reader.GetOrdinal("ContactEmail")) ? reader.GetString("ContactEmail") : null,
                                    ContactFirstName = !reader.IsDBNull(reader.GetOrdinal("ContactFirstName")) ? reader.GetString("ContactFirstName") : null,
                                    ContactLastName = !reader.IsDBNull(reader.GetOrdinal("ContactLastName")) ? reader.GetString("ContactLastName") : null,
                                    ContactPhoneNumber = !reader.IsDBNull(reader.GetOrdinal("ContactPhoneNumber")) ? reader.GetString("ContactPhoneNumber") : null,
                                    ContrastSensitivity_BCV = !reader.IsDBNull(reader.GetOrdinal("ContrastSensitivity_BCV")) ? reader.GetString("ContrastSensitivity_BCV") : null,
                                    CreatedBy = !reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? reader.GetString("CreatedBy") : null,
                                    //CreatedByUsername = !reader.IsDBNull(reader.GetOrdinal("CreatedByUsername")) ? reader.GetString("CreatedByUsername") : null,
                                    DateCreated = reader.GetDateTime("CreatedDate"),
                                    DigitalAddress = !reader.IsDBNull(reader.GetOrdinal("DigitalAddress")) ? reader.GetString("DigitalAddress") : null,
                                    DistrictName = !reader.IsDBNull(reader.GetOrdinal("DistrictName")) ? reader.GetString("DistrictName") : null,
                                    DOB = !reader.IsDBNull(reader.GetOrdinal("DOB")) ? reader.GetDateTime("DOB") : (DateTime?)null,
                                    FirstName = !reader.IsDBNull(reader.GetOrdinal("FirstName")) ? reader.GetString("FirstName") : null,
                                    //FormNumber = !reader.IsDBNull(reader.GetOrdinal("FormNumber")) ? reader.GetString("FormNumber") : null,
                                    GlareTest_BCV_OD = !reader.IsDBNull(reader.GetOrdinal("GlareTest_BCV_OD")) ? reader.GetString("GlareTest_BCV_OD") : null,
                                    GlareTest_BCV_OS = !reader.IsDBNull(reader.GetOrdinal("GlareTest_BCV_OS")) ? reader.GetString("GlareTest_BCV_OS") : null,
                                    GlareTest_BCV_OU = !reader.IsDBNull(reader.GetOrdinal("GlareTest_BCV_OU")) ? reader.GetString("GlareTest_BCV_OU") : null,
                                    HX_BCV_OD = !reader.IsDBNull(reader.GetOrdinal("HX_BCV_OD")) ? reader.GetString("HX_BCV_OD") : null,
                                    HX_BCV_OS = !reader.IsDBNull(reader.GetOrdinal("HX_BCV_OS")) ? reader.GetString("HX_BCV_OS") : null,
                                    HX_BCV_OU = !reader.IsDBNull(reader.GetOrdinal("HX_BCV_OU")) ? reader.GetString("HX_BCV_OU") : null,
                                    IsActive = !reader.IsDBNull(reader.GetOrdinal("IsActive")) && reader.GetBoolean("IsActive"),
                                    IsGHDriveSynchronized = !reader.IsDBNull(reader.GetOrdinal("IsGHDriveSynchronized")) && reader.GetBoolean("IsGHDriveSynchronized"),
                                    IsSynchronized = !reader.IsDBNull(reader.GetOrdinal("IsSynchronized")) && reader.GetBoolean("IsSynchronized"),
                                    MobileNumber = !reader.IsDBNull(reader.GetOrdinal("MobileNumber")) ? reader.GetString("MobileNumber") : null,
                                    //NameTitle = !reader.IsDBNull(reader.GetOrdinal("NameTitle")) ? (NameTitle)reader.GetInt32("NameTitle") : default,
                                    //Optometrist = !reader.IsDBNull(reader.GetOrdinal("Optometrist")) ? reader.GetString("Optometrist") : null,
                                    OptometristFirmId = !reader.IsDBNull(reader.GetOrdinal("OptometristFirmId")) ? reader.GetInt32("OptometristFirmId") : 0,
                                    OtherName = !reader.IsDBNull(reader.GetOrdinal("OtherName")) ? reader.GetString("OtherName") : null,
                                    PassOrFail = !reader.IsDBNull(reader.GetOrdinal("PassOrFail")) ? (PassOrFail)reader.GetInt32("PassOrFail") : default,
                                    PassResult = !reader.IsDBNull(reader.GetOrdinal("PassResult")) ? (PassResult)reader.GetInt32("PassResult") : PassResult.Unlimited,
                                    PathologicalRemarks = !reader.IsDBNull(reader.GetOrdinal("PathologicalRemarks")) ? reader.GetString("PathologicalRemarks") : null,
                                    RegionName = !reader.IsDBNull(reader.GetOrdinal("RegionName")) ? reader.GetString("RegionName") : null,
                                    RegistrationNumber = !reader.IsDBNull(reader.GetOrdinal("RegistrationNumber")) ? reader.GetString("RegistrationNumber") : null,
                                    ResultConclusion = !reader.IsDBNull(reader.GetOrdinal("ResultConclusion")) ? reader.GetString("ResultConclusion") : null,
                                    ResultServiceType = !reader.IsDBNull(reader.GetOrdinal("ResultServiceType")) ? (ResultServiceType)reader.GetInt32("ResultServiceType") : default,
                                    SingleImage_BCV_OU = !reader.IsDBNull(reader.GetOrdinal("SingleImage_BCV_OU")) ? reader.GetString("SingleImage_BCV_OU") : null,
                                    Status = !reader.IsDBNull(reader.GetOrdinal("Status")) ? (Status)reader.GetInt32("Status") : default,
                                    Surname = !reader.IsDBNull(reader.GetOrdinal("Surname")) ? reader.GetString("Surname") : null,
                                    TaxIdentificationNumber = !reader.IsDBNull(reader.GetOrdinal("TaxIdentificationNumber")) ? reader.GetString("TaxIdentificationNumber") : null,
                                    TelephoneNumber = !reader.IsDBNull(reader.GetOrdinal("TelephoneNumber")) ? reader.GetString("TelephoneNumber") : null,
                                    Unaided_OD = !reader.IsDBNull(reader.GetOrdinal("Unaided_OD")) ? reader.GetString("Unaided_OD") : null,
                                    Unaided_OS = !reader.IsDBNull(reader.GetOrdinal("Unaided_OS")) ? reader.GetString("Unaided_OS") : null,
                                    Unaided_OU = !reader.IsDBNull(reader.GetOrdinal("Unaided_OU")) ? reader.GetString("Unaided_OU") : null,
                                    //UpdatedByUsername = !reader.IsDBNull(reader.GetOrdinal("UpdatedByUsername")) ? reader.GetString("UpdatedByUsername") : null,
                                    IsDeleted = !reader.IsDBNull(reader.GetOrdinal("IsDeleted")) && reader.GetBoolean("IsDeleted"),
                                    IsRegistration = !reader.IsDBNull(reader.GetOrdinal("IsRegistration")) && reader.GetBoolean("IsRegistration"),
                                    UpdatedBy = !reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) ? reader.GetString("ModifiedBy") : null,
                                    UserName = !reader.IsDBNull(reader.GetOrdinal("UserName")) ? reader.GetString("UserName") : null,
                                    PassportImageUrl = !reader.IsDBNull(reader.GetOrdinal("PassportImageUrl")) ? reader.GetString("PassportImageUrl") : null
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return record;
        }

        public List<VisualAssessmentResultModel> FetchAssessmentResults(long? optometristAdminId, long? optometristId, long? id)
        {
            List<VisualAssessmentResultModel> result = new();

            SqlParameter[] parameters =
            {
                new SqlParameter("@OptometristAdminID",optometristAdminId??System.Data.SqlTypes.SqlInt64.Null),
                new SqlParameter("@OptometristID",optometristId??System.Data.SqlTypes.SqlInt64.Null),
                new SqlParameter("@Id",id??System.Data.SqlTypes.SqlInt64.Null)
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchVisualAssessmentResult", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OptometristAdminID", optometristAdminId ?? System.Data.SqlTypes.SqlInt64.Null);
                        cmd.Parameters.AddWithValue("@OptometristID", optometristId ?? System.Data.SqlTypes.SqlInt64.Null);
                        cmd.Parameters.AddWithValue("@Id", id ?? System.Data.SqlTypes.SqlInt64.Null);

                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new VisualAssessmentResultModel
                                {
                                    //DriversLicence = reader.GetString("DriversLicence"),
                                    //DVLAReferenceNo = reader.GetString("DVLAReferenceNo"),
                                    Email = reader.GetString("Email"),
                                    AccreditationNumber = reader.GetString("AccreditationNumber"),
                                    ReferenceNumber = reader.GetString("ReferenceNumber"),
                                    ContactNumber = reader.GetString("ContactNumber"),
                                    PostalAddress = reader.GetString("PostalAddress"),
                                    TestDate = reader.GetDateTime("TestDate"),
                                    BusinessAddress = reader.GetString("BusinessAddress"),
                                    BusinessName = reader.GetString("BusinessName"),
                                    Id = reader.GetInt64("Id"),
                                    //CreatedByFullName = reader.GetString("Fullname"),
                                    BCV_OD = reader.GetString("BCV_OD"),
                                    BCV_OS = reader.GetString("BCV_OS"),
                                    BCV_OU = reader.GetString("BCV_OU"),
                                    CentreCode = reader.GetString("CentreCode"),
                                    ColourVision_BCV_OU = reader.GetString("ColourVision_BCV_OU"),
                                    ContactEmail = reader.GetString("ContactEmail"),
                                    ContactFirstName = reader.GetString("ContactFirstName"),
                                    ContactLastName = reader.GetString("ContactLastName"),
                                    ContactPhoneNumber = reader.GetString("ContactPhoneNumber"),
                                    ContrastSensitivity_BCV = reader.GetString("ContrastSensitivity_BCV"),
                                    CreatedBy = reader.GetString("CreatedBy"),
                                    //CreatedByUsername = reader.GetString("CreatedByUsername"),
                                    DateCreated = reader.GetDateTime("CreatedDate"),
                                    DigitalAddress = reader.GetString("DigitalAddress"),
                                    DistrictName = reader.GetString("DistrictName"),
                                    DOB = reader.GetDateTime("DOB"),
                                    FirstName = reader.GetString("FirstName"),
                                    //FormNumber = reader.GetString("FormNumber"),
                                    GlareTest_BCV_OD = reader.GetString("GlareTest_BCV_OD"),
                                    GlareTest_BCV_OS = reader.GetString("GlareTest_BCV_OS"),
                                    GlareTest_BCV_OU = reader.GetString("GlareTest_BCV_OU"),
                                    HX_BCV_OD = reader.GetString("HX_BCV_OD"),
                                    HX_BCV_OS = reader.GetString("HX_BCV_OS"),
                                    HX_BCV_OU = reader.GetString("HX_BCV_OU"),
                                    IsActive = reader.GetBoolean("IsActive"),
                                    IsGHDriveSynchronized = reader.GetBoolean("IsGHDriveSynchronized"),
                                    IsSynchronized = reader.GetBoolean("IsSynchronized"),
                                    MobileNumber = reader.GetString("MobileNumber"),
                                    //NameTitle = (NameTitle)reader.GetInt32("NameTitle"),
                                    //Optometrist = reader.GetString("Optometrist"),
                                    OptometristFirmId = reader.GetInt32("OptometristFirmId"),
                                    OtherName = reader.GetString("OtherName"),
                                    PassOrFail = (PassOrFail)reader.GetInt32("PassOrFail"),
                                    PassResult = (PassResult)reader.GetInt32("PassResult"),
                                    PathologicalRemarks = reader.GetString("PathologicalRemarks"),
                                    RegionName = reader.GetString("RegionName"),
                                    RegistrationNumber = reader.GetString("RegistrationNumber"),
                                    ResultConclusion = reader.GetString("ResultConclusion"),
                                    ResultServiceType = (ResultServiceType)reader.GetInt32("ResultServiceType"),
                                    SingleImage_BCV_OU = reader.GetString("SingleImage_BCV_OU"),
                                    Status = (Status)reader.GetInt32("Status"),
                                    Surname = reader.GetString("Surname"),
                                    TaxIdentificationNumber = reader.GetString("TaxIdentificationNumber"),
                                    TelephoneNumber = reader.GetString("TelephoneNumber"),
                                    Unaided_OD = reader.GetString("Unaided_OD"),
                                    Unaided_OS = reader.GetString("Unaided_OS"),
                                    Unaided_OU = reader.GetString("Unaided_OU"),
                                    //UpdatedByUsername = reader.GetString("UpdatedByUsername"),
                                    IsDeleted = reader.GetBoolean("IsDeleted"),
                                    IsRegistration = reader.GetBoolean("IsRegistration"),
                                    UpdatedBy = reader.GetString("ModifiedBy"),
                                    UserName = reader.GetString("UserName")
                                });
                            }
                        }
                    }
                }

                List<VisualAssessmentResultModel> visualAssessments = new();// _context.Database.SqlQuery<VisualAssessmentResultModel>("FetchVisualAssessmentResult @OptometristAdminID, @OptometristID, @Id", parameters).ToList();
                String PassportData = string.Empty;
                if (id.HasValue)
                {
                    var assessmentResult = _context.VisualAssessmentResults.AsNoTracking().FirstOrDefault(x => x.Id == id.Value);
                    if (assessmentResult != null)
                    {
                        PassportData = assessmentResult.PassportImageUrl;
                        foreach (var item in result)
                        {
                            item.PassportImageUrl = PassportData;
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

        public ResultViewModel FetchAssessmentResults(int displayLength, int displayStart, int sortCol, string sortDir, string search, Int64? optometricId)
        {
            ResultViewModel model = new();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("FetchVisualAssessmentResultsAdmin", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DisplayLength", displayLength);
                    cmd.Parameters.AddWithValue("@DisplayStart", displayStart);

                    cmd.Parameters.AddWithValue("@SortCol", sortCol);
                    cmd.Parameters.AddWithValue("@SortDir", sortDir);

                    cmd.Parameters.AddWithValue("@optometricId", optometricId ?? System.Data.SqlTypes.SqlInt64.Null);
                    cmd.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(search) ? System.Data.SqlTypes.SqlString.Null : search);


                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.aaData.Add(new VisualAssessmentResultItemViewModel
                            {
                                ApplicantAddress = !reader.IsDBNull(reader.GetOrdinal("ApplicantAddress")) ? reader.GetString("ApplicantAddress") : null,
                                ApplicantName = !reader.IsDBNull(reader.GetOrdinal("ApplicantName")) ? reader.GetString("ApplicantName") : null,
                                DSReference = !reader.IsDBNull(reader.GetOrdinal("DSReference")) ? reader.GetString("DSReference") : null,
                                OptometristFirmName = !reader.IsDBNull(reader.GetOrdinal("OptometristFirmName")) ? reader.GetString("OptometristFirmName") : null,
                                TestDate = !reader.IsDBNull(reader.GetOrdinal("TestDateString")) ? reader.GetString("TestDateString") : null,
                                Id = !reader.IsDBNull(reader.GetOrdinal("Id")) ? reader.GetInt64("Id") : 0,
                                Grade = !reader.IsDBNull(reader.GetOrdinal("Grade")) ? reader.GetString("Grade") : null,
                                Optometrist = !reader.IsDBNull(reader.GetOrdinal("Optometrist")) ? reader.GetString("Optometrist") : null,

                            });


                        }
                    }
                }
            }
            String PassportData = string.Empty;
            model.iTotalDisplayRecords = model.aaData.Count == 0 ? 0 : model.aaData.Count();
            model.iTotalRecords = GetApplicantViewModelTotalCount();

            return model;
        }

        public ResultViewModel FetchAssessmentResults(ClientSearchRequest model)
        {
            ResultViewModel result = new();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("FetchVisualAssessmentResultByOptometricFirmId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@optometricId", model.OptometristFirmId ?? System.Data.SqlTypes.SqlInt32.Null);
                    cmd.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(model.Search) ? System.Data.SqlTypes.SqlString.Null : model.Search);


                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.aaData.Add(new VisualAssessmentResultItemViewModel
                            {
                                ApplicantAddress = !reader.IsDBNull(reader.GetOrdinal("ApplicantAddress")) ? reader.GetString("ApplicantAddress") : null,
                                ApplicantName = !reader.IsDBNull(reader.GetOrdinal("ApplicantName")) ? reader.GetString("ApplicantName") : null,
                                DSReference = !reader.IsDBNull(reader.GetOrdinal("DSReference")) ? reader.GetString("DSReference") : null,
                                OptometristFirmName = !reader.IsDBNull(reader.GetOrdinal("OptometristFirmName")) ? reader.GetString("OptometristFirmName") : null,
                                TestDate = !reader.IsDBNull(reader.GetOrdinal("TestDateString")) ? reader.GetString("TestDateString") : null,
                                Id = !reader.IsDBNull(reader.GetOrdinal("Id")) ? reader.GetInt64("Id") : 0,
                                Grade = !reader.IsDBNull(reader.GetOrdinal("Grade")) ? reader.GetString("Grade") : null
                            });

                        }
                    }
                }
            }
            result.iTotalDisplayRecords = result.aaData.Count;
            result.iTotalRecords = GetApplicantViewModelTotalCount();

            return result;
        }

        public PaginationResponseModel<List<VisualAssessmentResultItemViewModel>> FetchAssessmentResults(PaginationRequestModel<ClientSearchRequest> model)
        {
            PaginationResponseModel<List<VisualAssessmentResultItemViewModel>> result = new() { ListResult = new(),  };
            //var offset = (model.PageSize - 1) * model.PageSize;
            //model.InputModel.StartDate = Utility.StartOfDay(model.InputModel.StartDate);
            //model.InputModel.EndDate = Utility.EndOfDay(model.InputModel.EndDate);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("FetchVisualAssessmentResultByOptometricFirmId", conn))
                {
                    cmd.CommandTimeout = 300000;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@optometricId", model.InputModel.OptometristFirmId ?? System.Data.SqlTypes.SqlInt32.Null);
                    cmd.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(model.InputModel.Search) ? System.Data.SqlTypes.SqlString.Null : model.InputModel.Search);
                    //cmd.Parameters.AddWithValue("@PageIndex", model.PageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", model.PageSize);
                    cmd.Parameters.AddWithValue("@Name", model.InputModel.Name ?? System.Data.SqlTypes.SqlString.Null);


                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.ListResult.Add(new VisualAssessmentResultItemViewModel
                            {
                                ResultConclusion= !reader.IsDBNull(reader.GetOrdinal("ResultConclusion")) ? reader.GetString("ResultConclusion") : null,
                                ApplicantAddress = !reader.IsDBNull(reader.GetOrdinal("ApplicantAddress")) ? reader.GetString("ApplicantAddress") : null,
                                ApplicantName = !reader.IsDBNull(reader.GetOrdinal("ApplicantName")) ? reader.GetString("ApplicantName") : null,
                                DSReference = !reader.IsDBNull(reader.GetOrdinal("DSReference")) ? reader.GetString("DSReference") : null,
                                OptometristFirmName = !reader.IsDBNull(reader.GetOrdinal("OptometristFirmName")) ? reader.GetString("OptometristFirmName") : null,
                                TestDate = !reader.IsDBNull(reader.GetOrdinal("TestDateString")) ? reader.GetString("TestDateString") : null,
                                Id = !reader.IsDBNull(reader.GetOrdinal("Id")) ? reader.GetInt64("Id") : 0,
                                Grade = !reader.IsDBNull(reader.GetOrdinal("Grade")) ? reader.GetString("Grade") : null,
                            });

                        }
                        reader.NextResult();

                        while (reader.Read())
                        {
                            result.TotalCount = reader.GetInt32("TotalCount");
                        }
                    }
                }
            }
            var _result = new PaginationResponseModel<List<VisualAssessmentResultItemViewModel>>(result.TotalCount, model.PageSize, result.ListResult.Count);
            result.TotalPages = _result.TotalPages;
            result.FilteredRecords = result.ListResult.Count;
            result.StartIndex = _result.StartIndex;
            result.EndIndex = _result.EndIndex;
            return result;
        }

        public List<VisualAssessmentResultModel> FetchAssessmentResultsAdmin(Int64? optometricId)
        {
            List<VisualAssessmentResultModel> result = new();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchVisualAssessmentResultAdmin", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@optometricId", optometricId ?? System.Data.SqlTypes.SqlInt64.Null);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new VisualAssessmentResultModel
                                {
                                    //DriversLicence = reader.GetString("DriversLicence"),
                                    //DVLAReferenceNo = reader.GetString("DVLAReferenceNo"),
                                    Email = reader.GetString("Email"),
                                    AccreditationNumber = reader.GetString("AccreditationNumber"),
                                    ReferenceNumber = reader.GetString("ReferenceNumber"),
                                    ContactNumber = reader.GetString("ContactNumber"),
                                    PostalAddress = reader.GetString("PostalAddress"),
                                    TestDate = reader.GetDateTime("TestDate"),
                                    BusinessAddress = reader.GetString("BusinessAddress"),
                                    BusinessName = reader.GetString("BusinessName"),
                                    Id = reader.GetInt64("Id"),
                                    //CreatedByFullName = reader.GetString("Fullname")
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

            // List<VisualAssessmentResultModel> visualAssessments = new();// _context.Database.SqlQuery<VisualAssessmentResultModel>("FetchVisualAssessmentResultAdmin @optometricId", parameters).ToList();
            //String PassportData = string.Empty;
            //if (id.HasValue)
            //{
            //    var assessmentResult = _context.VisualAssessmentResults.FirstOrDefault(x => x.Id == id.Value);
            //    if (assessmentResult != null)
            //    {
            //        PassportData = assessmentResult.PassportImageUrl;
            //        foreach (var item in visualAssessments)
            //        {
            //            item.PassportImageUrl = PassportData;
            //        }
            //    }
            //}
            return result;
        }

        public List<ColorVisionScoresModel> GetColorVisionScores()
        {
            string[] result = new string[24]; //_context.ColourVisionScores.FirstOrDefault().Score.Split(',');
            for (int i = 0; i < 24; i++)
            {
                result[i] = (i + 1).ToString();
            }


            var visionScores = new List<ColorVisionScoresModel>();
            foreach (var i in result)
            {
                var visionScore = new ColorVisionScoresModel
                {
                    Id = Int64.Parse(i.Trim()),
                    Value = Int64.Parse(i.Trim())
                };
                visionScores.Add(visionScore);
            }
            return visionScores;
        }

        private int GetApplicantViewModelTotalCount()
        {


            int count = _context.VisualAssessmentResults.AsNoTracking().Count();// _context.Database.SqlQuery<int>("select count(*) from VisualAssessmentResults").FirstOrDefault();
            return count;
        }

        public PaginationResponseModel<List<VisualAssessmentResultListItem>> GetVisualAssessmentResult(PaginationRequestModel pagination, int? optometristFirmId, Status? status, DateTime? startDate, DateTime? endDate, string DSReference)
        {
            var result = new PaginationResponseModel<List<VisualAssessmentResultListItem>>() { ListResult = new() };
            try
            {
                int? _status = status.HasValue ? (int)status.Value : null;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetVisualAssessmentResult", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OptometristFirmId", optometristFirmId ?? System.Data.SqlTypes.SqlInt32.Null);
                        cmd.Parameters.AddWithValue("@Status", _status ?? System.Data.SqlTypes.SqlInt32.Null);
                        cmd.Parameters.AddWithValue("@StartDate", startDate ?? System.Data.SqlTypes.SqlDateTime.Null);
                        cmd.Parameters.AddWithValue("@EndDate", endDate ?? System.Data.SqlTypes.SqlDateTime.Null);
                        cmd.Parameters.AddWithValue("@DSReference", DSReference ?? System.Data.SqlTypes.SqlString.Null);
                        cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.ListResult.Add(new VisualAssessmentResultListItem
                                {
                                    PassResult= reader.IsDBNull(reader.GetOrdinal("PassResult")) ? null : reader.GetString(reader.GetOrdinal("PassResult")),
                                    ResultServiceType = reader.IsDBNull(reader.GetOrdinal("ResultServiceType"))?null: (ResultServiceType)reader.GetInt32(reader.GetOrdinal("ResultServiceType")),
                                    //DVLAReferenceNo = reader.IsDBNull(reader.GetOrdinal("DVLAReferenceNo")) ? null : reader.GetString(reader.GetOrdinal("DVLAReferenceNo")),
                                    ReferenceNumber = reader.IsDBNull(reader.GetOrdinal("ReferenceNumber")) ? null : reader.GetString(reader.GetOrdinal("ReferenceNumber")),
                                    Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt64(reader.GetOrdinal("Id")),
                                    FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                                    OtherName = reader.IsDBNull(reader.GetOrdinal("OtherName")) ? null : reader.GetString(reader.GetOrdinal("OtherName")),
                                    Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? Status.InProgress : (Status)reader.GetInt32(reader.GetOrdinal("Status")),
                                    Surname = reader.IsDBNull(reader.GetOrdinal("Surname")) ? null : reader.GetString(reader.GetOrdinal("Surname")),
                                    TestDate = reader.IsDBNull(reader.GetOrdinal("TestDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("TestDate"))
                                });
                            }

                            reader.NextResult();

                            while (reader.Read())
                            {
                                result.TotalCount = reader.GetInt32("TotalCount");
                            }
                        }

                    }
                }
                var _result = new PaginationResponseModel<List<VisualAssessmentResultListItem>>(result.TotalCount, pagination.PageSize, result.ListResult.Count);
                result.StartIndex = _result.StartIndex;
                result.PageSize=_result.PageSize;
                result.EndIndex = _result.EndIndex;
                result.TotalPages = _result.TotalPages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }



            return result;
        }

        public List<SelectListItem> ResultConclusion()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Fit to drive", Value = "Fit to drive" });
            items.Add(new SelectListItem { Text = "Fit to drive with glasses", Value = "Fit to drive with glasses" });
            items.Add(new SelectListItem { Text = "Not fit to drive", Value = "Not fit to drive" });
            return items;
        }

        public async Task<MessageResponse> Transmit(VisualAssessmentTransmissionModel model)
        {
            MessageResponse result = new();
            var context = _context;
            var scope = await context.Database.BeginTransactionAsync();
            using (scope)
            {
                try
                {
                    var entity = await _context.VisualAssessmentResults.FirstOrDefaultAsync(x => x.IsTransmitted);
                    if (entity != null)
                    {
                        await scope.RollbackAsync();
                        result.Message = "Application has already been transmitted";
                        result.Success = false;
                        return result;
                    }

                    if (!string.IsNullOrEmpty(model.PassportBase64))
                    {
                        string passportFilePath = Path.Combine(_environment.WebRootPath, "Passports", model.PassportImageUrl);

                        byte[] imageBytes = Convert.FromBase64String(model.PassportBase64);

                        // Write the byte array to a file
                        File.WriteAllBytes(passportFilePath, imageBytes);

                    }

                    string referenceNumber = GenerateReferenceNo(model.OptometristFirmId, (Status)model.Status);

                    VisualAssessmentResult visualAssessmentResult = new()
                    {
                        AccessType = (AccessType)model.AccessType,
                        BCV_OD = model.BCV_OD,
                        BCV_OS = model.BCV_OS,
                        BCV_OU = model.BCV_OU,
                        ColourVision_BCV_OU = model.BCV_OU,
                        ContactNumber = model.ContactNumber,
                        ContrastSensitivity_BCV = model.ContrastSensitivity_BCV,
                        CreatedBy = model.CreatedBy,
                        CreatedDate = model.CreatedDate,
                        DOB = model.DOB,
                        //DriversLicence = model.DriversLicence,
                        //DVLAReferenceNo = model.DVLAReferenceNo,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        //FormNumber = model.FormNumber,
                        GlareTest_BCV_OD = model.GlareTest_BCV_OD,
                        GlareTest_BCV_OS = model.GlareTest_BCV_OS,
                        GlareTest_BCV_OU = model.GlareTest_BCV_OU,
                        HX_BCV_OD = model.HX_BCV_OD,
                        HX_BCV_OS = model.HX_BCV_OS,
                        HX_BCV_OU = model.HX_BCV_OU,
                        IsActive = model.IsActive,
                        IsDeleted = model.IsDeleted,
                        IsRegistration = model.IsRegistration,
                        IsSynchronized = model.IsSynchronized,
                        //NameTitle = (NameTitle)model.NameTitle,
                        //OldDVLAReferenceNo = model.OldDVLAReferenceNo,
                        OptometristFirmId = model.OptometristFirmId,
                        OtherName = model.OtherName,
                        PassOrFail = model.PassOrFail,
                        PassportImageUrl = model.PassportImageUrl,
                        PathologicalRemarks = model.PathologicalRemarks,
                        PassResult = (PassResult)model.PassResult,
                        PostalAddress = model.PostalAddress,
                        ReferenceNumber = referenceNumber,
                        ResultConclusion = model.ResultConclusion,
                        ResultServiceType = (ResultServiceType)model.ResultServiceType,
                        SingleImage_BCV_OU = model.SingleImage_BCV_OU,
                        Status = model.Status,
                        Surname = model.Surname,
                        Nationality = model.TaxIdentificationNumber,
                        TestDate = model.TestDate,
                        Unaided_OD = model.Unaided_OD,
                        Unaided_OS = model.Unaided_OS,
                        Unaided_OU = model.Unaided_OU,
                        IsTransmitted = true,
                        TransmittedDate = DateTime.UtcNow,
                        NationalID = model.NationalID,
                        PassportNumber = model.PassportNumber,
                        DvlaLicenseNumber = model.DvlaLicenseNumber
                    };
                    _context.VisualAssessmentResults.Add(visualAssessmentResult);
                    await _context.SaveChangesAsync();

                    if (_appSettings.Online)
                    {
                        Slot slot = _slotRepositoryQuery.FilterAsync(x => x.OptometristFirmId == model.OptometristFirmId && x.AccessType == ((ResultServiceType)model.ResultServiceType == ResultServiceType.LearnerDriversLicence ? AccessType.LearnerDriversLicence : AccessType.OtherLicenceCategory)).Result.FirstOrDefault();
                        if (slot == null)
                        {
                            await scope.RollbackAsync();
                            result.Message = "There is no available slot to continue with this assessment result";
                            return result;
                        }
                        if (slot.Quantity == 0)
                        {
                            await scope.RollbackAsync();
                            result.Message = "There is no available slot to continue with this assessment result";
                            return result;
                        }

                        if (model.Status == Status.Complete)
                        {
                            //Utilize Slot if Online

                            slot.Quantity = slot.Quantity - 1;
                            await context.SaveChangesAsync();


                            //Send Sms Notification
                            string passResult = model.PassOrFail.ToString(); //EnumHelper<PassOrFail>.GetDisplayValue(model.PassOrFail.GetValueOrDefault());

                            _smsRepository.SendAssessmentResult(model.FirstName, model.ContactNumber, referenceNumber, passResult, context);
                            //send email
                            _notificationRepository.SendAssessmentResult(model.FirstName, model.ContactNumber, referenceNumber, passResult, model.Email, context);
                        }
                    }
                    await scope.CommitAsync();
                    result.Message = model.FormNumber;
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    await scope.RollbackAsync();
                    _logger.LogError(ex.Message, ex);
                    result.Message = $"Message = {ex.Message}\r\n";
                    result.Message += $"Stack Trace = {ex.StackTrace}";
                }
            }
            
            return result;
        }

        public async Task<MessageResponse> LogBulkTransmission(List<VisualAssessmentTransmissionModel> data)
        {
            MessageResponse result = new();
            try
            {
                string transmissionData = JsonConvert.SerializeObject(data);

                VisualAssessmentTransmission visualAssessmentTransmission = new()
                {
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsTransmitted = false,
                    RecordCount = data.Count,
                    RetryCount = 0,
                    Data = transmissionData
                };
                _context.VisualAssessmentTransmissions.Add(visualAssessmentTransmission);
                await _context.SaveChangesAsync();
                result.Success = true;
                result.Message = "Data transmitted successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                result.Message = "An error occurred in the remote server while trying to process data";
            }
            return result;  
        }


        public async Task<MessageResponse<List<string>>> TransmitBulk(List<VisualAssessmentTransmissionModel> data)
        {
            MessageResponse<List<string>> result = new MessageResponse<List<string>>();
            List<string> formNumberList = new List<string>();
            foreach (VisualAssessmentTransmissionModel model in data)
            {
                var resultItem = await Transmit(model);
                if (resultItem.Success) formNumberList.Add(resultItem.Message);               
            }
            result.Success = formNumberList.Count > 0;
            result.Message = formNumberList.Count > 0 ? $"{formNumberList.Count} out of {data.Count} were transmitted successfully" : "All items were transmitted successfully";
            return result;
        }
    }
}
