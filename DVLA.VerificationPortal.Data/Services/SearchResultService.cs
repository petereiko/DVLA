using AutoMapper;
using Azure;
using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Domain.Entities;
using DVLA.VerificationPortal.Domain.Interfaces;
using DVLA.VerificationPortal.Shared;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Enums;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Application.Services
{
    [DisallowConcurrentExecution]
    public class SearchResultService: ISearchResultService
    {
        private readonly IGenericRepository<VisualAssessmentResult> _visualAssessmentResultRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<SearchResultService> _logger;
        private readonly IApiClientService _apiClientService;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IAuthUser _authUser;
        private readonly IAuditRepo _auditRepo;
        
        public SearchResultService(IGenericRepository<VisualAssessmentResult> visualAssessmentResultRepository, IMapper mapper, IHttpContextAccessor contextAccessor, IHostingEnvironment environment, ILogger<SearchResultService> logger, IApiClientService apiClientService, IConfiguration configuration, IUserRepository userRepository, IUserService userService, IAuthUser authUser, IAuditRepo auditRepo)
        {
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _environment = environment;
            _logger = logger;
            _apiClientService = apiClientService;
            _configuration = configuration;
            _userRepository = userRepository;
            _userService = userService;
            _authUser = authUser;
            _auditRepo = auditRepo;
        }

        public async Task<IEnumerable<VisualAssessmentResultDto>> GetResultsAsync(string searchTerm)
        {
            IEnumerable<VisualAssessmentResultDto> items = Enumerable.Empty<VisualAssessmentResultDto>();
            try
            {
    //            var result = people
    //.Where(p => (p.Firstname + " " + p.Surname)
    //    .ToLower()
    //    .Contains(fullNameToSearch.ToLower()))
    //.ToList();
                string trimmedServedSearchTerm = searchTerm.Trim(); 


                Expression<Func<VisualAssessmentResult, bool>> expression = v => v.ReferenceNumber.Contains(searchTerm) || v.FirstName.Contains(searchTerm) || v.Surname.Contains(searchTerm) || v.ContactNumber.Contains(searchTerm)
                || (v.FirstName + " " + v.Surname).ToLower().Contains(trimmedServedSearchTerm.ToLower()) || (v.Surname.ToLower() + " " + v.FirstName.ToLower()).Contains(trimmedServedSearchTerm.ToLower());
                IEnumerable<VisualAssessmentResult> results = await _visualAssessmentResultRepository.FilterAsync(expression, false);
                results = results.Take(20);
                items = _mapper.Map<List<VisualAssessmentResultDto>>(results);

                //foreach (var item in items)
                //{
                //    item.EncodedKey = Utility.EncryptUrlID((int)item.Id);
                //}
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
                        PassConclusion = EnumHelper.GetEnumDescription(item.PassResult),
                        Verified = item.IsVerified,
                        TestDate = item.TestDate,
                        TestType = EnumHelper.GetEnumDescription(item.ResultServiceType),
                        IdentityNumber = string.IsNullOrEmpty(item.PassportNumber) ? item.NationalID : item.PassportNumber,
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

        public async Task<VisualAssessmentResultDto> GetResultAsync(long id)
        {
            VisualAssessmentResult result = await _visualAssessmentResultRepository.GetSingleAsync(x => x.Id == id, false);
            var model = _mapper.Map<VisualAssessmentResultDto>(result);
            //model.EncodedKey = Utility.EncryptUrlID((int)model.Id);
            return model;
        }

        public async Task<VisualAssessmentResultDto> GetResultByReferenceAsync(string reference)
        {
            VisualAssessmentResult result = await _visualAssessmentResultRepository.GetSingleAsync(x => x.ReferenceNumber == reference, false);
            var model = _mapper.Map<VisualAssessmentResultDto>(result);
            return model;
        }

        public async Task<MessageResponse> PushBulk(VisualAssessmentResultDto result)
        {
            List<VisualAssessmentResult> entities = new();
            VisualAssessmentResult entity = _mapper.Map<VisualAssessmentResult>(result);

            VisualAssessmentResult record = await _visualAssessmentResultRepository.GetSingleAsync(x => x.VisualAssessmentResultId == entity.VisualAssessmentResultId, false);
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
                VisualAssessmentResult entity = _mapper.Map<VisualAssessmentResult>(model);

                VisualAssessmentResult record = await _visualAssessmentResultRepository.GetSingleAsync(x => x.ReferenceNumber == entity.ReferenceNumber, false);
                if (record != null) return new() { Message = "Record Exists", Success = true };
                entity.Id = 0;
                entity.TransmittedDate = DateTime.UtcNow;
                await _visualAssessmentResultRepository.AddAsync(entity);

                //await SendResultAsync(entity, CancellationToken.None);

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

        public async Task<MessageResponse> VerifyResult(string token, VerifyType verifyType)
        {
            MessageResponse response = new();
            try
            {
                long id = Utility.DecryptUrlID(token);
                VisualAssessmentResult assessment = await _visualAssessmentResultRepository.GetByIdAsync(id);
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
                    assessment.VerifiedBy = _authUser.UserId;
                }
                assessment.VerifyType = verifyType;
                await _visualAssessmentResultRepository.UpdateAsync(assessment);
                await _auditRepo.AddAuditAsync("Verify Result", $"Verified {assessment.ReferenceNumber}");
                response.Success = true;
                response.Message = "Result verified successfully";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return response;
        }

        public async Task<MessageResponse> VerifyResultByReference(string token, VerifyType verifyType)
        {
            MessageResponse response = new();
            try
            {
                VisualAssessmentResult assessment = await _visualAssessmentResultRepository.GetSingleAsync(x => x.ReferenceNumber == token);
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
                    assessment.VerifiedBy = _authUser.UserId;
                }
                assessment.VerifyType = verifyType;
                await _visualAssessmentResultRepository.UpdateAsync(assessment);
                await _auditRepo.AddAuditAsync("Verify Result", $"Verified {assessment.ReferenceNumber}");
                response.Success = true;
                response.Message = "Result verified successfully";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return response;
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
                    assessment.VerifiedBy = _authUser.UserId;
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


        public async Task<DvlaResponse> SendResultToGenesysAsync(VisualAssessmentResult model, CancellationToken cancellationToken)
        {
            DvlaResponse result = new();
            try
            {
                var payload = new
                {
                    dvlaSvcInvoiceNo = model.ReferenceNumber,
                    eyeTestResult = EnumHelper.GetEnumDescription(model.PassOrFail),
                    eyeTestDate = model.TestDate,
                    eyeTestRefNo = model.ReferenceNumber,
                    eyeTestOfficer = model.OptometristName,
                    eyeTestCenter = model.OptometristFirmName
                };
                string jsonRequest = JsonConvert.SerializeObject(payload);

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _configuration["DVLA:Url"]);
                request.Headers.Add("Authorization", $"Bearer {_configuration["DVLA:BearerToken"]}");
                var content = new StringContent(jsonRequest, null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request, cancellationToken);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    result = JsonConvert.DeserializeObject<DvlaResponse>(jsonResponse);
                }
                else
                {
                    result.msg = jsonResponse;
                    result.status = "0x";
                    result.code = "0x";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }

        public async Task ProcessGenesysAsync()
        {
            IEnumerable<VisualAssessmentResult> assessments = await _visualAssessmentResultRepository.FilterAsync(x => x.GenesisIsTranmitted != true && x.GenesisMessage == null && x.ReferenceNumber != null, true, 20);
            foreach (var assessmentResult in assessments)
            {
                DvlaResponse response = await SendResultToGenesysAsync(assessmentResult, CancellationToken.None);
                if (response.status== "ok")
                {
                    assessmentResult.GenesisIsTranmitted = true;
                    assessmentResult.GenesisMessage = response.msg;
                    assessmentResult.GenesisResponseCode = response.code;
                    assessmentResult.GenesisStatus = response.status;
                    assessmentResult.GenesisTransmittedDate = DateTime.UtcNow;
                }
                else
                {
                    assessmentResult.GenesisError = response.msg;
                    assessmentResult.GenesisResponseCode = response.code;
                    assessmentResult.GenesisStatus = response.status;
                    assessmentResult.GenesisIsTranmitted = false;
                }
                await _visualAssessmentResultRepository.UpdateAsync(assessmentResult);
            }
        }

    }
}
