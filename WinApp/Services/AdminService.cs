using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WinApp.Data;
using WinApp.Models;

namespace WinApp.Services
{
    public class AdminService
    {

        public static async Task<MessageResponse> DeleteAllEntities()
        {
            MessageResponse response = new MessageResponse();
            try
            {
                using (DVLADBContext context = new DVLADBContext())
                {
                    context.ColourVisionScores.RemoveRange(context.ColourVisionScores);
                    context.VisualAcuityScores.RemoveRange(context.VisualAcuityScores);
                    context.VisualFieldScores.RemoveRange(context.VisualFieldScores);
                    context.OptometristFirmUsers.RemoveRange(context.OptometristFirmUsers);
                    context.OptometristFirms.RemoveRange(context.OptometristFirms);
                    context.Districts.RemoveRange(context.Districts);
                    context.Regions.RemoveRange(context.Regions);
                    context.AspNetRoles.RemoveRange(context.AspNetRoles);
                    context.AspNetUserRoles.RemoveRange(context.AspNetUserRoles);
                    context.AspNetUsers.RemoveRange(context.AspNetUsers);
                    await context.SaveChangesAsync();
                }
                response.Success = true;
                response.Message = "Entries deleted";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                response.Message = ex.Message;
            }
            return response;
        }

        public static async Task<MessageResponse> SynchUsers()
        {
            MessageResponse result = new MessageResponse();
            result = await InsertRoles();
            if (result.Success)
            {
                result = await InsertUsers();

                if (result.Success)
                {
                    result = await InsertUserRoles();
                }
            }
            return result;
        }

