using Azure;
using DVLA.Business.EmailModule;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.Auth;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DVLA.Business.OptometristFirmModule
{
    public class OptometristService : IOptometristService
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<OptometristService> _logger;
        private readonly IAuthUser _authUser;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public OptometristService(DVLADbContext context, ILogger<OptometristService> logger, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IUserService userService, IConfiguration configuration, IEmailService emailService, IAuthUser authUser)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _configuration = configuration;
            _emailService = emailService;
            _authUser = authUser;
        }

        public async Task<MessageResponse> CreateOptometricFirm(OptometristFirmViewModel model)
        {
            MessageResponse result = new();

            var context = _context;
            var transaction = await context.Database.BeginTransactionAsync();
            using (transaction)
            {
                try
                {
                    var optometristFirmQuery = context.OptometristFirms.AsNoTracking();

                    var businessName = optometristFirmQuery.FirstOrDefault(x => x.BusinessName == model.BusinessName);
                    if (businessName != null)
                    {
                        await transaction.RollbackAsync();
                        result.Message = "Business name already exist";
                        return result;
                    }

                    var registrationNumber = optometristFirmQuery.FirstOrDefault(x => x.RegistrationNumber == model.RegistrationNumber);
                    if (registrationNumber != null)
                    {
                        await transaction.RollbackAsync();
                        result.Message = "Registration number already exist";
                        return result;
                    }

                    var accreditationNumber = optometristFirmQuery.FirstOrDefault(x => x.AccreditationNumber == model.AccreditationNumber);
                    if (accreditationNumber != null)
                    {
                        await transaction.RollbackAsync();
                        result.Message = "Accreditation number already exist";
                        return result;
                    }

                    var user = await _userManager.FindByEmailAsync(model.ContactEmail);
                    if (user != null)
                    {
                        await transaction.RollbackAsync();
                        result.Message = "User already exist";
                        return result;
                    }

                    var applicationUser = new ApplicationUser()
                    {
                        CreatedDate = DateTime.Now,
                        Email = model.ContactEmail,
                        FirstName = model.ContactFirstName,
                        LastName = model.ContactLastName,
                        IsActive = true,
                        MobileNumber = model.ContactPhoneNumber,
                        Address = model.BusinessAddress,
                        UserName = model.ContactEmail,
                        CreatedBy = _authUser.UserId,
                        DefaultRole = AppConstants.Roles[1],
                        Id = Guid.NewGuid().ToString()
                    };

                    string password = Guid.NewGuid().ToString().Substring(0, 7).Replace("-", "");

                    IdentityResult identityResult = await _userManager.CreateAsync(applicationUser, password);

                    if (identityResult.Succeeded)
                    {

                        var optomestrist = new OptometristFirm
                        {
                            RegionId = model.RegionId,
                            DistrictId = model.DistrictId,
                            AccreditationNumber = model.AccreditationNumber,
                            BusinessAddress = model.BusinessAddress,
                            BusinessName = model.BusinessName,
                            ContactEmail = model.ContactEmail,
                            ContactFirstName = model.ContactFirstName,
                            ContactLastName = model.ContactLastName,
                            ContactPhoneNumber = model.ContactPhoneNumber,
                            CreatedBy = _authUser.UserId,
                            DigitalAddress = model.DigitalAddress,
                            IsActive = true,
                            IsDeleted = false,
                            MobileNumber = model.MobileNumber,
                            RegistrationNumber = model.RegistrationNumber,
                            Town = model.Town,
                            TelephoneNumber = model.TelephoneNumber,
                        };
                        context.OptometristFirms.Add(optomestrist);
                        context.SaveChanges();

                        //Generate Reference Number
                        var region = context.Regions.AsNoTracking().FirstOrDefault(x => x.Id == model.RegionId);
                        if (region == null)
                        {
                            await transaction.RollbackAsync();
                            result.Message = "The region selected does not exist";
                            return result;
                        }
                        string regionPrefix = region.Name.Substring(0, 3);
                        string CentreCode = "VA" + DateTime.Now.ToString("yyyymmdd").Substring(2, 2) + optomestrist.Id.ToString().PadLeft(6, '0') + regionPrefix;

                        optomestrist.CentreCode = CentreCode;
                        await context.SaveChangesAsync();

                        OptometristFirmUser optometristUser = new()
                        {
                            OptometristFirmId = optomestrist.Id,
                            ApplicationUserId = applicationUser.Id
                        };

                        context.OptometristFirmUsers.Add(optometristUser);
                        context.SaveChanges();

                        identityResult = await _userManager.AddToRoleAsync(applicationUser, AppConstants.Roles[1]);

                        string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

                        var encodedToken = WebUtility.UrlEncode(confirmationToken);
                        string baseUrl = _configuration["AppConstants:BaseUrl"];
                        string url = $"{baseUrl}/Account/ConfirmEmail?encodedToken={encodedToken}&userid={applicationUser.Id}";

                        string message = $"An account has been created on <a href='{baseUrl}/Account/Login'>HEFRA</a> with the default password <b>{password}</b>. Kindly login with your email {model.ContactEmail} and password {password};  update the password to confirm your account.";

                        bool EmailLogSuccess = await _emailService.LogEmail(new EmailLogDto
                        {
                            Email = model.ContactEmail,
                            Message = message,
                            Subject = "Account Confirmation",
                            Url = url
                        });

                        if (!EmailLogSuccess)
                        {
                            await transaction.RollbackAsync();
                            result.Message = "Could not complete account creation at this time. Please try again later.";
                            return result;
                        }

                        result.Message = $"Account created successfully";
                        await transaction.CommitAsync();
                        result.Success = true;
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        result.Message = identityResult.Errors.FirstOrDefault()?.Description;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex.Message, ex);
                    throw;
                }
            }
            return result;
        }

        public async Task<PaginationResponseModel<List<OptometristFirmViewModel>>> GetAllOptometricFirms(PaginationRequestModel model)
        {
            PaginationResponseModel<List<OptometristFirmViewModel>> result = new();
            try
            {
                var query = _context.OptometristFirms.AsNoTracking().Include(x=>x.District).Include(x=>x.Region);
                result.ListResult = await query.Skip((model.PageIndex - 1) * model.PageSize)
             .Take(model.PageSize)
             .Select(x => new OptometristFirmViewModel
             {
                 ContactEmail = x.ContactEmail,
                 AccreditationNumber = x.AccreditationNumber,
                 BusinessAddress = x.BusinessAddress,
                 Id = x.Id,
                 BusinessName = x.BusinessName,
                 CentreCode = x.CentreCode,
                 CreatedDate = x.CreatedDate,
                 ContactFirstName = x.ContactFirstName,
                 IsActive = x.IsActive,
                 ContactLastName = x.ContactLastName,
                 ContactPhoneNumber = x.ContactPhoneNumber,
                 DigitalAddress = x.DigitalAddress,
                 DistrictId = x.DistrictId,
                 DistrictName = x.District.Name,
                 IsSynchronized = x.IsSynchronized,
                 MobileNumber = x.MobileNumber,
                 RegionId = x.RegionId,
                 RegionName = x.Region.Name,
                 RegistrationNumber = x.RegistrationNumber,
                 ReorderLevel = x.ReorderLevel,
                 TelephoneNumber = x.TelephoneNumber,
                 Town = x.Town
             }).OrderBy(x => x.Id).ToListAsync();
                result.TotalCount = await query.CountAsync();
                result.PageIndex = model.PageIndex;
                result.PageSize = model.PageSize;
                var pModel = new PaginationResponseModel<PaginationResponseModel<List<UserViewModel>>>(result.TotalCount, result.PageSize,result.ListResult.Count);
                result.TotalPages = pModel.TotalPages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

            }
            return result;
        }

        public async Task<OptometristFirmViewModel> GetOptometricFirm(int id)
        {
            OptometristFirmViewModel result = null;
            try
            {
                result = await _context.OptometristFirms.AsNoTracking().Include(x => x.District).Include(x => x.Region)
                    .Select(x => new OptometristFirmViewModel
                    {
                        ContactEmail = x.ContactEmail,
                        AccreditationNumber = x.AccreditationNumber,
                        BusinessAddress = x.BusinessAddress,
                        Id = x.Id,
                        BusinessName = x.BusinessName,
                        CentreCode = x.CentreCode,
                        CreatedDate = x.CreatedDate,
                        ContactFirstName = x.ContactFirstName,
                        IsActive = x.IsActive,
                        ContactLastName = x.ContactLastName,
                        ContactPhoneNumber = x.ContactPhoneNumber,
                        DigitalAddress = x.DigitalAddress,
                        DistrictId = x.DistrictId,
                        DistrictName = x.District.Name,
                        IsSynchronized = x.IsSynchronized,
                        MobileNumber = x.MobileNumber,
                        RegionId = x.RegionId,
                        RegionName = x.Region.Name,
                        RegistrationNumber = x.RegistrationNumber,
                        ReorderLevel = x.ReorderLevel,
                        TelephoneNumber = x.TelephoneNumber,
                        Town = x.Town
                    }).FirstOrDefaultAsync(x => x.Id == id);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

            }
            return result;
        }

        public async Task<MessageResponse> UpdateOptometricFirm(OptometristFirmViewModel model)
        {
            MessageResponse result = new();
            var context = _context;
            var transaction = await context.Database.BeginTransactionAsync();
            using (transaction)
            {
                try
                {

                    var optomestristFirm = context.OptometristFirms.FirstOrDefault(x => x.Id == model.Id);
                    string oldContactEmail = optomestristFirm.ContactEmail;
                    if (optomestristFirm == null)
                    {
                        await transaction.RollbackAsync();
                        result.Message = "No optometrist record found";
                        return result;
                    }

                    optomestristFirm.RegionId = model.RegionId;
                    optomestristFirm.DistrictId = model.DistrictId;
                    optomestristFirm.AccreditationNumber = model.AccreditationNumber;
                    optomestristFirm.BusinessAddress = model.BusinessAddress;
                    optomestristFirm.BusinessName = model.BusinessName;
                    optomestristFirm.ContactEmail = model.ContactEmail;
                    optomestristFirm.ContactFirstName = model.ContactFirstName;
                    optomestristFirm.ContactLastName = model.ContactLastName;
                    optomestristFirm.ContactPhoneNumber = model.ContactPhoneNumber;
                    optomestristFirm.CreatedBy = _authUser.UserId;
                    optomestristFirm.DigitalAddress = model.DigitalAddress;
                    optomestristFirm.IsActive = true;
                    optomestristFirm.IsDeleted = false;
                    optomestristFirm.MobileNumber = model.MobileNumber;
                    optomestristFirm.RegistrationNumber = model.RegistrationNumber;
                    optomestristFirm.Town = model.Town;
                    optomestristFirm.TelephoneNumber = model.TelephoneNumber;

                    await context.SaveChangesAsync();

                    if (!oldContactEmail.Equals(model.ContactEmail))
                    {
                        var applicationUser = new ApplicationUser()
                        {
                            CreatedDate = DateTime.Now,
                            Email = model.ContactEmail,
                            FirstName = model.ContactFirstName,
                            LastName = model.ContactLastName,
                            IsActive = true,
                            MobileNumber = model.ContactPhoneNumber,
                            Address = model.BusinessAddress,
                            UserName = model.ContactEmail,
                            CreatedBy = _authUser.UserId,
                            Id = Guid.NewGuid().ToString()
                        };

                        string password = Guid.NewGuid().ToString().Substring(0, 7).Replace("-", "");

                        var admin = await _userManager.CreateAsync(applicationUser, password);
                        if (admin.Succeeded)
                        {
                            context.OptometristFirmUsers.Add(new OptometristFirmUser()
                            {
                                OptometristFirmId = optomestristFirm.Id,
                                ApplicationUserId = applicationUser.Id
                            });
                            await context.SaveChangesAsync();

                            var userId = applicationUser.Id;
                            await _userManager.AddToRoleAsync(applicationUser, AppConstants.Roles[1]);

                            string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

                            var encodedToken = WebUtility.UrlEncode(confirmationToken);
                            string baseUrl = _configuration["AppConstants:BaseUrl"];
                            string url = $"{baseUrl}/Account/ConfirmEmail?encodedToken={encodedToken}&userid={applicationUser.Id}";

                            string message = $"An account has been created on <a href='{baseUrl}/Account/Login'>HEFRA</a> with the default password <b>{password}</b>. Kindly login with your email {model.ContactEmail} and password {password};  update the password to confirm your account.";

                            bool EmailLogSuccess = await _emailService.LogEmail(new EmailLogDto
                            {
                                Email = model.ContactEmail,
                                Message = message,
                                Subject = "Account Confirmation",
                                Url = url
                            });

                            if (!EmailLogSuccess)
                            {
                                await transaction.RollbackAsync();
                                result.Message = "Could not complete account creation at this time. Please try again later.";
                                return result;
                            }
                        }

                    }

                    result.Message = "Record saved successfully";
                    result.Success = true;
                    await transaction.CommitAsync();
                    //AddAudit(Activities.UPDATE_OPTOMETRIST_FIRM, "Update otpmetrist Firm");
                    return result;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex.Message, ex);
                    result.Message = "Kindly try again later";
                }
            }
            
            return result;
        }

        public async Task<MessageResponse> ChangeStatus(int optometristFirmId)
        {
            MessageResponse result = new();
            try
            {
                var optometrist = _context.OptometristFirms.FirstOrDefault(x => x.Id == optometristFirmId);
                if (optometrist.IsActive)
                {
                    result.Message = "Optometrist successfully deactivated";
                    optometrist.IsActive = false;
                }
                else
                {
                    result.Message = "Optometrist successfully activated";
                    optometrist.IsActive = true;
                }


                string respMessage = "";
                var users = _context.OptometristFirmUsers.Include(x=>x.ApplicationUser).Where(x => x.OptometristFirmId == optometristFirmId).Select(x =>x.ApplicationUser).ToList();  //_userRepository.GetOptometristFirmUsers(optometrist.Id);
                foreach (var user in users)
                {
                    if (user.IsDeleted) continue;
                    user.IsActive = optometrist.IsActive;
                    user.ModifiedBy = _authUser.UserId;
                    user.DateUpdated = DateTime.UtcNow;
                }

                optometrist.ModifiedBy = _authUser.UserId;
                optometrist.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                result.Message = "Kindly try again later";
            }
            return result;
        }
    }
}
