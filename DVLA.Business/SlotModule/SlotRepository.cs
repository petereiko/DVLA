using Azure;
using DVLA.Business.EmailModule;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.Auth;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.PaystackDtos;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.Domains;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NPOI.HSSF.Record;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.SlotModule
{
    public class SlotRepository : ISlotRepository
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<SlotRepository> _logger;
        private readonly IHostingEnvironment _environment;
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _connectionString;
        private readonly IAuthUser _authUser;
        private static readonly object _lock = new object();

        public SlotRepository(DVLADbContext context, IConfiguration configuration, ILogger<SlotRepository> logger, IHostingEnvironment environment, IUserService userService, IOptions<AppSettings> options, IEmailService emailService, UserManager<ApplicationUser> userManager, IAuthUser authUser)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
            _userService = userService;
            _appSettings = options.Value;
            _emailService = emailService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _authUser = authUser;
        }

        public List<PriceModel> AmountPerSlot()
        {
            var slotPrice = _context.SlotPrices.AsNoTracking().Where(x => x.IsActive)
                .Select(p => new PriceModel { AccessType = p.AccessType, SlotMarketPrice = p.Price }).ToList();
            return slotPrice;
        }

        public MessageResponse ApproveSlotRequest(int id)
        {
            MessageResponse response = new();
            var context = _context;
            var transaction = context.Database.BeginTransaction();
            using (transaction)
            {
                try
                {
                    SlotRequest slotRequest = context.SlotRequests.FirstOrDefault(p => p.Id == id && (p.Status == SlotRequestStatus.Pending || p.Status == SlotRequestStatus.Opened));
                    if (slotRequest == null)
                    {
                        response.Message = "The record does not exist";
                        return response;
                    }

                    //if (slotRequest.Status != SlotRequestStatus.Pending)
                    //{
                    //    response.Message = "Request has already been treated, duplicate!";
                    //    return response;
                    //}
                    slotRequest.Status = SlotRequestStatus.Approved;
                    slotRequest.DateApproved = DateTime.Now;
                    slotRequest.ModifiedDate = DateTime.Now;
                    slotRequest.ModifiedBy = _authUser.UserId;
                    context.SaveChanges();

                    //Add the number of purchased slot to existing

                    Slot slot = context.Slots.FirstOrDefault(x => x.OptometristFirmId == slotRequest.OptometristFirmId && x.AccessType == slotRequest.AccessType);
                    if (slot != null)
                    {
                        slot.Quantity += slotRequest.Quantity;
                        slot.ModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        slot = new Slot
                        {
                            OptometristFirmId = slotRequest.OptometristFirmId,
                            Quantity = slotRequest.Quantity,
                            AccessType = slotRequest.AccessType,
                            ReorderLevel = 5
                        };
                        context.Slots.Add(slot);
                    }
                    context.SaveChanges();
                    transaction.Commit();

                    response.Message = "Request approved successfully";
                    response.Success = true;

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex.Message, ex);
                    response.Message = "The data for approval has been updated by another person";
                    response.Success = false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    response.Message = "An error occurred while trying to approve the request";
                }
            }

            return response;
        }

        public MessageResponse ComputeSlotQuantity(decimal amountPaid, AccessType accessType)
        {
            MessageResponse response = new MessageResponse();
            SlotPrice slotPrice = _context.SlotPrices.FirstOrDefault(x => x.AccessType == accessType && x.IsActive);
            if (slotPrice != null)
            {
                decimal perUnitPrice = slotPrice.Price;
                decimal output = amountPaid / perUnitPrice;
                output = Math.Truncate(output);
                response.Message = Convert.ToInt32(output).ToString();
                response.Success = true;
            }
            else
            {
                response.Message = "No slot price has been set";
            }
            return response;
        }

        public MessageResponse<long> CreateSlot(SlotModel model)
        {
            MessageResponse<long> response = new();
            Slot slot = new Slot
            {
                OptometristFirmId = model.OptometristFirmId,
                Quantity = model.Quantity,
                ReorderLevel = model.ReorderLevel
            };
            _context.Slots.Add(slot);
            try
            {
                _context.SaveChanges();
                response.Message = "Slot created successfully";
                response.Success = true;
                response.Result = slot.Id;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while trying to log the slot";
                response.Success = false;
            }
            return response;
        }

        public MessageResponse<long> CreateSlotPrice(SlotPriceModel model)
        {
            MessageResponse<long> response = new();
            model.CreatedBy = _authUser.UserId;
            SlotPrice slotPrice = new SlotPrice { IsActive = model.IsActive, Price = model.Price, CreatedBy = model.CreatedBy, CreatedDate = model.CreatedDate, AccessType = model.AccessType };
            if (_context.SlotPrices.Any(x => x.AccessType == model.AccessType && x.IsActive))
            {
                slotPrice.IsActive = false;
            }
            _context.SlotPrices.Add(slotPrice);
            try
            {
                _context.SaveChanges();
                response.Message = "Slot Price created successfully";
                response.Success = true;
                response.Result = slotPrice.Id;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while trying to create Slot Price";
            }
            return response;
        }

        public MessageResponse CreateSlotRequest(SlotRequestModel model)
        {
            MessageResponse response = new();
            string fileName = string.Empty;
            string documentPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "AppFile", "SlotPaymentEvidence");// HttpContext.Current.Server.MapPath("~/AppFile/SlotPaymentEvidence/");
            if (!Directory.Exists(documentPath)) Directory.CreateDirectory(documentPath);
            if (model.PostedFile != null)
            {
                fileName = Path.GetFileName(model.PostedFile.FileName);
                string ext = Path.GetExtension(fileName);
                string documentName = Guid.NewGuid().ToString() + ext;
                fileName = Path.Combine(documentPath, documentName);
                try
                {
                    FileStream fs = new FileStream(fileName, FileMode.Create);
                    model.PostedFile.CopyTo(fs);
                    model.PaymentProof = documentName;
                }
                catch (Exception ex)
                {
                    response.Message = "An error occurred while trying to upload the document. Please try again later";
                    response.Success = false;
                    return response;
                }
            }

            SlotPrice slotPrice = _context.SlotPrices.FirstOrDefault(x => x.AccessType == model.AccessType && x.IsActive);
            if (slotPrice == null)
            {
                response.Message = "Could not compute slot unit price";
                return response;
            }

            decimal perUnitPrice = slotPrice.Price;
            decimal output = model.AmountPaid.Value / perUnitPrice;
            int Quantity = (int)Math.Truncate(output);


            SlotRequest slotRequest = new SlotRequest
            {
                Comment = model.Comment,
                DateApproved = model.DateApproved,
                OptometristFirmId = model.OptometristFirmId,
                PaymentProof = model.PaymentProof,
                Quantity = Quantity,
                AccessType = model.AccessType.GetValueOrDefault(),
                Status = model.Status,
                CreatedBy = _authUser.UserId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                ReferenceNumber = ReferenceNoSlotRequest(),
                AmountPaid = model.AmountPaid.GetValueOrDefault(),
                PaymentMethod = PaymentMethod.Upload
            };
            _context.SlotRequests.Add(slotRequest);
            try
            {
                _context.SaveChanges();
                response.Message = $"Your request for {slotRequest.Quantity} slot(s) has been submitted successfully. Please wait for the Admin to review it.";
                response.Success = true;

                var currentUser = _context.ApplicationUsers.AsNoTracking().FirstOrDefault(x => x.Id == _authUser.UserId);
                if (currentUser != null)
                {

                    IList<ApplicationUser> slotManagetUsers = _userManager.GetUsersInRoleAsync(AppRoles.SLOTMANAGER).GetAwaiter().GetResult();
                    slotRequest = _context.SlotRequests.AsNoTracking().Include(x => x.OptometristFirm).FirstOrDefault(x => x.Id == slotRequest.Id);

                    foreach (var slotManagerUser in slotManagetUsers)
                    {
                        _emailService.LogEmail(new()
                        {
                            Email = slotManagerUser.Email,
                            Message = $"<h3>Dear {slotManagerUser.FullName},</h3><p>A Slot Request from {slotRequest.OptometristFirm.BusinessName} by {currentUser.FullName} has been initiated. Kindly attend.</p><p>Kindly visit <a href='{_appSettings.BaseUrl}'>DVLA</a></p>.<p>Best regards.</p>",
                            Subject = "New Slot Request Notification",
                            Url = _appSettings.BaseUrl
                        }).GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while trying to submit your request. Please try again later";
                _logger.LogError(ex.Message, ex);
                //Delete the uploaded file
                if (File.Exists(fileName))
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch (Exception ex1)
                    {
                    }
            }
            return response;
        }

        private string ReferenceNoSlotRequest()
        {
            return "DV" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 9).ToLower();
        }

        public IEnumerable<SlotRequestModel> FetchCustomerSlotRequests(string applicationUserId)
        {
            IEnumerable<SlotRequestModel> records = Enumerable.Empty<SlotRequestModel>();
            try
            {
                var entities = _context.SlotRequests.Include(x => x.OptometristFirm).AsNoTracking().Where(x => x.CreatedBy == applicationUserId).ToList();
                records = entities.Select(x => new SlotRequestModel
                {
                    AccessType = x.AccessType,
                    AmountPaid = x.AmountPaid,
                    Comment = x.Comment,
                    DateApproved = x.DateApproved,
                    OptometristFirmId = x.OptometristFirmId,
                    PaymentProof = x.PaymentProof,
                    Quantity = x.Quantity,
                    Status = x.Status,
                    DateCreated = x.CreatedDate,
                    BusinessName = x.OptometristFirm.BusinessName,
                    Id = x.Id,
                    PaymentMethod = x.PaymentMethod,
                    ReferenceNumber = x.ReferenceNumber,
                    TelephoneNumber = x.OptometristFirm.TelephoneNumber
                }).OrderByDescending(x => x.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return records;
        }


        //Todo
        public async Task<int> FetchLowQuantitySlots(string applicationUserId)
        {
            int availableQuantity = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchLowSlots", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", applicationUserId ?? System.Data.SqlTypes.SqlString.Null);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                availableQuantity = reader.GetInt32("Quantity");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return availableQuantity;
        }

        public SlotModel FetchSlotReOrderLevel(long id)
        {
            var slot = _context.Slots.FirstOrDefault(x => x.Id == id);
            if (slot != null)
            {
                SlotModel model = new SlotModel
                {
                    Id = slot.Id,
                    ReorderLevel = slot.ReorderLevel.GetValueOrDefault(),
                    Quantity = slot.Quantity,
                    OptometristFirmId = slot.OptometristFirmId,
                    AccessType = slot.AccessType
                };
                return model;
            }
            else
            {
                return null;
            }

        }

        public IEnumerable<SlotModel> FetchSlotReOrderLevelByOptometristfirm(int optometristfirmId)
        {
            var slot = _context.Slots.Where(x => x.OptometristFirmId == optometristfirmId);
            if (slot != null)
            {
                var model = slot.Select(x => new SlotModel
                {
                    Id = x.Id,
                    ReorderLevel = x.ReorderLevel.GetValueOrDefault(),
                    Quantity = x.Quantity,
                    OptometristFirmId = x.OptometristFirmId,
                    AccessType = x.AccessType
                }).ToList();
                return model;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<SlotModel> FetchSlotReOrderLevels()
        {
            return _context.Slots.Select(x => new SlotModel
            {
                Id = x.Id,
                ReorderLevel = x.ReorderLevel.GetValueOrDefault(),
                OptometristFirmId = x.OptometristFirmId,
                Quantity = x.Quantity,
                AccessType = x.AccessType
            }).AsEnumerable();
        }

        //Todo
        public async Task<SlotRequestModel> FetchSlotRequestById(int id, int status)
        {
            SlotRequestModel result = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchSlotRequestById", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@Id", id);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result = new SlotRequestModel
                                {
                                    Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString("Comment"),
                                    AccessType = (AccessType)reader.GetInt32("AccessType"),
                                    AmountPaid = reader.IsDBNull(reader.GetOrdinal("AmountPaid")) ? 0 : reader.GetDecimal("AmountPaid"),
                                    DateCreated = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? DateTime.MinValue : reader.GetDateTime("CreatedDate"),
                                    Id = reader.GetInt64("Id"),
                                    BusinessName = reader.IsDBNull(reader.GetOrdinal("BusinessName")) ? null : reader.GetString("BusinessName"),
                                    OptometristFirmId = reader.IsDBNull(reader.GetOrdinal("OptometristFirmId")) ? 0 : reader.GetInt32("OptometristFirmId"),
                                    DateApproved = reader.IsDBNull(reader.GetOrdinal("DateApproved")) ? DateTime.MinValue : reader.GetDateTime("DateApproved"),
                                    PaymentProof = reader.IsDBNull(reader.GetOrdinal("PaymentProof")) ? null : reader.GetString("PaymentProof"),
                                    Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0 : reader.GetInt32("Quantity"),
                                    ReferenceNumber = reader.IsDBNull(reader.GetOrdinal("ReferenceNumber")) ? null : reader.GetString("ReferenceNumber"),
                                    Status = (SlotRequestStatus)reader.GetInt32("Status"),
                                    TelephoneNumber = reader.IsDBNull(reader.GetOrdinal("TelephoneNumber")) ? null : reader.GetString("TelephoneNumber")
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
            return result;
        }

        public async Task<IEnumerable<SlotRequestModel>> FetchSlotRequests(SlotRequestParameter request)
        {

            List<SlotRequestModel> records = new();
            List<SlotRequest> entities = await _context.SlotRequests.AsNoTracking().Where(x => x.Status == (SlotRequestStatus)request.status).Include(x => x.OptometristFirm).Take(request.length).ToListAsync();
            if (request.StartDate.HasValue)
            {
                request.StartDate = Utility.StartOfDay(request.StartDate.Value);
                entities = entities.Where(x => x.CreatedDate > request.StartDate.Value).ToList();
            }
            if (request.EndDate.HasValue)
            {
                request.EndDate = Utility.EndOfDay(request.EndDate.Value);
                entities = entities.Where(x => x.CreatedDate < request.EndDate.Value).ToList();
            }

            records = entities.Select(x => new SlotRequestModel
            {
                AccessType = x.AccessType,
                AmountPaid = x.AmountPaid,
                BusinessName = x.OptometristFirm.BusinessName,
                DateApproved = x.DateApproved,
                Comment = x.Comment,
                DateCreated = x.CreatedDate,
                Id = x.Id,
                OptometristFirmId = x.OptometristFirmId,
                PaymentMethod = x.PaymentMethod,
                PaymentProof = x.PaymentProof,
                Quantity = x.Quantity,
                ReferenceNumber = x.ReferenceNumber,
                Status = x.Status,
                TelephoneNumber = x.OptometristFirm.TelephoneNumber
            }).OrderByDescending(x => x.Id).ToList();


            return records;
        }

        public async Task<List<SlotRequestModel>> FetchSlotsForIncomeReport(DateTime? StartDate, DateTime? EndDate)
        {
            List<SlotRequestModel> records = new();
            try
            {
                StartDate = StartDate.HasValue ? StartDate.Value : DateTime.Now;
                EndDate = EndDate.HasValue ? EndDate.Value.AddHours(23) : DateTime.Now.AddHours(23);
                SqlParameter[] parameters =
                {
                new SqlParameter("@StartDate",StartDate),
                new SqlParameter("@EndDate",EndDate)
            };
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FetchSlotRequestsForIncomeReport", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", EndDate);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                records.Add(new SlotRequestModel
                                {
                                    Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString("Comment"),
                                    AccessType = (AccessType)(reader.IsDBNull(reader.GetOrdinal("AccessType")) ? 0 : reader.GetInt32("AccessType")),
                                    AmountPaid = reader.IsDBNull(reader.GetOrdinal("AmountPaid")) ? 0 : reader.GetDecimal("AmountPaid"),
                                    DateCreated = reader.GetDateTime("CreatedDate"),
                                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                                    BusinessName = reader.IsDBNull(reader.GetOrdinal("BusinessName")) ? null : reader.GetString("BusinessName"),
                                    OptometristFirmId = reader.IsDBNull(reader.GetOrdinal("OptometristFirmId")) ? 0 : reader.GetInt32("OptometristFirmId"),
                                    DateApproved = reader.IsDBNull(reader.GetOrdinal("DateApproved")) ? (DateTime?)null : reader.GetDateTime("DateApproved"),
                                    PaymentProof = reader.IsDBNull(reader.GetOrdinal("PaymentProof")) ? null : reader.GetString("PaymentProof"),
                                    Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0 : reader.GetInt32("Quantity"),
                                    ReferenceNumber = reader.IsDBNull(reader.GetOrdinal("ReferenceNumber")) ? null : reader.GetString("ReferenceNumber"),
                                    Status = (SlotRequestStatus)(reader.IsDBNull(reader.GetOrdinal("Status")) ? 0 : reader.GetInt32("Status")),
                                    TelephoneNumber = reader.IsDBNull(reader.GetOrdinal("TelephoneNumber")) ? null : reader.GetString("TelephoneNumber")
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
            return records;
        }

        public OptometristFirmModel GetOptometristFirmByApplicationUserID(string applicationUserID)
        {
            OptometristFirmModel model = null;
            OptometristFirmUser optometristFirmUser = _context.OptometristFirmUsers.AsNoTracking().FirstOrDefault(x => x.ApplicationUserId == applicationUserID);
            if (optometristFirmUser == null) return model;
            long optopetristFirmUserID = optometristFirmUser.OptometristFirmId;
            OptometristFirm optometristFirm = _context.OptometristFirms.AsNoTracking().FirstOrDefault(x => x.Id == optopetristFirmUserID);
            if (optometristFirm == null) return model;
            model = new OptometristFirmModel
            {
                AccreditationNumber = optometristFirm.AccreditationNumber,
                BusinessAddress = optometristFirm.BusinessAddress,
                BusinessName = optometristFirm.BusinessName,
                ContactEmailAddress = optometristFirm.BusinessName,
                ContactFirstName = optometristFirm.ContactFirstName,
                ContactLastName = optometristFirm.ContactLastName,
                ContactPhoneNumber = optometristFirm.ContactPhoneNumber,
                CreatedBy = optometristFirm.CreatedBy,
                DigitalAddress = optometristFirm.DigitalAddress,
                Id = optometristFirm.Id,
                IsActive = optometristFirm.IsActive,
                IsDeleted = optometristFirm.IsDeleted,
                MobileNumber = optometristFirm.MobileNumber,
                RegionId = optometristFirm.RegionId,
                DistrictId = optometristFirm.DistrictId,
                RegistrationNumber = optometristFirm.RegistrationNumber,
                TelephoneNumber = optometristFirm.TelephoneNumber,
                Town = optometristFirm.Town,
                UpdatedBy = optometristFirm.ModifiedBy,
                UserId = applicationUserID
            };
            return model;
        }

        public IEnumerable<OptometristFirmModel> GetOptometristFirms()
        {
            return _context.OptometristFirms.AsNoTracking().Select(x => new OptometristFirmModel
            {
                AccreditationNumber = x.AccreditationNumber,
                BusinessAddress = x.BusinessAddress,
                BusinessName = x.BusinessName,
                ContactEmailAddress = x.ContactEmail,
                ContactFirstName = x.ContactFirstName,
                ContactLastName = x.ContactLastName,
                ContactPhoneNumber = x.ContactPhoneNumber,
                DigitalAddress = x.DigitalAddress,
                Id = x.Id,
                MobileNumber = x.MobileNumber,
                RegionId = x.RegionId,
                DistrictId = x.DistrictId,
                RegistrationNumber = x.RegistrationNumber,
                TelephoneNumber = x.TelephoneNumber,
                Town = x.Town
            }).OrderBy(x => x.BusinessName).AsEnumerable();
        }

        public SlotPriceModel GetSlotPrice(int id)
        {
            SlotPriceModel model = null;
            SlotPrice slotPrice = _context.SlotPrices.Find(id);
            if (slotPrice != null)
            {
                model = new SlotPriceModel { Id = slotPrice.Id, IsActive = slotPrice.IsActive, Price = slotPrice.Price, AccessType = slotPrice.AccessType };
            }
            return model;
        }

        public SlotPriceModel GetSlotPrice()
        {
            SlotPriceModel model = null;
            SlotPrice slotPrice = _context.SlotPrices.FirstOrDefault(x => x.IsActive);
            if (slotPrice != null)
            {
                model = new SlotPriceModel { Id = slotPrice.Id, IsActive = slotPrice.IsActive, Price = slotPrice.Price };
            }
            return model;
        }

        public IEnumerable<SlotPriceModel> GetSlotPrices()
        {
            List<SlotPriceModel> slotPrices = _context.SlotPrices.Select(x => new SlotPriceModel { Id = x.Id, IsActive = x.IsActive, Price = x.Price, CreatedDate = x.CreatedDate, ModifiedDate = x.ModifiedDate, UpdatedBy = x.ModifiedBy, CreatedBy = x.CreatedBy, AccessType = x.AccessType }).OrderByDescending(x => x.Id).ToList();
            foreach (var item in slotPrices)
            {
                var obj = _context.ApplicationUsers.FirstOrDefault(x => x.Id == item.CreatedBy);
                if (obj != null)
                {
                    item.CreatedByFullName = !string.IsNullOrEmpty(item.CreatedBy) ? (obj.LastName + " " + obj.FirstName) : "";
                }
                else
                {
                    item.CreatedByFullName = "";
                }
                obj = _context.ApplicationUsers.FirstOrDefault(x => x.Id == item.UpdatedBy);
                if (obj != null)
                {
                    item.UpdatedByFullName = !string.IsNullOrEmpty(item.UpdatedBy) ? (obj.LastName + " " + obj.FirstName) : "";
                }
                else
                {
                    item.UpdatedByFullName = "";
                }
            }
            return slotPrices;
        }

        public MessageResponse Preview(int id)
        {
            MessageResponse response = new();
            SlotRequest slotRequest = _context.SlotRequests.FirstOrDefault(x => x.Id == id);
            if (slotRequest == null)
            {
                response.Message = "The record does not exist";
                return response;
            }
            slotRequest.Status = SlotRequestStatus.Opened;
            try
            {
                _context.SaveChanges();

                response.Message = "Request has been viewed";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while trying to reject the request";
            }
            return response;
        }

        public MessageResponse RejectSlotRequest(RejectSlotRequestModel model)
        {
            MessageResponse response = new();
            SlotRequest slotRequest = _context.SlotRequests.FirstOrDefault(x => x.Id == model.Id);
            if (slotRequest == null)
            {
                response.Message = "The record does not exist";
                return response;
            }
            slotRequest.Status = SlotRequestStatus.Reject;
            slotRequest.Comment = model.Reason.ToUpper();
            try
            {
                _context.SaveChanges();

                var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == slotRequest.CreatedBy);
                // _notificationRepository.SlotDeclineNotification(user, slotRequest.Quantity, callbackUrl);


                response.Message = "Request rejected successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while trying to reject the request";
            }
            return response;
        }

        public MessageResponse SlotDeduction(SlotDeductionModel model)
        {
            MessageResponse response = new();

            var userData = _authUser.UserId;

            Slot slot = _context.Slots.FirstOrDefault(x => x.OptometristFirmId == model.OptometristFirmId && x.AccessType == (AccessType)model.AccessType);
            if (slot == null)
            {
                response.Message = "Slot does not exist";
                return response;
            }

            if (model.Quantity > slot.Quantity)
            {
                response.Message = "You cannot remove more than the available number of slots for this Optometrist Firm";
                return response;
            }
            slot.Quantity -= model.Quantity;

            SlotReductionLog log = new SlotReductionLog
            {
                Comment = model.Comment.Trim(),
                CreatedBy = _authUser.UserId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                OptometristFirmId = model.OptometristFirmId,
                Quantity = model.Quantity,
                AccessType = (AccessType)model.AccessType
            };
            _context.SlotReductionLogs.Add(log);
            try
            {
                _context.SaveChanges();
                response.Message = "Slot Deduction successful";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while trying to Top Up";
                _logger.LogError(ex.Message, ex);
            }

            return response;
        }

        public MessageResponse UpdateSlotPrice(SlotPriceModel model, int id)
        {
            MessageResponse response = new();
            SlotPrice slotPrice = _context.SlotPrices.Find(id);
            slotPrice.IsActive = model.IsActive;
            slotPrice.Price = model.Price;
            slotPrice.AccessType = model.AccessType;
            slotPrice.ModifiedBy = _authUser.UserId;
            slotPrice.ModifiedDate = model.ModifiedDate;
            if (model.IsActive)
            {
                //Deactivate all other records
                List<SlotPrice> otherSlotPrices = _context.SlotPrices.Where(x => x.AccessType == model.AccessType && x.Id != id).ToList();
                foreach (var other in otherSlotPrices)
                {
                    other.IsActive = false;
                }

            }

            try
            {
                _context.SaveChanges();
                response.Message = "Slot Price updated successfully";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while trying to update entry";
            }
            return response;
        }

        public MessageResponse UpdateSlotReOrderLevel(SlotModel model)
        {
            MessageResponse response = new MessageResponse();
            try
            {
                Slot slot = _context.Slots.FirstOrDefault(x => x.Id == model.Id);
                slot.ReorderLevel = model.ReorderLevel;

                _context.SaveChanges();
                response.Success = true;
                response.Message = "Slot Reorder level updated successfully";
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while trying update the reorder level";
            }
            return response;
        }
    }
}
