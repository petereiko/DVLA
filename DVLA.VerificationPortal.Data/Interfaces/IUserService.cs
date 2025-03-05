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
    public interface IUserService
    {
        Task<ApplicationUserDto> OnboardUserAsync(OnboardUserRequest request);
        Task<MessageResponse> SendResetPasswordTokenAsync(ForgotPasswordRequest request);
        Task<bool> ConfirmEmailAsync(string encodedToken, string userid);
        Task<MessageResponse> ResetPasswordAsync(ResetPasswordRequest request);
        Task<MessageResponse> ChangePasswordAsync(ChangePasswordRequest request);
        Task<MessageResponse> LogoutAsync();
        Task<ApplicationUserDto> UpdateAsync(EditUserRequest request);
        Task<PaginatedResponse<ApplicationUserDto>> GetAllAsync(int pageIndex, int pageSize);
        List<RoleDto> GetAllRoles();
        Task<ApplicationUserDto> LoginAsync(LoginRequest request);
        Task SeedRolesAsync();
        Task<ApplicationUserDto> GetUserByEmailAsync(string email);
        Task<string> GeneratePasswordResetTokenAsync(string id);
        Task<ApplicationUserDto> GetUserByIdAsync(string id);
        Task SeedSuperAdminAsync();
    }
}
