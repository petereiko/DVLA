using DVLA.DATA.Domains;
using DVLA.Data.Models.DataObjects.PaystackDtos;
using DVLA.Data.Models.Domains;
using DVLA.Data.Models.Enumerables;
using DVLA.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DVLA.Business.PaymentModule
{
    public class PaymentService : IPaymentService
    {
        private readonly string _connectionString;
        private readonly ILogger<PaymentService> _logger;
        private readonly IConfiguration _configuration;
        public PaymentService(IConfiguration configuration, ILogger<PaymentService> logger)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public VerificationResponse VerifyPayment(string reference)
        {
            VerificationResponse response = null;
            VerificationBadRequestResponse error = null;

            var optionsBuilder = new DbContextOptionsBuilder<DVLADbContext>();
            optionsBuilder.UseSqlServer(_connectionString);
            DVLADbContext context = new DVLADbContext(optionsBuilder.Options);
            var scope = context.Database.BeginTransaction();

            using (scope)
            {
                try
                {
                    SlotRequest slotRequest = context.SlotRequests.FirstOrDefault(x => x.PaymentMethod == PaymentMethod.Online && x.ReferenceNumber == reference);
                    if (slotRequest == null)
                    {
                        scope.Rollback();
                        response.message = "Slot Request not found";
                        return response;
                    }
                    if (slotRequest.Status == SlotRequestStatus.Approved)
                    {
                        scope.Rollback();
                        response.message = "Slot Request has been approved";
                        return response;
                    }

                    PaystackVerification verification = context.PaystackVerifications.FirstOrDefault(x => x.Reference == slotRequest.ReferenceNumber);
                    if (verification != null && verification.RetryCount > 5)
                    {
                        scope.Rollback();
                        response.message = "Number of retries exceeded";
                        return response;
                    }

                    //Call Paystack Verification Endpoint
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.paystack.co/transaction/verify/{reference}");
                    string secret = _configuration["AppConstants:PaystackSecretKey"];
                    request.Headers.Add("Authorization", $"Bearer {secret}");
                    var apiResponse = client.SendAsync(request).GetAwaiter().GetResult();
                    var json = apiResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    bool transactionSuccessful = false;
                    if (apiResponse.IsSuccessStatusCode)
                    {
                        response = JsonConvert.DeserializeObject<VerificationResponse>(json);
                        transactionSuccessful = response.data.status == "success";

                        slotRequest.Status = transactionSuccessful ? SlotRequestStatus.Approved : SlotRequestStatus.Reject;
                        slotRequest.ModifiedDate = DateTime.Now;
                        slotRequest.DateApproved = transactionSuccessful ? DateTime.Now : null;
                        if (transactionSuccessful)
                        {
                            Slot slot = context.Slots.FirstOrDefault(x => x.OptometristFirmId == slotRequest.OptometristFirmId && x.AccessType == slotRequest.AccessType);
                            if (slot == null)
                            {
                                slot = new()
                                {
                                    OptometristFirmId = slotRequest.OptometristFirmId,
                                    AccessType = slotRequest.AccessType,
                                    CreatedBy = slotRequest.CreatedBy,
                                    Quantity = slotRequest.Quantity,
                                    ReorderLevel = 0
                                };
                                context.Slots.Add(slot);
                            }
                            else
                            {
                                slot.Quantity += slotRequest.Quantity;
                                slot.ModifiedDate = DateTime.Now;
                            }
                        }
                    }


                    else
                    {
                        error = JsonConvert.DeserializeObject<VerificationBadRequestResponse>(json);
                        slotRequest.Status = SlotRequestStatus.Reject;
                        slotRequest.ModifiedDate = DateTime.Now;

                    }
                    context.SaveChanges();


                    if (verification == null)
                    {
                        verification = new()
                        {
                            CreatedBy = slotRequest.CreatedBy,
                            Reference = slotRequest.ReferenceNumber,
                            RetryCount = 0,
                            SlotRequestId = slotRequest.Id,
                            Success = transactionSuccessful,
                            TranId = transactionSuccessful ? response.data.Id : null,
                            VerificationData = response != null ? JsonConvert.SerializeObject(response.data) : JsonConvert.SerializeObject(error)
                        };
                        context.PaystackVerifications.Add(verification);
                    }
                    else
                    {
                        verification.TranId = transactionSuccessful ? response.data.Id : null;
                        verification.RetryCount = verification.RetryCount + 1;
                        verification.VerificationData = response != null ? JsonConvert.SerializeObject(response.data) : JsonConvert.SerializeObject(error);
                        verification.ModifiedDate = DateTime.Now;
                        verification.ModifiedBy = slotRequest.ModifiedBy;
                        verification.Success = transactionSuccessful;
                    }
                    context.SaveChanges();
                    scope.Commit();
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception details
                    var entry = ex.Entries.Single();
                    var entityType = entry.Metadata.Name;
                    var state = entry.State;
                    // Handle or log the error as necessary
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    _logger.LogError(ex.Message, ex);
                }

            }
            response = response == null ? new VerificationResponse() { message = error.message } : response;

            return response;
        }

        public async Task<InitiatePaymentResponse> InitiatePayment(InitiatePaymentRequest model)
        {
            InitiatePaymentResponse result = new();

            model.reference = DateTime.Now.Ticks.ToString();

            var optionsBuilder = new DbContextOptionsBuilder<DVLADbContext>();
            optionsBuilder.UseSqlServer(_connectionString);
            DVLADbContext context = new DVLADbContext(optionsBuilder.Options);

            var scope = await context.Database.BeginTransactionAsync();
            using (scope)
            {
                try
                {
                    InitiatePaystackTransferRequest paymentRequest = new()
                    {
                        Amount = Convert.ToDecimal(model.amount),
                        CreatedBy = model.UserId,
                        CreatedDate = DateTime.Now,
                        Email = model.email,
                        IsActive = true,
                        OptometristFirmId = model.OptometristFirmId,
                        Reference = model.reference
                    };
                    context.InitiatePaystackTransferRequests.Add(paymentRequest);
                    await context.SaveChangesAsync();

                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.paystack.co/transaction/initialize");



                    request.Headers.Add("Authorization", "Bearer sk_test_4410c12527c1882602431956acf855b79f82f6bd");
                    string amt = (paymentRequest.Amount * 100M).ToString();
                    model.amount = amt;
                    string json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        json = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<InitiatePaymentResponse>(json);
                    }
                    else
                    {
                        await scope.RollbackAsync();
                        result.message = "Could not initiate the transaction at the moment. Please try again later";
                        return result;
                    }
                    InitiatePaystackTransferResponse paymentResponse = new()
                    {
                        Message = result.message,
                        Reference = model.reference,
                        AccessCode = result.data == null ? null : result.data.access_code,
                        AuthorizationUrl = result.data == null ? null : result.data.authorization_url,
                        CreatedDate = DateTime.Now,
                        Status = result.status,
                        InitiatePaystackTransferRequestId = paymentRequest.Id
                    };
                    context.InitiatePaystackTransferResponses.Add(paymentResponse);
                    await context.SaveChangesAsync();

                    SlotPrice slotPrice = context.SlotPrices.FirstOrDefault(x => x.AccessType == (AccessType)model.accessType && x.IsActive);
                    if (slotPrice == null)
                    {
                        await scope.RollbackAsync();
                        result.message = "Could not compute slot unit price";
                        return result;
                    }

                    decimal perUnitPrice = slotPrice.Price;
                    decimal output = paymentRequest.Amount / perUnitPrice;
                    int Quantity = (int)Math.Truncate(output);


                    SlotRequest slotRequest = new()
                    {
                        AccessType = (AccessType)model.accessType,
                        PaymentMethod = PaymentMethod.Online,
                        AmountPaid = paymentRequest.Amount,
                        Comment = null,
                        CreatedDate = DateTime.Now,
                        InitiatePaystackTransferRequestId = paymentRequest.Id,
                        OptometristFirmId = paymentRequest.OptometristFirmId,
                        Quantity = Quantity,
                        ReferenceNumber = paymentRequest.Reference,
                        Status = SlotRequestStatus.Pending,
                        CreatedBy = paymentRequest.CreatedBy
                    };
                    context.SlotRequests.Add(slotRequest);
                    await context.SaveChangesAsync();

                    await scope.CommitAsync();
                }
                catch (Exception ex)
                {
                    await scope.RollbackAsync();
                    _logger.LogError(ex.Message, ex);
                }
            }
            return result;
        }
    }
}
