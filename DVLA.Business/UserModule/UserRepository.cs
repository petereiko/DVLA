using DVLA.DATA.Domains;
using DVLA.Data.Models.Auth;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DVLA.Data.Models.DataObjects.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using NPOI.SS.Formula.Functions;
using DVLA.Data.Models.DataObjects.UtilityObjects;

namespace DVLA.Business.UserModule
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<UserRepository> _logger;
        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUserService _userService;
        private readonly IAuthUser _authUser;
        public UserRepository(DVLADbContext context, ILogger<UserRepository> logger, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IUserService userService, IAuthUser authUser)
        {
            _context = context;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _authUser = authUser;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public UserViewModel GetUserDetails(string Id)
        {
            UserViewModel user = new();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("FetchUserById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", Id ?? System.Data.SqlTypes.SqlString.Null);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new()
                            {
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                                IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                IsDeleted = reader.IsDBNull(reader.GetOrdinal("IsDeleted")) ? false : reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? null : reader.GetString(reader.GetOrdinal("Id")),
                                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                                OptometristFirmId = reader.IsDBNull(reader.GetOrdinal("OptometristFirmId")) ? null : reader.GetInt32(reader.GetOrdinal("OptometristFirmId")),
                                MobileNumber = reader.IsDBNull(reader.GetOrdinal("MobileNumber")) ? null : reader.GetString(reader.GetOrdinal("MobileNumber")),
                                OptometristFirmName = reader.IsDBNull(reader.GetOrdinal("OptometristFirmName")) ? null : reader.GetString(reader.GetOrdinal("OptometristFirmName")),
                                RoleId = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? null : reader.GetString(reader.GetOrdinal("RoleId")),
                                //DefaultRole = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                EmailConfirmed = reader.IsDBNull(reader.GetOrdinal("EmailConfirmed")) ? false : reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                                IsFirstLogin = reader.IsDBNull(reader.GetOrdinal("IsFirstLogin")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFirstLogin")),
                                Phone = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                PIN = reader.IsDBNull(reader.GetOrdinal("Pin")) ? null : reader.GetString(reader.GetOrdinal("Pin")),
                                //Role = new()
                                //{
                                //    Id = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? null : reader.GetString(reader.GetOrdinal("RoleId")),
                                //    Name = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName"))
                                //},


                            };
                        }
                    }

                }
            }

            user.Roles = new();

            ApplicationUser applicationUser = _userManager.FindByIdAsync(user.Id).GetAwaiter().GetResult();
            IList<string> roles = _userManager.GetRolesAsync(applicationUser).GetAwaiter().GetResult();

            var allRoles = _roleManager.Roles.AsNoTracking().ToList();

            foreach (ApplicationRole role in allRoles)
            {
                bool isInRole = _userManager.IsInRoleAsync(applicationUser, role.Name).GetAwaiter().GetResult();
                user.Roles.Add(new CheckBoxListItemDto { Id = role.Id, IsChecked = isInRole, Name = role.Name });
            }


            return user;
        }

        public List<UserViewModel> GetUsers(string roleName, string CreatedBy)
        {
            List<UserViewModel> users = new();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("FetchUsersByRoleName", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RoleName", roleName ?? System.Data.SqlTypes.SqlString.Null);
                    cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy ?? System.Data.SqlTypes.SqlString.Null);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new()
                            {
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                                IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                IsDeleted = reader.IsDBNull(reader.GetOrdinal("IsDeleted")) ? false : reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? null : reader.GetString(reader.GetOrdinal("Id")),
                                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                                OptometristFirmId = reader.IsDBNull(reader.GetOrdinal("OptometristFirmId")) ? null : reader.GetInt32(reader.GetOrdinal("OptometristFirmId")),
                                MobileNumber = reader.IsDBNull(reader.GetOrdinal("MobileNumber")) ? null : reader.GetString(reader.GetOrdinal("MobileNumber")),
                                OptometristFirmName = reader.IsDBNull(reader.GetOrdinal("OptometristFirmName")) ? null : reader.GetString(reader.GetOrdinal("OptometristFirmName")),
                                //RoleId = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? null : reader.GetString(reader.GetOrdinal("RoleId")),
                                BusinessName = reader.IsDBNull(reader.GetOrdinal("BusinessName")) ? null : reader.GetString(reader.GetOrdinal("BusinessName")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                EmailConfirmed = reader.IsDBNull(reader.GetOrdinal("EmailConfirmed")) ? false : reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                                IsFirstLogin = reader.IsDBNull(reader.GetOrdinal("IsFirstLogin")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFirstLogin")),
                                Phone = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                PIN = reader.IsDBNull(reader.GetOrdinal("Pin")) ? null : reader.GetString(reader.GetOrdinal("Pin")),
                                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                                //Role = new()
                                //{
                                //    Id = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? null : reader.GetString(reader.GetOrdinal("RoleId")),
                                //    Name = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName"))
                                //}
                            });
                        }
                    }

                }
            }

            foreach (var user in users)
            {
                ApplicationUser appUser = _userManager.FindByIdAsync(user.Id).GetAwaiter().GetResult();
                IList<string> roles = _userManager.GetRolesAsync(appUser).GetAwaiter().GetResult();
                user.Roles = roles.Select(role => new CheckBoxListItemDto { Name = role }).ToList();
            }

            return users;
        }

        public List<UserViewModel> GetUsersByOptometristFirm(int OptometristFirmId)
        {
            List<UserViewModel> users = new();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("FetchUserByOptometristFirmId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OptometristFirmId", OptometristFirmId);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new()
                            {
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                                IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                IsDeleted = reader.IsDBNull(reader.GetOrdinal("IsDeleted")) ? false : reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                                Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? null : reader.GetString(reader.GetOrdinal("Id")),
                                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                                OptometristFirmId = reader.GetInt32(reader.GetOrdinal("OptometristFirmId")),
                                MobileNumber = reader.IsDBNull(reader.GetOrdinal("MobileNumber")) ? null : reader.GetString(reader.GetOrdinal("MobileNumber")),
                                OptometristFirmName = reader.IsDBNull(reader.GetOrdinal("OptometristFirmName")) ? null : reader.GetString(reader.GetOrdinal("OptometristFirmName")),
                                RoleId = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? null : reader.GetString(reader.GetOrdinal("RoleId")),
                                //DefaultRole = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                EmailConfirmed = reader.IsDBNull(reader.GetOrdinal("EmailConfirmed")) ? false : reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                                IsFirstLogin = reader.IsDBNull(reader.GetOrdinal("IsFirstLogin")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFirstLogin")),
                                Phone = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                PIN = reader.IsDBNull(reader.GetOrdinal("Pin")) ? null : reader.GetString(reader.GetOrdinal("Pin")),
                                //Role = new()
                                //{
                                //    Id = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? null : reader.GetString(reader.GetOrdinal("RoleId")),
                                //    Name = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName"))
                                //}
                            });
                        }
                    }

                }
            }
            return users;
        }

        public async Task<MessageResponse> UpdateAsync(UserViewModel model)
        {
            MessageResponse result = new();

            var context = _context;
            var scope = context.Database.BeginTransaction();
            using (scope)
            {
                try
                {
                    var userDetails = _context.ApplicationUsers.FirstOrDefault(x => x.Id == model.Id);
                    if (userDetails == null)
                    {
                        scope.Rollback();
                        result.Message = "No user record found";
                        return result;
                    }
                    if (model.Email != userDetails.Email)
                    {
                        //Check is model.Email is already in use
                        bool newEmailExist = await _context.ApplicationUsers.AnyAsync(x => x.Email.ToLower() == model.Email);
                        if (newEmailExist)
                        {
                            scope.Rollback();
                            result.Message = $"{model.Email} is in use.";
                            return result;
                        }
                    }



                    userDetails.Id = model.Id;
                    userDetails.DateUpdated = DateTime.Now;
                    userDetails.Email = model.Email;
                    userDetails.FirstName = model.FirstName;
                    userDetails.LastName = model.LastName;
                    userDetails.MobileNumber = model.MobileNumber;
                    userDetails.UserName = model.Email;
                    userDetails.ModifiedBy = _authUser.UserId;
                    userDetails.IsActive = model.IsActive;
                    userDetails.EmailConfirmed = model.IsActive;
                    userDetails.NormalizedEmail = model.Email;
                    userDetails.NormalizedUserName = model.Email;
                    userDetails.UserName = model.Email;
                    //userDetails.DefaultRole = model.DefaultRole;
                    userDetails.PhoneNumber = model.Phone;
                    userDetails.Pin = model.PIN;
                    userDetails.OptometristFirmId = model.OptometristFirmId;
                    _context.SaveChanges();

                    //var role = _context.ApplicationRoles.FirstOrDefault(r => r.Name == model.DefaultRole);
                    var userRoleQuery = _context.ApplicationUserRoles.Where(u => u.UserId == model.Id);

                    IList<string> roles = _userManager.GetRolesAsync(userDetails).GetAwaiter().GetResult();
                    //if (roles.Contains(AppRoles.SYSTEMADMIN))
                    //{
                    //    scope.Rollback();
                    //    result.Message = "You cannot update your record";
                    //    return result;
                    //}

                    //Detect if there are role changes
                    if (model.Roles.Count(x => x.IsChecked) != userRoleQuery.Count() && !model.Roles.Select(x => x.Id).SequenceEqual(userRoleQuery.Select(x => x.RoleId)))
                    {
                        List<ApplicationUserRole> userRoles = userRoleQuery.ToList();
                        _context.ApplicationUserRoles.RemoveRange(userRoles);

                        userRoles = model.Roles.Where(x => x.IsChecked).Select(x => new ApplicationUserRole
                        {
                            RoleId = x.Id,
                            UserId = model.Id,
                        }).ToList();
                        _context.ApplicationUserRoles.AddRange(userRoles);
                        _context.SaveChanges();

                    }

                    var selectedRoles = model.Roles.Where(x => x.IsChecked).Select(x => x.Name).ToList();
                    if (selectedRoles.Contains(AppRoles.FRONTOFFICER) || selectedRoles.Contains(AppRoles.FACILITYOWNER) || selectedRoles.Contains(AppRoles.OPTOMETRIST))
                    {

                        var optometristUserDetails = _context.OptometristFirmUsers.FirstOrDefault(x => x.ApplicationUserId == model.Id);

                        if (optometristUserDetails == null)
                        {
                            //optometristUserDetails.OptometristFirmId = model.OptometristFirmId.GetValueOrDefault();
                            var optometristUser = new OptometristFirmUser
                            {
                                OptometristFirmId = model.OptometristFirmId.GetValueOrDefault(),
                                ApplicationUserId = model.Id
                            };
                            _context.OptometristFirmUsers.Add(optometristUser);
                            _context.SaveChanges();
                        }
                        else
                        {

                            if (optometristUserDetails.OptometristFirmId != model.OptometristFirmId)
                            {
                                var oldData = _context.OptometristFirmUsers.Where(x => x.ApplicationUserId == model.Id);
                                _context.RemoveRange(oldData);

                                var newData = new OptometristFirmUser
                                {
                                    ApplicationUserId = model.Id,
                                    CreatedDate = DateTime.UtcNow,
                                    OptometristFirmId = model.OptometristFirmId.GetValueOrDefault(),
                                    IsActive = true,
                                    IsDeleted = false
                                };
                                _context.OptometristFirmUsers.Add(newData);
                               await _context.SaveChangesAsync();
                            }
                        }

                    }

                        result.Message = "Record saved successfully";
                    result.Success = true;
                    scope.Commit();

                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    result.Message = "Kindly try again later";
                    _logger.LogError(ex.Message, ex);
                }
                return result;
            }
        }
    }
}
