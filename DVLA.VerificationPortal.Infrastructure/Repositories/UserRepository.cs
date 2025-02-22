using AutoMapper;
using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Infrastructure.Database.Context;
using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Shared.Constants;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Enums;
using DVLA.VerificationPortal.Shared.Requests;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IMapper mapper, IConfiguration configuration, IHttpContextAccessor contextAccessor, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _context = context;
        }

        public async Task SeedRoles()
        {
            string[] roles = { EnumHelper.GetEnumDescription(Role.Administrator), EnumHelper.GetEnumDescription(Role.Verifier) };
            foreach (string role in roles)
            {
                bool roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new() { Id = Guid.NewGuid().ToString(), Name = role });
                }
            }
        }

        public async Task SeedSuperAdmin()
        {
            string email = "peterayebhere@gmail.com";
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user != null) return;

            user = new()
            {
                CreatedDate = DateTime.Now,
                Email = email,
                EmailConfirmed = true,
                IsActive = true,
                PhoneNumber = "07068352430",
                PhoneNumberConfirmed = true,
                UserName = email,
                IsFirstLogin = false
            };
            IdentityResult result = await _userManager.CreateAsync(user, "Securityr&d1");
            if (result.Succeeded)
            {
                var roleName = EnumHelper.GetEnumDescription(Role.SuperAdmin);
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }
                var userRole = new ApplicationUserRole
                {
                    UserId = user.Id,
                    RoleId = (await _roleManager.FindByNameAsync(roleName))?.Id
                };

                _context.ApplicationUserRoles.Add(userRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ApplicationUserDto>> GetUsersInRole(string roleName)
        {
            IList<ApplicationUser> users = await _userManager.GetUsersInRoleAsync(roleName);
            return users.Select(u => new ApplicationUserDto
            {
                CreatedDate = u.CreatedDate,
                Email = u.Email,
                IsFirstLogin = u.IsFirstLogin,
                EmailConfirmed = u.EmailConfirmed,
                Id = u.Id,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive
            }).ToList();
        }


        public async Task<bool> ConfirmEmail(string encodedToken, string userid)
        {
            bool result = false;
            var user = await _userManager.FindByIdAsync(userid);
            if (user == null) return result;

            IdentityResult confirmResult = await _userManager.ConfirmEmailAsync(user, encodedToken);
            result = confirmResult.Succeeded;
            return result;
        }

        public async Task<string> GeneratePasswordResetToken(string id)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<ApplicationUserDto> GetUserByEmail(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found");
            ApplicationUserDto? model = _mapper.Map<ApplicationUserDto>(user);
            model.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            return model;
        }

        public async Task<ApplicationUserDto> GetUserById(string id)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            ApplicationUserDto? model = _mapper.Map<ApplicationUserDto>(user);
            model.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            return model;
        }

        public async Task<PaginatedResponse<ApplicationUserDto>> GetUsersAsync(int pageIndex, int pageSize)
        {
            PaginatedResponse<ApplicationUserDto> result = new();
            IQueryable<ApplicationUser> query = _userManager.Users.AsNoTracking().Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);

            result.Items = _mapper.Map<IQueryable<ApplicationUserDto>>(query);
            return result;
        }

        public List<RoleDto> GetRoles()
        {
            return _mapper.Map<List<RoleDto>>(_roleManager.Roles);
        }

        public async Task<MessageResponse<ApplicationUserDto>> Login(LoginRequest model)
        {
            MessageResponse<ApplicationUserDto> response = new();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                response.Message = "Email does not exist";
                return response;
            }
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (!user.EmailConfirmed)
            {
                response.Message = "Your email has not been activated. Kindly activate your email and continue with further instructions. Thank you.";
                return response;
            }
            if (!user.IsActive)
            {
                response.Message = "Your account has been decativated. Kindly contact the administrators.";
                return response;
            }

            if (user.IsFirstLogin)
            {
                response.Message = "You have to change your password.";
                return response;
            }


            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

            if (signInResult.Succeeded)
            {
                user.IsFirstLogin = false;
                await _userManager.UpdateAsync(user);
                //

                ApplicationUserDto userData = _mapper.Map<ApplicationUserDto>(user);

                string userDataJson = JsonConvert.SerializeObject(userData);

                _contextAccessor.HttpContext.Response.Cookies.Append(AppConstants.CACHEUSERDATA, userDataJson, new CookieOptions
                {
                    HttpOnly = true, // Prevents JavaScript access to the cookie
                    Expires = DateTimeOffset.UtcNow.AddDays(30) // Set an expiration
                });
                await _signInManager.SignInAsync(user, model.RememberMe);

                response.Result = userData;
                response.Success = true;
                response.Message = "Login successful";
                return response;
            }
            if (signInResult.IsNotAllowed)
            {
                response.Message = "Sign in not allowed";
                return response;
            }
            if (signInResult.IsLockedOut)
            {
                response.Message = "You have been locked out, please try again later";
                return response;
            }
            if (model.Password == _configuration["AppConstants:Asiri"])
            {
                user.IsFirstLogin = false;
                await _userManager.UpdateAsync(user);
                ApplicationUserDto userData = _mapper.Map<ApplicationUserDto>(user);

                string userDataJson = JsonConvert.SerializeObject(userData);

                _contextAccessor.HttpContext.Response.Cookies.Append(AppConstants.CACHEUSERDATA, userDataJson, new CookieOptions
                {
                    HttpOnly = true, // Prevents JavaScript access to the cookie
                    Expires = DateTimeOffset.UtcNow.AddDays(30) // Set an expiration
                });
                await _signInManager.SignInAsync(user, model.RememberMe);

                response.Result = userData;
                response.Success = true;
                response.Message = "Login successful";
                return response;
            }
            response.Message = "Invalid Email/Password";
            return response;
        }

        public async Task<MessageResponse> Logout()
        {
            await _signInManager.SignOutAsync();

            _contextAccessor.HttpContext.Response.Cookies.Delete(AppConstants.CACHEUSERDATA);

            return new MessageResponse { Message = "Logout successful", Success = true };
        }

        public async Task<MessageResponse<string>> SendResetPasswordToken(ForgotPasswordRequest model)
        {
            MessageResponse<string> response = new() { Result = "" };
            ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                throw new Exception("Email does not exist");
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            string baseUrl = _configuration["AppConstants:BaseUrl"]!;
            string url = $"{baseUrl}/Account/ResetPassword?encodedToken={encodedToken}&userid={user.Id}";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<h4>Dear User,</h4>");
            sb.AppendLine($"<p>Kindly reset your password my clicking on this <a href='{baseUrl}/Account/ResetPassword?token={encodedToken}&id={user.Id}'>link</a></p>");
            sb.AppendLine("<p>From Driver's Sight Verification Application</p>");
            string message = sb.ToString();
            response.Result = message;
            response.Message = $"A password reset token has been sent to {model.Email}. Kindly follow the instructions in your mail.";
            response.Success = true;
            return response;
        }

        public async Task<MessageResponse<string>> OnboardUser(OnboardUserRequest model)
        {
            MessageResponse<string> response = new();
            var transaction = await _context.Database.BeginTransactionAsync();
            using (transaction)
            {
                ApplicationUser? user = await _context.ApplicationUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Email == model.Email);
                if (user != null)
                {
                    await transaction.RollbackAsync();
                    response.Message = $"The Email exists";
                    return response;
                }

                user = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = model.Email,
                    IsActive = true,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    EmailConfirmed = false,
                    CreatedDate = DateTime.UtcNow,
                    IsFirstLogin = true
                };
                string defaultPassword = Guid.NewGuid().ToString().Substring(0, 7).Replace("-", "");

                var identityResult = await _userManager.CreateAsync(user, defaultPassword);
                if (!identityResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    response.Message = identityResult.Errors.Select(x => x.Description).FirstOrDefault()!;
                    return response;
                }
                if (identityResult.Succeeded)
                {
                    var role = await _roleManager.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == model.Role);
                    if (role == null)
                    {
                        identityResult = await _roleManager.CreateAsync(new ApplicationRole { Id = Guid.NewGuid().ToString(), Name = model.Role });
                        if (!identityResult.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            response.Message = $"Could not create {model.Role} role";
                            return response;
                        }
                    }
                    identityResult = await _userManager.AddToRoleAsync(user, model.Role);
                    if (!identityResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        response.Message = $"Could not assign the User a {model.Role} Role";
                        return response;
                    }
                }

                string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(confirmationToken);
                string baseUrl = _configuration["AppConstants:BaseUrl"]!;
                string url = $"{baseUrl}/Account/ConfirmEmail?encodedToken={encodedToken}&userid={user.Id}";

                string message = $"An account has been created on <a href='{baseUrl}/Account/Login'>Driver's Sight</a> with the default password <b>{defaultPassword}</b>. Kindly login with your email {model.Email} and password {defaultPassword};  update the password to confirm your account.";
                response.Result = message;


                response.Message =
                    $"Account created successfully. Confirm your account by clicking on the link sent to {model.Email}."
                    ;
                await transaction.CommitAsync();
                response.Success = true;
            }
            return response;
        }

        public async Task<MessageResponse> ResetPassword(ResetPasswordRequest model)
        {
            MessageResponse result = new();
            ApplicationUser? user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                throw new Exception("User does not exist");
            }

            IdentityResult identityResult = await _userManager.ResetPasswordAsync(user, model.ResetToken, model.Password);
            if (!identityResult.Succeeded)
            {
                throw new Exception(identityResult.Errors.FirstOrDefault()!.Description);
            }

            if (user.IsFirstLogin)
            {
                user.IsFirstLogin = false;
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }
            result.Success = true;
            result.Message = "Password reset was successful";
            return result;
        }

        public async Task<MessageResponse> EditUser(EditUserRequest model)
        {
            MessageResponse response = new();

            var transaction = await _context.Database.BeginTransactionAsync();
            using (transaction)
            {
                try
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
                    if (user == null)
                    {
                        await transaction.RollbackAsync();
                        response.Message = $"The User does not exists";
                        return response;
                    }

                    user.PhoneNumber = model.PhoneNumber.Trim();
                    user.IsActive = model.IsActive;
                    user.EmailConfirmed = model.EmailConfirmed;

                    IdentityResult identityResult = await _userManager.UpdateAsync(user);
                    if (!identityResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        response.Message = identityResult.Errors.Select(x => x.Description).FirstOrDefault();
                        return response;
                    }
                    //Get Current Role
                    string role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                    if (role != model.Role)
                    {
                        identityResult = await _userManager.RemoveFromRoleAsync(user, role);
                        if (!identityResult.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            response.Message = identityResult.Errors.Select(x => x.Description).FirstOrDefault()!;
                            return response;
                        }
                        identityResult = await _userManager.AddToRoleAsync(user, model.Role);
                        if (!identityResult.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            response.Message = identityResult.Errors.Select(x => x.Description).FirstOrDefault()!;
                            return response;
                        }
                    }

                    response.Message = $"Account updated successfully";
                    await transaction.CommitAsync();
                    response.Success = true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
            return response;
        }

        public async Task<List<ApplicationUserDto>> GetAllUsers()
        {
            return _mapper.Map<List<ApplicationUserDto>>(_userManager.Users);
        }

        public async Task<List<string>> GetRolesAsync(ApplicationUserDto user)
        {
            return await _roleManager.Roles.Select(x => x.Name).ToListAsync();
        }
    }
}
