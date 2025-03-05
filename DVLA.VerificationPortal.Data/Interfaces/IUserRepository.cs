using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Requests;
using DVLA.VerificationPortal.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<ApplicationUserDto>> GetUsersInRole(string roleName);
        Task<bool> ConfirmEmail(string encodedToken, string userid);
        Task<string> GeneratePasswordResetToken(string id);
        Task<ApplicationUserDto> GetUserByEmail(string email);
        Task<ApplicationUserDto> GetUserById(string id);
        Task<PaginatedResponse<ApplicationUserDto>> GetUsersAsync(int pageIndex, int pageSize);
        List<RoleDto> GetAllRoles();
        Task<ApplicationUserDto> Login(LoginRequest model);
        Task<MessageResponse> Logout();
        Task<MessageResponse> SendResetPasswordToken(ForgotPasswordRequest model);
        Task<ApplicationUserDto> OnboardUserAsync(OnboardUserRequest model);
        Task<MessageResponse> ResetPasswordAsync(ResetPasswordRequest model);
        Task<MessageResponse> ChangePasswordAsync(ChangePasswordRequest model);
        Task<ApplicationUserDto> EditUser(EditUserRequest model);
        Task<List<ApplicationUserDto>> GetAllUsers();
        Task SeedRoles();
        Task SeedSuperAdmin();
        Task<List<string>> GetRolesAsync(ApplicationUserDto user);
    }
}
