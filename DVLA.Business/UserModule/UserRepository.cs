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

namespace DVLA.Business.UserModule
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<UserRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public UserRepository(DVLADbContext context, ILogger<UserRepository> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public UserViewModel GetUserDetails(string Id)
        {
            UserViewModel users = new();
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
                            users = new()
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
                                DefaultRole = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                EmailConfirmed = reader.IsDBNull(reader.GetOrdinal("EmailConfirmed")) ? false : reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                                IsFirstLogin = reader.IsDBNull(reader.GetOrdinal("IsFirstLogin")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFirstLogin")),
                                Phone = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                PIN = reader.IsDBNull(reader.GetOrdinal("Pin")) ? null : reader.GetString(reader.GetOrdinal("Pin")),
                                Role = new()
                                {
                                    Id = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? null : reader.GetString(reader.GetOrdinal("RoleId")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName"))
                                },
                                
                                
                            };
                        }
                    }

                }
            }
            return users;
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
                                RoleId = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? null : reader.GetString(reader.GetOrdinal("RoleId")),
                                DefaultRole = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                EmailConfirmed = reader.IsDBNull(reader.GetOrdinal("EmailConfirmed")) ? false : reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                                IsFirstLogin = reader.IsDBNull(reader.GetOrdinal("IsFirstLogin")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFirstLogin")),
                                Phone = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                PIN = reader.IsDBNull(reader.GetOrdinal("Pin")) ? null : reader.GetString(reader.GetOrdinal("Pin")),
                                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                                Role = new()
                                {
                                    Id = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? null : reader.GetString(reader.GetOrdinal("RoleId")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName"))
                                }
                            });
                        }
                    }

                }
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
                                DefaultRole = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                EmailConfirmed = reader.IsDBNull(reader.GetOrdinal("EmailConfirmed")) ? false : reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                                IsFirstLogin = reader.IsDBNull(reader.GetOrdinal("IsFirstLogin")) ? false : reader.GetBoolean(reader.GetOrdinal("IsFirstLogin")),
                                Phone = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                PIN = reader.IsDBNull(reader.GetOrdinal("Pin")) ? null : reader.GetString(reader.GetOrdinal("Pin")),
                                Role = new()
                                {
                                    Id = reader.IsDBNull(reader.GetOrdinal("RoleId")) ? null : reader.GetString(reader.GetOrdinal("RoleId")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : reader.GetString(reader.GetOrdinal("RoleName"))
                                }
                            });
                        }
                    }

                }
            }
            return users;
        }

        public bool Update(UserViewModel model, string updatedBy, out string responseMessage)
        {
            responseMessage = "";

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
                        responseMessage = "No user record found";
                        return false;
                    }
                    if (model.Email != userDetails.Email)
                    {
                        scope.Rollback();
                        responseMessage = "Email address cannot be changed.";
                        return false;
                    }

                    userDetails.Id = model.Id;
                    userDetails.DateUpdated = DateTime.Now;
                    //userDetails.Email = model.Email;
                    userDetails.FirstName = model.FirstName;
                    userDetails.LastName = model.LastName;
                    userDetails.MobileNumber = model.MobileNumber;
                    userDetails.UserName = model.Email;
                    userDetails.ModifiedBy = updatedBy;
                    userDetails.IsActive = model.IsActive;
                    userDetails.EmailConfirmed = model.IsActive;
                    userDetails.DefaultRole = model.DefaultRole;
                    userDetails.PhoneNumber = model.Phone;
                    userDetails.Pin = model.PIN;
                    userDetails.OptometristFirmId = model.OptometristFirmId;
                    _context.SaveChanges();

                    var role = _context.ApplicationRoles.FirstOrDefault(r => r.Name == model.DefaultRole);
                    var userRole = _context.ApplicationUserRoles.FirstOrDefault(u => u.UserId == model.Id);
                    if (userRole != null && userRole.RoleId != role.Id)
                    {
                        _context.Entry(userRole).State = EntityState.Deleted;
                        _context.SaveChanges();

                        var newRole = new ApplicationUserRole
                        {
                            RoleId = role.Id,
                            UserId = model.Id
                        };
                        _context.ApplicationUserRoles.Add(newRole);
                        _context.SaveChanges();
                    }

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
                    else if (optometristUserDetails != null && optometristUserDetails.OptometristFirmId != model.OptometristFirmId && model.DefaultRole != AppRoles.SYSTEMADMIN)
                    {
                        var oldData = _context.OptometristFirmUsers.FirstOrDefault(x => x.ApplicationUserId == model.Id && x.OptometristFirmId == optometristUserDetails.OptometristFirmId);
                        _context.OptometristFirmUsers.Remove(oldData);
                        _context.SaveChanges(true);

                        optometristUserDetails.OptometristFirmId = model.OptometristFirmId.GetValueOrDefault();
                        var optometristUser = new OptometristFirmUser
                        {
                            OptometristFirmId = model.OptometristFirmId.GetValueOrDefault(),
                            ApplicationUserId = model.Id
                        };
                        _context.OptometristFirmUsers.Add(optometristUser);
                        _context.SaveChanges();

                       
                    }

                    

                    responseMessage = "Record saved successfully";
                    scope.Commit();
                    return true;
                    
                }
                catch (Exception ex)
                {
                    scope.Rollback();
                    responseMessage = "Kindly try again later";
                    _logger.LogError(ex.Message, ex);
                    return false;
                }

            }
        }
    }
}
