using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using Microsoft.EntityFrameworkCore;
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
    public class SyncOptometristFirmJob : IJob
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<SyncOptometristFirmJob> _logger;
        private readonly AppSettings _appSettings;

        public SyncOptometristFirmJob(DVLADbContext context, ILogger<SyncOptometristFirmJob> logger, IOptions<AppSettings> options)
        {
            _context = context;
            _logger = logger;
            _appSettings = options.Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation($"Update Started");

                var optometristFirms = _context.OptometristFirms.AsNoTracking().Where(x => x.IsSynchronized != true).ToList();

                if (optometristFirms.Count == 0) return;

                _logger.LogInformation($"{optometristFirms.Count} results found");

                IEnumerable<OptometristFirmTransmissionDto> transmissions = optometristFirms
                    .Select(x => new OptometristFirmTransmissionDto
                    {
                        AccreditationNumber = x.AccreditationNumber,
                        BusinessAddress = x.BusinessAddress,
                        BusinessName = x.BusinessName,
                        CentreCode = x.CentreCode,
                        ContactEmail = x.ContactEmail,
                        ContactFirstName = x.ContactFirstName,
                        ContactLastName = x.ContactLastName,
                        ContactPhoneNumber = x.ContactPhoneNumber,
                        CreatedBy = x.CreatedBy,
                        CreatedDate = x.CreatedDate,
                        DigitalAddress = x.DigitalAddress,
                        DistrictId = x.DistrictId,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        IsSynchronized = true,
                        MobileNumber = x.MobileNumber,
                        ModifiedBy = string.IsNullOrEmpty(x.ModifiedBy) ? "" : x.ModifiedBy,
                        ModifiedDate = x.ModifiedDate,
                        OptometristFirmId = x.Id,
                        RegionId = x.RegionId,
                        RegistrationNumber = x.RegistrationNumber,
                        ReorderLevel = x.ReorderLevel,
                        TelephoneNumber = x.TelephoneNumber,
                        Town = x.Town
                    });

                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _appSettings.ApiVerificationTransmitOpometristFirmsUrl);
                request.Headers.Add("X-API-KEY", _appSettings.ApiKey);
                var requestBody = JsonConvert.SerializeObject(transmissions);
                _logger.LogInformation($"Request Body {requestBody}");
                var content = new StringContent(requestBody, null, "application/json");
                request.Content = content;
                var response = client.SendAsync(request).GetAwaiter().GetResult();
                _logger.LogInformation($"Response Object: {JsonConvert.SerializeObject(response)}");
                var jsonSuccess = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {

                    List<int> returnedOptometristIds = JsonConvert.DeserializeObject<List<int>>(jsonSuccess);
                    if (returnedOptometristIds.Count > 0)
                    {

                        foreach (var id in returnedOptometristIds)
                        {
                            var opt = _context.OptometristFirms.FirstOrDefault(x => x.Id == id);
                            opt.IsSynchronized = true;
                            _context.SaveChanges();
                        }
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
