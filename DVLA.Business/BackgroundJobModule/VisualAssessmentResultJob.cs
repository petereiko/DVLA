using DVLA.Business.ReportModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
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
    public class VisualAssessmentResultJob : IJob
    {
        private readonly ILogger<VisualAssessmentResultJob> _logger;
        private readonly AppSettings _appSettings;
        private readonly IReportRepository _reportRepository;
        private readonly DVLADbContext _context;

        public VisualAssessmentResultJob(ILogger<VisualAssessmentResultJob> logger, IOptions<AppSettings> options, IReportRepository reportRepository, DVLADbContext context)
        {
            _logger = logger;
            _appSettings = options.Value;
            _reportRepository = reportRepository;
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation($"Push Visuai Assessment Result Started");

                bool runPushAssessment = Convert.ToBoolean(_appSettings.RunPushAssessmentResult);

                _logger.LogInformation($"Service Started: {runPushAssessment}");

                if (!runPushAssessment) { return; }

                var visualAssessmentResults = _reportRepository.FetchAllPendingTransmissions();

                _logger.LogInformation($"{visualAssessmentResults.Count} results found");

                foreach (VisualAssessmentResultDto item in visualAssessmentResults)
                {
                    try
                    {
                        using var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, _appSettings.ApiVerificationPushUrl);
                        request.Headers.Add("X-API-KEY", _appSettings.ApiKey);
                        var requestBody = JsonConvert.SerializeObject(item);
                        _logger.LogInformation($"Request Body {requestBody}");
                        var content = new StringContent(requestBody, null, "application/json");
                        request.Content = content;
                        var response = client.SendAsync(request).GetAwaiter().GetResult();
                        _logger.LogInformation($"Response Object: {JsonConvert.SerializeObject(response)}");
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonSuccess = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            _logger.LogInformation($"SUCCESS Response JSON: {jsonSuccess}");
                            MessageResponse messageResponse = JsonConvert.DeserializeObject<MessageResponse>(jsonSuccess);
                            if (messageResponse.Success)
                            {
                                var visualAssessmentResult = _context.VisualAssessmentResults.FirstOrDefault(x => x.Id == item.VisualAssessmentResultId);

                                visualAssessmentResult.IsTransmitted = true;
                                visualAssessmentResult.TransmittedDate = messageResponse.Message.Equals("Record Exists") ? visualAssessmentResult.TransmittedDate : DateTime.UtcNow;
                                visualAssessmentResult.TransmissionError = null;
                                visualAssessmentResult.HasTransmissionError = false;
                                _context.SaveChanges();
                            }
                        }
                        else
                        {

                            string errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                            _logger.LogInformation($"Error Object: {errorContent}");

                            var assessment = _context.VisualAssessmentResults.FirstOrDefault(x => x.Id == item.Id);
                            assessment.IsTransmitted = false;
                            assessment.HasTransmissionError = true;
                            assessment.TransmissionError = errorContent;
                            _context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Could not reach the Push API");
                        _logger.LogError(ex.Message, ex);
                        continue;
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
