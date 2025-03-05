using AutoMapper;
using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Domain.Entities;
using DVLA.VerificationPortal.Domain.Interfaces;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public SearchResultService(IGenericRepository<VisualAssessmentResult> visualAssessmentResultRepository, IMapper mapper, IHttpContextAccessor contextAccessor, IHostingEnvironment environment)
        {
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _environment = environment;
        }

        public async Task<IEnumerable<VisualAssessmentResultDto>> GetResultsAsync(string searchTerm)
        {
            Expression<Func<VisualAssessmentResult, bool>> expression = v => v.ReferenceNumber.Contains(searchTerm) || v.FirstName.Contains(searchTerm) || v.Surname.Contains(searchTerm) || v.ContactNumber.Contains(searchTerm);
            IEnumerable<VisualAssessmentResult> results = await _visualAssessmentResultRepository.FilterAsync(expression, false);
            return _mapper.Map<List<VisualAssessmentResultDto>>(results);
        }

        public async Task<VisualAssessmentResultDto> GetResultAsync(int id)
        {
            VisualAssessmentResult result = await _visualAssessmentResultRepository.GetSingleAsync(x=>x.VisualAssessmentResultId==id, false);
            return _mapper.Map<VisualAssessmentResultDto>(result);
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

        public async Task<MessageResponse> Push()
        {
            HttpRequest request = _contextAccessor.HttpContext.Request;
            string? json = request.Form["VisualAssessmentResult"];
            VisualAssessmentResultDto model =JsonConvert.DeserializeObject<VisualAssessmentResultDto>(json);
            if (model == null) return new() { Message = "No content" };

            string PassportFolder = Path.Combine(_environment.WebRootPath, "Passports");
            if (!Directory.Exists(PassportFolder)) Directory.Exists(PassportFolder);

            foreach (var file in request.Form.Files)
            {
                var filePath = Path.Combine(PassportFolder, file.FileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
            }

            List<VisualAssessmentResult> entities = new();
            VisualAssessmentResult entity = _mapper.Map<VisualAssessmentResult>(model);

            VisualAssessmentResult record = await _visualAssessmentResultRepository.GetSingleAsync(x => x.VisualAssessmentResultId == entity.VisualAssessmentResultId, false);
            if (record != null) return new() { Message = "Record Exists", Success = false };
            entity.Id = 0;
            await _visualAssessmentResultRepository.AddAsync(entity);
            return new() { Message = "Visual Assessment Result pushed successfully", Success = true };
        }
    }
}
