using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Infrastructure.Models;
using DVLA.VerificationPortal.Shared;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Enums;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Repositories
{
    public class SearchResultService : ISearchResultService
    {
        private readonly IGenericRepository<VisualAssessmentResult> _visualAssessmentResultRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<SearchResultService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IAuditRepo _auditRepo;
        private readonly IApiClientService _apiClientService;
        public SearchResultService(IGenericRepository<VisualAssessmentResult> visualAssessmentResultRepository, IHttpContextAccessor contextAccessor, IHostingEnvironment environment, ILogger<SearchResultService> logger, IConfiguration configuration, IUserRepository userRepository, IUserService userService, IAuditRepo auditRepo, IApiClientService apiClientService)
        {
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _contextAccessor = contextAccessor;
            _environment = environment;
            _logger = logger;
            _configuration = configuration;
            _userRepository = userRepository;
            _userService = userService;
            _auditRepo = auditRepo;
            _apiClientService = apiClientService;
        }

        public async Task<IEnumerable<VisualAssessmentResultDto>> GetResultsAsync(string searchTerm)
        {
            List<VisualAssessmentResultDto> items = new();
            try
            {
                string trimmedServedSearchTerm = searchTerm.Trim();


                Expression<Func<VisualAssessmentResult, bool>> expression = v => v.ReferenceNumber.Contains(searchTerm) || v.FirstName.Contains(searchTerm) || v.Surname.Contains(searchTerm) || v.ContactNumber.Contains(searchTerm)
                || (v.FirstName + " " + v.Surname).ToLower().Contains(trimmedServedSearchTerm.ToLower()) || (v.Surname.ToLower() + " " + v.FirstName.ToLower()).Contains(trimmedServedSearchTerm.ToLower());
                IEnumerable<VisualAssessmentResult> results = await _visualAssessmentResultRepository.FilterAsync(expression, false);
                results = results.Take(20);

                foreach (VisualAssessmentResult result in results)
                {
                    items.Add(GetDto(result));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return items;

        }

        public async Task<TestResultDto> GetResultAsync(string? reference)
        {
            TestResultDto? result = null;
            try
            {
                Expression<Func<VisualAssessmentResult, bool>> expression = v => v.ReferenceNumber.Equals(reference);
                IEnumerable<VisualAssessmentResult> results = await _visualAssessmentResultRepository.FilterAsync(expression, false);

                foreach (var item in results)
                {
                    result = new()
                    {
                        FullName = $"{item.Surname} {item.FirstName}",
                        PassConclusion = item.PassResult is null? "N/A": EnumHelper.GetEnumDescription(item.PassResult),
                        Verified = item.IsVerified,
                        TestDate = item.TestDate,
                        ResultServiceType = item.ResultServiceType,
                        ResultServiceTypeName = item.ResultServiceType is not null
                            ? EnumHelper.GetEnumDescription((ResultServiceType)item.ResultServiceType)
                            : "N/A",
                        DvlaLicenseNumber = item.DvlaLicenseNumber,
                        IdentityNumber = string.IsNullOrEmpty(item.PassportNumber)
                            ? item.NationalID
                            : item.PassportNumber,
                        IdentityType = string.IsNullOrEmpty(item.PassportNumber) ? "National ID" : "Passport Number",
                        ContactNumber = item.ContactNumber
                    };
                    try
                    {
                        result.Passport = await Utility.ConvertImageUrlToBase64($"{_configuration["AppConstants:PassportUrl"]}{item.PassportImageUrl}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;

        }


        public async Task<TestResultDto> GetTestByDSReferenceAsync(string? dsreference)
        {
            TestResultDto? result = null;
            try
            {
                Expression<Func<VisualAssessmentResult, bool>> expression = v => v.DvlaLicenseNumber.Equals(dsreference);
                IEnumerable<VisualAssessmentResult> results = await _visualAssessmentResultRepository.FilterAsync(expression, false);

                foreach (var item in results)
                {
                    result = new()
                    {
                        FullName = $"{item.Surname} {item.FirstName}",
                        PassConclusion = EnumHelper.GetEnumDescription(item.PassResult),
                        Verified = item.IsVerified,
                        TestDate = item.TestDate,
                        ResultServiceType = item.ResultServiceType,
                        ResultServiceTypeName = item.ResultServiceType is not null
                            ? EnumHelper.GetEnumDescription((ResultServiceType)item.ResultServiceType)
                            : "N/A",
                        DvlaLicenseNumber = item.DvlaLicenseNumber,
                        IdentityNumber = string.IsNullOrEmpty(item.PassportNumber)
                            ? item.NationalID
                            : item.PassportNumber,
                        IdentityType = string.IsNullOrEmpty(item.PassportNumber) ? "National ID" : "Passport Number"
                    };
                    try
                    {
                        result.Passport = await Utility.ConvertImageUrlToBase64($"{_configuration["AppConstants:PassportUrl"]}{item.PassportImageUrl}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;

        }


        private VisualAssessmentResultDto GetDto(VisualAssessmentResult entity)
        {
            VisualAssessmentResultDto model = new()
            {
                AccessType = entity.AccessType,
                BCV_OD = entity.BCV_OD,
                BCV_OS = entity.BCV_OS,
                BCV_OU = entity.BCV_OU,
                ReferenceNumber = entity.ReferenceNumber,
                VerifiedDate = entity.VerifiedDate,
                ColourVision_BCV_OU = entity.ColourVision_BCV_OU,
                ContactNumber = entity.ContactNumber,
                ContrastSensitivity_BCV = entity.ContrastSensitivity_BCV,
                CreatedDate = entity.CreatedDate,
                DOB = entity.DOB,
                DvlaLicenseNumber = entity.DvlaLicenseNumber,
                Email = entity.Email,
                FirstName = entity.FirstName,
                Gender = entity.Gender,
                GlareTest_BCV_OD = entity.GlareTest_BCV_OD,
                GlareTest_BCV_OS = entity.GlareTest_BCV_OS,
                GlareTest_BCV_OU = entity.GlareTest_BCV_OU,
                HX_BCV_OD = entity.HX_BCV_OD,
                HX_BCV_OS = entity.HX_BCV_OS,
                HX_BCV_OU = entity.HX_BCV_OD,
                Id = entity.Id,
                IsRegistration = entity.IsRegistration,
                IsVerified = entity.IsVerified,
                NationalID = entity.NationalID,
                Nationality = entity.NationalID,
                OptometristFirmId = entity.OptometristFirmId,
                OptometristFirmName = entity.OptometristFirmName,
                OptometristName = entity.OptometristName,
                OtherName = entity.OtherName,
                PassOrFail = entity.PassOrFail,
                PassportImageUrl = entity.PassportImageUrl,
                PassportNumber = entity.PassportNumber,
                PassResult = entity.PassResult,
                PathologicalRemarks = entity.PathologicalRemarks,
                PostalAddress = entity.PostalAddress,
                ResultConclusion = entity.ResultConclusion,
                ResultServiceType = (ResultServiceType?)entity.ResultServiceType,
                SingleImage_BCV_OU = entity.SingleImage_BCV_OU,
                Status = entity.Status,
                Surname = entity.Surname,
                TestDate = entity.TestDate,
                TestType = entity.TestType,
                TransmittedDate = entity.TransmittedDate,
                Unaided_OD = entity.Unaided_OD,
                Unaided_OS = entity.Unaided_OS,
                Unaided_OU = entity.Unaided_OU,
                VisualAssessmentResultId = entity.VisualAssessmentResultId
            };
            //model.EncodedKey = Utility.EncryptUrlID((int)model.Id);
            return model;
        }

        public async Task<VisualAssessmentResultDto> GetAssessmentResultAsync(string reference)
        {
            VisualAssessmentResult result = await _visualAssessmentResultRepository.GetSingleAsync(x => x.ReferenceNumber == reference, false);
            VisualAssessmentResultDto model = new()
            {
                AccessType = result.AccessType,
                BCV_OD = result.BCV_OD,
                BCV_OS = result.BCV_OS,
                BCV_OU = result.BCV_OU,
                ReferenceNumber = result.ReferenceNumber,
                VerifiedDate = result.VerifiedDate,
                ColourVision_BCV_OU = result.ColourVision_BCV_OU,
                ContactNumber = result.ContactNumber,
                ContrastSensitivity_BCV = result.ContrastSensitivity_BCV,
                CreatedDate = result.CreatedDate,
                DOB = result.DOB,
                DvlaLicenseNumber = result.DvlaLicenseNumber,
                Email = result.Email,
                FirstName = result.FirstName,
                Gender = result.Gender,
                GlareTest_BCV_OD = result.GlareTest_BCV_OD,
                GlareTest_BCV_OS = result.GlareTest_BCV_OS,
                GlareTest_BCV_OU = result.GlareTest_BCV_OU,
                HX_BCV_OD = result.HX_BCV_OD,
                HX_BCV_OS = result.HX_BCV_OS,
                HX_BCV_OU = result.HX_BCV_OD,
                Id = result.Id,
                IsRegistration = result.IsRegistration,
                IsVerified = result.IsVerified,
                NationalID = result.NationalID,
                Nationality = result.NationalID,
                OptometristFirmId = result.OptometristFirmId,
                OptometristFirmName = result.OptometristFirmName,
                OptometristName = result.OptometristName,
                OtherName = result.OtherName,
                PassOrFail = result.PassOrFail,
                PassportImageUrl = result.PassportImageUrl,
                PassportNumber = result.PassportNumber,
                PassResult = result.PassResult,
                PathologicalRemarks = result.PathologicalRemarks,
                PostalAddress = result.PostalAddress,
                ResultConclusion = result.ResultConclusion,
                ResultServiceType = (ResultServiceType?)result.ResultServiceType,
                SingleImage_BCV_OU = result.SingleImage_BCV_OU,
                Status = result.Status,
                Surname = result.Surname,
                TestDate = result.TestDate,
                TestType = result.TestType,
                TransmittedDate = result.TransmittedDate,
                Unaided_OD = result.Unaided_OD,
                Unaided_OS = result.Unaided_OS,
                Unaided_OU = result.Unaided_OU,
                VisualAssessmentResultId = result.VisualAssessmentResultId
            };
            //model.EncodedKey = Utility.EncryptUrlID((int)model.Id);
            return model;
        }

        public async Task<MessageResponse> PushBulk(VisualAssessmentResultDto result)
        {
            List<VisualAssessmentResult> entities = new();
            VisualAssessmentResult entity = new()
            {
                Id = result.Id,
                ReferenceNumber = result.ReferenceNumber,
                VisualAssessmentResultId = result.Id,
                AccessType = result.AccessType,
                BCV_OD = result.BCV_OD,
                BCV_OS = result.BCV_OS,
                BCV_OU = result.BCV_OU,
                ColourVision_BCV_OU = result.ColourVision_BCV_OU,
                ContactNumber = result.ContactNumber,
                ContrastSensitivity_BCV = result.ContrastSensitivity_BCV,
                CreatedDate = result.CreatedDate,
                CreatedBy = result.CreatedBy,
                DOB = result.DOB,
                DvlaLicenseNumber = result.DvlaLicenseNumber,
                Email = result.Email,
                FirstName = result.FirstName,
                Gender = result.Gender,
                GlareTest_BCV_OD = result.GlareTest_BCV_OD,
                GlareTest_BCV_OS = result.GlareTest_BCV_OS,
                GlareTest_BCV_OU = result.GlareTest_BCV_OU,
                HX_BCV_OD = result.HX_BCV_OD,
                HX_BCV_OS = result.HX_BCV_OS,
                HX_BCV_OU = result.HX_BCV_OU,
                IsRegistration = result.IsRegistration,
                NationalID = result.NationalID,
                OptometristFirmId = result.OptometristFirmId,
                Nationality = result.Nationality,
                OptometristFirmName = result.OptometristFirmName,
                OptometristName = result.OptometristName,
                OtherName = result.OtherName,
                PassOrFail = result.PassOrFail,
                PassportImageUrl = result.PassportImageUrl,
                PassportNumber = result.PassportNumber,
                PassResult = result.PassResult,
                PathologicalRemarks = result.PathologicalRemarks,
                PostalAddress = result.PostalAddress,
                ResultConclusion = result.ResultConclusion,
                ResultServiceType = (int?)result.ResultServiceType,
                SingleImage_BCV_OU = result.SingleImage_BCV_OU,
                Status = result.Status,
                Surname = result.Surname,
                TestDate = result.TestDate,
                TransmittedDate = DateTime.UtcNow,
                TestType = result.TestType,
                Unaided_OD = result.Unaided_OD,
                Unaided_OS = result.Unaided_OS,
                Unaided_OU = result.Unaided_OU,
                VerifiedDate = result.VerifiedDate
            };

            VisualAssessmentResult record = await _visualAssessmentResultRepository.GetSingleAsync(x => x.ReferenceNumber == entity.ReferenceNumber, false);
            if (record != null) return new() { Message = "Record Exists", Success = false };
            entity.Id = 0;
            await _visualAssessmentResultRepository.AddAsync(entity);
            return new() { Message = "Visual Assessment Result pushed successfully", Success = true };
        }

        public async Task<MessageResponse> Push(VisualAssessmentResultDto model)
        {
            try
            {
                if (model == null) return new() { Message = "No content" };

                List<VisualAssessmentResult> entities = new();
                VisualAssessmentResult entity = new()
                {
                    Id = model.Id,
                    AccessType = model.AccessType,
                    BCV_OD = model.BCV_OD,
                    BCV_OS = model.BCV_OS,
                    BCV_OU = model.BCV_OU,
                    ColourVision_BCV_OU = model.ColourVision_BCV_OU,
                    ContactNumber = model.ContactNumber,
                    ContrastSensitivity_BCV = model.ContrastSensitivity_BCV,
                    CreatedBy = model.CreatedBy,
                    CreatedDate = model.CreatedDate,
                    DOB = model.DOB,
                    DvlaLicenseNumber = model.DvlaLicenseNumber,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    Gender = model.Gender,
                    GlareTest_BCV_OD = model.GlareTest_BCV_OD,
                    GlareTest_BCV_OS = model.GlareTest_BCV_OS,
                    GlareTest_BCV_OU = model.GlareTest_BCV_OU,
                    HX_BCV_OD = model.HX_BCV_OD,
                    HX_BCV_OS = model.HX_BCV_OS,
                    HX_BCV_OU = model.HX_BCV_OU,
                    IsVerified = model.IsVerified,
                    ResultServiceType = (int)model.ResultServiceType,
                    IsRegistration = model.IsRegistration,
                    NationalID = model.NationalID,
                    Nationality = model.Nationality,
                    OptometristFirmId = model.OptometristFirmId,
                    OptometristFirmName = model.OptometristFirmName,
                    OptometristName = model.OptometristName,
                    OtherName = model.OtherName,
                    PassOrFail = model.PassOrFail,
                    PassportImageUrl = model.PassportImageUrl,
                    PassportNumber = model.PassportNumber,
                    PathologicalRemarks = model.PathologicalRemarks,
                    PostalAddress = model.PostalAddress,
                    PassResult = model.PassResult,
                    ReferenceNumber = model.ReferenceNumber,
                    ResultConclusion = model.ResultConclusion,
                    SingleImage_BCV_OU = model.SingleImage_BCV_OU,
                    Status = model.Status,
                    Surname = model.Surname,
                    TestDate = model.TestDate,
                    TestType = model.TestType,
                    TransmittedDate = model.TransmittedDate,
                    Unaided_OD = model.Unaided_OD,
                    Unaided_OS = model.Unaided_OS,
                    Unaided_OU = model.Unaided_OU,
                    VerifiedDate = model.VerifiedDate,
                    VisualAssessmentResultId = model.VisualAssessmentResultId,
                    InvoiceNumber = model.InvoiceNumber
                };

                VisualAssessmentResult record = await _visualAssessmentResultRepository.GetSingleAsync(x => x.ReferenceNumber == entity.ReferenceNumber, false);
                if (record != null) return new() { Message = "Record Exists", Success = true };
                entity.Id = 0;
                await _visualAssessmentResultRepository.AddAsync(entity);
                return new() { Message = "Visual Assessment Result pushed successfully", Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new() { Message = ex.Message, Success = false };
            }
        }

        public async Task<MessageResponse> UpdateAuthDoctor(UpdateDocRequestDto model)
        {
            try
            {
                if (model == null) return new() { Message = "No content" };

                //List<VisualAssessmentResult> entities = new();
                //VisualAssessmentResult entity = _mapper.Map<VisualAssessmentResult>(model);

                VisualAssessmentResult record = await _visualAssessmentResultRepository.GetSingleAsync(x => x.VisualAssessmentResultId == model.VisualAssessmentResultId && x.ReferenceNumber == model.ReferenceNumber);
                if (record != null)
                {
                    if (record.OptometristName != model.OptometristName)
                    {
                        record.OptometristName = model.OptometristName;
                        await _visualAssessmentResultRepository.UpdateAsync(record);
                    }
                }
                return new() { Message = "Visual Assessment Result update successfully", Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new() { Message = ex.Message, Success = false };
            }
        }

        public async Task<MessageResponse<string>> VerifyResultByReferenceAsync(string referenceNumber, VerifyType verifyType)
        {
            MessageResponse<string> response = new();
            try
            {
                VisualAssessmentResult assessment = await _visualAssessmentResultRepository.GetSingleAsync(x => x.ReferenceNumber == referenceNumber);
                if (assessment == null)
                {
                    response.Message = "Record not found";
                    return response;
                }
                if (assessment.IsVerified)
                {
                    response.Message = "This result is already verified";
                    return response;
                }
                assessment.IsVerified = true;
                assessment.VerifiedDate = DateTime.UtcNow;
                if (verifyType == VerifyType.API)
                {
                    assessment.VerifiedBy = _apiClientService.ApiKey;
                }
                else
                {
                    //UserProperty userProperty = CookieSessionHelper.GetUserId(_contextAccessor.HttpContext);
                    assessment.VerifiedBy = _contextAccessor.HttpContext.User.Identity.GetUserId();
                    await _auditRepo.AddAuditAsync("Verify Result", $"Verified {assessment.ReferenceNumber}");
                }
                assessment.VerifyType = verifyType;
                await _visualAssessmentResultRepository.UpdateAsync(assessment);
                response.Success = true;
                response.Message = "Result verified successfully";
                response.Result = EnumHelper.GetEnumDescription(assessment.PassResult);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return response;
        }

        //public async Task<MessageResponse> VerifyResult(string token, VerifyType verifyType)
        //{
        //    MessageResponse response = new();
        //    try
        //    {
        //        long id = Utility.DecryptUrlID(token);
        //        VisualAssessmentResult assessment = await _visualAssessmentResultRepository.GetByIdAsync(id);
        //        if (assessment == null)
        //        {
        //            response.Message = "Record not found";
        //            return response;
        //        }
        //        if (assessment.IsVerified)
        //        {
        //            response.Message = "This result is already verified";
        //            return response;
        //        }
        //        assessment.IsVerified = true;
        //        assessment.VerifiedDate = DateTime.UtcNow;
        //        if (verifyType == VerifyType.API)
        //        {
        //            assessment.VerifiedBy = _apiClientService.ApiKey;
        //        }
        //        else
        //        {
        //            assessment.VerifiedBy = _userProperty.Id;
        //        }
        //        assessment.VerifyType = verifyType;
        //        await _visualAssessmentResultRepository.UpdateAsync(assessment);
        //        await _auditRepo.AddAuditAsync("Verify Result", $"Verified {assessment.ReferenceNumber}");
        //        response.Success = true;
        //        response.Message = "Result verified successfully";
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message, ex);
        //    }
        //    return response;
        //}

        //public async Task<MessageResponse<string>> VerifyResultByReferenceAsync(string referenceNumber, VerifyType verifyType)
        //{
        //    MessageResponse<string> response = new();
        //    try
        //    {
        //        VisualAssessmentResult assessment = await _visualAssessmentResultRepository.GetSingleAsync(x => x.ReferenceNumber == referenceNumber);
        //        if (assessment == null)
        //        {
        //            response.Message = "Record not found";
        //            return response;
        //        }
        //        if (assessment.IsVerified)
        //        {
        //            response.Message = "This result is already verified";
        //            return response;
        //        }
        //        assessment.IsVerified = true;
        //        assessment.VerifiedDate = DateTime.UtcNow;
        //        if (verifyType == VerifyType.API)
        //        {
        //            assessment.VerifiedBy = _apiClientService.ApiKey;
        //        }
        //        else
        //        {
        //            assessment.VerifiedBy = _userProperty.Id;
        //            await _auditRepo.AddAuditAsync("Verify Result", $"Verified {assessment.ReferenceNumber}");
        //        }
        //        assessment.VerifyType = verifyType;
        //        await _visualAssessmentResultRepository.UpdateAsync(assessment);
        //        response.Success = true;
        //        response.Message = "Result verified successfully";
        //        response.Result = EnumHelper.GetEnumDescription(assessment.PassResult);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message, ex);
        //    }
        //    return response;
        //}

        public async Task ProcessGenesysAsync()
        {
            var records = await _visualAssessmentResultRepository.FilterAsync(x => x.GenesisIsTranmitted != true && x.GenesisStatus == null && x.GenesisError == null && x.InvoiceNumber != null, true, 20);
            foreach (var record in records)
            {
                var result = await Transmit(record);
                if (result == null)
                {
                    record.GenesisError = "Exception Caught";
                    record.GenesisStatus = null;
                    await _visualAssessmentResultRepository.UpdateAsync(record);
                }
                else
                {
                    record.GenesisIsTranmitted = result.code == "00";
                    record.GenesisMessage = result.msg;
                    record.GenesisStatus = result.status;
                    record.GenesisResponseCode = result.code;
                    record.GenesisTransmittedDate = record.GenesisIsTranmitted.GetValueOrDefault() ? DateTime.Now : null;
                    await _visualAssessmentResultRepository.UpdateAsync(record);
                }
            }
        }

        public async Task<GenesysResponse?> Transmit(VisualAssessmentResult assessment)
        {
            GenesysResponse? genesysResponse = null;
            try
            {
                var payload = new
                {
                    dvlaSvcInvoiceNo = assessment.InvoiceNumber,
                    eyeTestResult = EnumHelper.GetEnumDescription(assessment.PassOrFail),
                    eyeTestDate = assessment.TestDate?.ToString("yyyy-MM-dd hh:mm:ss"),
                    eyeTestRefNo = assessment.ReferenceNumber,
                    eyeTestOfficer = assessment.OptometristName,
                    eyeTestCenter = assessment.OptometristFirmName
                };

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("https://online.dvla.gov.gh/api/dlams/eyetest/result"),
                        Headers =
                                    {
                                        { "authorization", "Bearer 005fxqNKPDc69TAxblXuKVyaOGYX0OBqjDXBgUL9MolokHZocdAVmhRZdvsB" },
                                    },
                        Content = new StringContent(JsonSerializer.Serialize(payload))
                        {
                            Headers =
                                    {
                                        ContentType = new MediaTypeHeaderValue("application/json")
                                    }
                        }
                    };
                    using (var response = await client.SendAsync(request))
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        genesysResponse = JsonSerializer.Deserialize<GenesysResponse>(body);
                    }
                }

                return genesysResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return genesysResponse;
            }
        }
    }
}
