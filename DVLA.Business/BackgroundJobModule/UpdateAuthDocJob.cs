using DVLA.Business.ReportModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.DATA.Domains;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.BackgroundJobModule
{
    [DisallowConcurrentExecution]
    public class UpdateAuthDocJob : IJob
    {
        private readonly ILogger<UpdateAuthDocJob> _logger;
        private readonly IReportRepository _reportRepository;
        private readonly AppSettings _appSettings;
        private readonly DVLADbContext _context;

        public UpdateAuthDocJob(ILogger<UpdateAuthDocJob> logger, IReportRepository reportRepository, IOptions<AppSettings> options, DVLADbContext context)
        {
            _logger = logger;
            _reportRepository = reportRepository;
            _appSettings = options.Value;
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation($"Update Started");

                var visualAssessmentResults = _reportRepository.FetchAllPendingAuthDocUpdate();

                _logger.LogInformation($"{visualAssessmentResults.Count} results found");

                foreach (UpdateDocRequestDto item in visualAssessmentResults)
                {
                    try
                    {
                        using var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, _appSettings.ApiVerificationUpdateDocUrl);
                        request.Headers.Add("X-API-KEY", _appSettings.ApiKey);
                        var requestBody = JsonConvert.SerializeObject(item);
                        _logger.LogInformation($"Request Body {requestBody}");
                        var content = new StringContent(requestBody, null, "application/json");
                        request.Content = content;
                        var response = await client.SendAsync(request);
                        _logger.LogInformation($"Response Object: {JsonConvert.SerializeObject(response)}");
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonSuccess = await response.Content.ReadAsStringAsync();
                            MessageResponse messageResponse = JsonConvert.DeserializeObject<MessageResponse>(jsonSuccess);
                            if (messageResponse.Success)
                            {
                                VisualAssessmentResult visualAssessmentResult = _context.VisualAssessmentResults.FirstOrDefault(x => x.Id == item.VisualAssessmentResultId);

                                visualAssessmentResult.OptometristNameIsUpdate = true;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Could not reach the Push API");
                        _logger.LogError(ex.Message, ex);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
