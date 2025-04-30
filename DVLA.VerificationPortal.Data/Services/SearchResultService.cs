using AutoMapper;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Application.Services
{
    public class SearchResultService: ISearchResultService
    {
        private readonly IGenericRepository<VisualAssessmentResult> _visualAssessmentResultRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<SearchResultService> _logger;
        private readonly IApiClientService _apiClientService;
        private readonly IConfiguration _configuration;

        public SearchResultService(IGenericRepository<VisualAssessmentResult> visualAssessmentResultRepository, IMapper mapper, IHttpContextAccessor contextAccessor, IHostingEnvironment environment, ILogger<SearchResultService> logger, IApiClientService apiClientService, IConfiguration configuration)
        {
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _environment = environment;
            _logger = logger;
            _apiClientService = apiClientService;
            _configuration = configuration;
        }

        public async Task<IEnumerable<VisualAssessmentResultDto>> GetResultsAsync(string searchTerm)
        {
            IEnumerable<VisualAssessmentResultDto> items = Enumerable.Empty<VisualAssessmentResultDto>();
            try
            {
                Expression<Func<VisualAssessmentResult, bool>> expression = v => v.ReferenceNumber.Contains(searchTerm) || v.FirstName.Contains(searchTerm) || v.Surname.Contains(searchTerm) || v.ContactNumber.Contains(searchTerm);
                IEnumerable<VisualAssessmentResult> results = await _visualAssessmentResultRepository.FilterAsync(expression, false);
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
                        PassConclusion = item.ResultConclusion,
                        Verified = item.IsVerified,
                        TestDate = item.TestDate,
                        TestType = EnumHelper.GetEnumDescription(item.ResultServiceType)
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

        public async Task<VisualAssessmentResultDto> GetResultAsync(int id)
        {
            VisualAssessmentResult result = await _visualAssessmentResultRepository.GetSingleAsync(x => x.Id == id, false);
            var model = _mapper.Map<VisualAssessmentResultDto>(result);
            //model.EncodedKey = Utility.EncryptUrlID((int)model.Id);
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

                VisualAssessmentResult record = await _visualAssessmentResultRepository.GetSingleAsync(x => x.VisualAssessmentResultId == entity.VisualAssessmentResultId, false);
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

        public async Task<MessageResponse> VerifyResult(string token)
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
                await _visualAssessmentResultRepository.UpdateAsync(assessment);
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

        public async Task<MessageResponse> VerifyResultByReferenceAsync(string referenceNumber)
        {
            MessageResponse response = new();
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
                await _visualAssessmentResultRepository.UpdateAsync(assessment);
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
    }
}