        private static async Task<MessageResponse> InsertUsers()
        {
            MessageResponse result = new MessageResponse();
            try
            {
                List<ApplicationUser> users = new List<ApplicationUser>();

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/AdminOperation/fetchusers");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<List<ApplicationUser>>(json);
                }

                int counter = 0;

                using (DVLADBContext context = new DVLADBContext())
                {
                    foreach (var user in users)
                    {
                        AspNetUser aspNetUser = new AspNetUser
                        {
                            Email = user.Email,
                            AccessFailedCount = user.AccessFailedCount,
                            Address = user.Address,
                            ConcurrencyStamp = user.ConcurrencyStamp,
                            CreatedBy = user.CreatedBy,
                            CreatedDate = user.CreatedDate,
                            DateUpdated = user.DateUpdated,
                            DefaultRole = user.DefaultRole,
                            DepartmentId = user.DepartmentId,
                            DOB = user.DOB,
                            EmailConfirmed = user.EmailConfirmed,
                            FirstName = user.FirstName,
                            Id = user.Id,
                            IsActive = user.IsActive,
                            IsDeleted = user.IsDeleted,
                            IsFirstLogin = user.IsFirstLogin,
                            LastName = user.LastName,
                            LockoutEnabled = user.LockoutEnabled,
                            LockoutEnd = user.LockoutEnd,
                            MiddleName = user.MiddleName,
                            MobileNumber = user.MobileNumber,
                            ModifiedBy = user.ModifiedBy,
                            NormalizedEmail = user.NormalizedEmail,
                            NormalizedUserName = user.NormalizedUserName,
                            OptometristFirmId = user.OptometristFirmId,
                            PasswordHash = user.PasswordHash,
                            PhoneNumber = user.PhoneNumber,
                            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                            Pin = user.Pin,
                            SecurityStamp = user.SecurityStamp,
                            TwoFactorEnabled = user.TwoFactorEnabled,
                            UserName = user.UserName
                        };
                        context.AspNetUsers.Add(aspNetUser);
                    }
                    await context.SaveChangesAsync();
                }
                result.Success = true;
                result.Message = $"{counter} users saved";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = ex.Message;
            }
            return result;
        }
        private static async Task<MessageResponse> InsertRoles()
        {
            MessageResponse result = new MessageResponse();
            try
            {
                List<ApplicationRole> roles = new List<ApplicationRole>();

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/AdminOperation/fetchroles");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    roles = JsonConvert.DeserializeObject<List<ApplicationRole>>(json);
                }

                int counter = 0;

                using (DVLADBContext context = new DVLADBContext())
                {
                    var dbRoles = context.AspNetRoles.AsNoTracking();
                    context.AspNetRoles.RemoveRange(dbRoles);
                    await context.SaveChangesAsync();

                    foreach (var role in roles)
                    {
                        counter++;
                        AspNetRole aspNetRole = new AspNetRole
                        {
                            Id = role.Id,
                            Name = role.Name,
                            NormalizedName = role.NormalizedName
                        };
                        context.AspNetRoles.Add(aspNetRole);
                    }
                    await context.SaveChangesAsync();
                }
                result.Success = true;
                result.Message = $"{counter} users saved";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = ex.Message;
            }
            return result;
        }
        private static async Task<MessageResponse> InsertUserRoles()
        {
            MessageResponse result = new MessageResponse();
            try
            {
                List<ApplicationUserRole> userRoles = new List<ApplicationUserRole>();

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/AdminOperation/fetchuserroles");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    userRoles = JsonConvert.DeserializeObject<List<ApplicationUserRole>>(json);
                }

                int counter = 0;

                using (DVLADBContext context = new DVLADBContext())
                {


                    foreach (var userRole in userRoles)
                    {
                        counter++;
                        AspNetUserRole aspNetUserRole = new AspNetUserRole
                        {
                            UserId = userRole.UserId,
                            RoleId = userRole.RoleId,
                            Discriminator = "ApplicationUserRole"
                        };
                        context.AspNetUserRoles.Add(aspNetUserRole);
                    }
                    await context.SaveChangesAsync();
                }
                result.Success = true;
                result.Message = $"{counter} users saved";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = ex.Message;
            }
            return result;
        }


        public static async Task<MessageResponse> SyncLocations()
        {
            MessageResponse result = new MessageResponse();
            result = await InsertRegion();
            if (result.Success)
            {
                result = await InsertDistrict();
            }
            return result;
        }

        private static async Task<MessageResponse> InsertDistrict()
        {
            MessageResponse result = new MessageResponse();
            try
            {
                List<District> districts = new List<District>();

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/AdminOperation/districts");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    districts = JsonConvert.DeserializeObject<List<District>>(json);
                }

                int counter = 0;

                using (DVLADBContext context = new DVLADBContext())
                {
                    foreach (var district in districts)
                    {
                        counter++;
                        District d = new District
                        {
                            Id = district.Id,
                            Name = district.Name,
                            RegionId = district.RegionId
                        };
                        context.Districts.Add(d);
                    }
                    await context.SaveChangesAsync();
                }
                result.Success = true;
                result.Message = $"{counter} districts saved";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = ex.Message;
            }
            return result;
        }
        private static async Task<MessageResponse> InsertRegion()
        {
            MessageResponse result = new MessageResponse();
            try
            {
                List<Region> regions = new List<Region>();

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/AdminOperation/regions");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    regions = JsonConvert.DeserializeObject<List<Region>>(json);
                }

                int counter = 0;

                using (DVLADBContext context = new DVLADBContext())
                {
                    foreach (var region in regions)
                    {
                        counter++;
                        Region r = new Region
                        {
                            Id = region.Id,
                            Name = region.Name,
                            PrefixName = region.PrefixName
                        };
                        context.Regions.Add(r);
                    }
                    await context.SaveChangesAsync();
                }
                result.Success = true;
                result.Message = $"{counter} regions saved";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = ex.Message;
            }
            return result;
        }


        public static async Task<MessageResponse> SyncClinicals()
        {
            MessageResponse result = new MessageResponse();
            result = await InsertColorVisionScores();
            if (result.Success)
            {
                result = await InsertVisualAcuityScores();
                if (result.Success)
                {
                    result = await InsertVisualFieldScores();
                }
            }
            return result;
        }
        private static async Task<MessageResponse> InsertColorVisionScores()
        {
            MessageResponse result = new MessageResponse();
            try
            {
                List<ColourVisionScore> colourVisionScores = new List<ColourVisionScore>();

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/AdminOperation/colorvisionscores");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    colourVisionScores = JsonConvert.DeserializeObject<List<ColourVisionScore>>(json);
                }

                int counter = 0;

                using (DVLADBContext context = new DVLADBContext())
                {

                    foreach (var colorVisionScore in colourVisionScores)
                    {
                        counter++;
                        ColourVisionScore cv = new ColourVisionScore
                        {
                            Id = colorVisionScore.Id,
                            CreatedBy = colorVisionScore.CreatedBy,
                            CreatedDate = colorVisionScore.CreatedDate,
                            IsActive = colorVisionScore.IsActive,
                            IsDeleted = colorVisionScore.IsDeleted,
                            ModifiedBy = colorVisionScore.ModifiedBy,
                            ModifiedDate = colorVisionScore.ModifiedDate,
                            //RowVersion = colorVisionScore.RowVersion,
                            Score = colorVisionScore.Score
                        };
                        context.ColourVisionScores.Add(cv);

                    }
                    await context.SaveChangesAsync();
                }
                result.Success = true;
                result.Message = $"{counter} color vision scores saved";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = ex.Message;
            }
            return result;
        }
        private static async Task<MessageResponse> InsertVisualAcuityScores()
        {
            MessageResponse result = new MessageResponse();
            try
            {
                List<VisualAcuityScore> actuityScore = new List<VisualAcuityScore>();

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/AdminOperation/visualacuityscores");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    actuityScore = JsonConvert.DeserializeObject<List<VisualAcuityScore>>(json);
                }

                int counter = 0;

                using (DVLADBContext context = new DVLADBContext())
                {
                    foreach (var colorVisionScore in actuityScore)
                    {
                        counter++;
                        VisualAcuityScore aspNetRole = new VisualAcuityScore
                        {
                            Id = colorVisionScore.Id,
                            CreatedBy = colorVisionScore.CreatedBy,
                            CreatedDate = colorVisionScore.CreatedDate,
                            IsActive = colorVisionScore.IsActive,
                            IsDeleted = colorVisionScore.IsDeleted,
                            ModifiedBy = colorVisionScore.ModifiedBy,
                            ModifiedDate = colorVisionScore.ModifiedDate,
                            //RowVersion = colorVisionScore.RowVersion,
                            Score = colorVisionScore.Score
                        };
                        context.VisualAcuityScores.Add(aspNetRole);
                        
                    }
                    await context.SaveChangesAsync();
                }
                result.Success = true;
                result.Message = $"{counter} visual acuity scores saved";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = ex.Message;
            }
            return result;
        }
        private static async Task<MessageResponse> InsertVisualFieldScores()
        {
            MessageResponse result = new MessageResponse();
            try
            {
                List<VisualFieldScore> colourVisionScores = new List<VisualFieldScore>();

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/AdminOperation/visualfieldscores");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    colourVisionScores = JsonConvert.DeserializeObject<List<VisualFieldScore>>(json);
                }

                int counter = 0;

                using (DVLADBContext context = new DVLADBContext())
                {
                    foreach (var colorVisionScore in colourVisionScores)
                    {
                        counter++;
                        VisualFieldScore aspNetRole = new VisualFieldScore
                        {
                            Id = colorVisionScore.Id,
                            CreatedBy = colorVisionScore.CreatedBy,
                            CreatedDate = colorVisionScore.CreatedDate,
                            IsActive = colorVisionScore.IsActive,
                            IsDeleted = colorVisionScore.IsDeleted,
                            ModifiedBy = colorVisionScore.ModifiedBy,
                            ModifiedDate = colorVisionScore.ModifiedDate,
                            //RowVersion = colorVisionScore.RowVersion,
                            Score = colorVisionScore.Score
                        };
                        context.VisualFieldScores.Add(aspNetRole);
                    }
                    await context.SaveChangesAsync();

                }
                result.Success = true;
                result.Message = $"{counter} visual field scores saved";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = ex.Message;
            }
            return result;
        }


        public static async Task<MessageResponse> SyncOptometristFirms()
        {
            MessageResponse result = new MessageResponse();
            result = await InsertOprometristFirms();
            if (result.Success)
            {
                result = await InsertOptometristFirmUsers();
            }
            return result;
        }



        private static async Task<MessageResponse> InsertOprometristFirms()
        {
            MessageResponse result = new MessageResponse();
            try
            {
                List<OptometristFirm> optometristFirms = new List<OptometristFirm>();

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/AdminOperation/optometristfirms");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    optometristFirms = JsonConvert.DeserializeObject<List<OptometristFirm>>(json);
                }

                int counter = 0;

                using (DVLADBContext context = new DVLADBContext())
                {
                    foreach (var optometristFirm in optometristFirms)
                    {
                        counter++;
                        OptometristFirm firm = new OptometristFirm
                        {
                            Id = optometristFirm.Id,
                            CreatedBy = optometristFirm.CreatedBy,
                            CreatedDate = optometristFirm.CreatedDate,
                            IsActive = optometristFirm.IsActive,
                            IsDeleted = optometristFirm.IsDeleted,
                            ModifiedBy = optometristFirm.ModifiedBy,
                            ModifiedDate = optometristFirm.ModifiedDate,
                            AccreditationNumber = optometristFirm.AccreditationNumber,
                            BusinessAddress = optometristFirm.BusinessAddress,
                            BusinessName = optometristFirm.BusinessName,
                            CentreCode = optometristFirm.CentreCode,
                            ContactEmail = optometristFirm.ContactEmail,
                            ContactFirstName = optometristFirm.ContactFirstName,
                            ContactLastName = optometristFirm.ContactLastName,
                            ContactPhoneNumber = optometristFirm.ContactPhoneNumber,
                            DigitalAddress = optometristFirm.DigitalAddress,
                            DistrictId = optometristFirm.DistrictId,
                            IsSynchronized = optometristFirm.IsSynchronized,
                            MobileNumber = optometristFirm.MobileNumber,
                            RegionId = optometristFirm.RegionId,
                            RegistrationNumber = optometristFirm.RegistrationNumber,
                            ReorderLevel = optometristFirm.ReorderLevel,
                            TelephoneNumber = optometristFirm.TelephoneNumber,
                            Town = optometristFirm.Town
                        };
                        context.OptometristFirms.Add(firm);
                    }
                    await context.SaveChangesAsync();

                }
                result.Success = true;
                result.Message = $"{counter} optometrist firms saved";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = ex.Message;
            }
            return result;
        }
        private static async Task<MessageResponse> InsertOptometristFirmUsers()
        {
            MessageResponse result = new MessageResponse();
            try
            {
                List<OptometristFirmUser> optometristFirmUsers = new List<OptometristFirmUser>();

                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost/api/AdminOperation/optometristfirmusers");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    optometristFirmUsers = JsonConvert.DeserializeObject<List<OptometristFirmUser>>(json);
                }

                int counter = 0;

                using (DVLADBContext context = new DVLADBContext())
                {
                    foreach (var optometristFirmUser in optometristFirmUsers)
                    {
                        counter++;
                        OptometristFirmUser optUser = new OptometristFirmUser
                        {
                            Id = optometristFirmUser.Id,
                            CreatedBy = optometristFirmUser.CreatedBy,
                            CreatedDate = optometristFirmUser.CreatedDate,
                            IsActive = optometristFirmUser.IsActive,
                            IsDeleted = optometristFirmUser.IsDeleted,
                            ModifiedBy = optometristFirmUser.ModifiedBy,
                            ModifiedDate = optometristFirmUser.ModifiedDate,
                            ApplicationUserId = optometristFirmUser.ApplicationUserId,
                            OptometristFirmId = optometristFirmUser.OptometristFirmId
                        };
                        context.OptometristFirmUsers.Add(optUser);
                    }
                    await context.SaveChangesAsync();

                }
                result.Success = true;
                result.Message = $"{counter} visual field scores saved";
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
