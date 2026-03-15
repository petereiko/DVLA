using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Infrastructure.Models;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Requests;
using DVLA.VerificationPortal.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Repositories
{
    public interface IUserService
    {
        Task<ApplicationUser?> GetUserByEmail(string email);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);

        Task<MessageResponse> LoginAsync(LoginRequest loginRequest);
        Task<MessageResponse> OnboardUserAsync(OnboardUserRequest request);

        Task<MessageResponse> SendResetPasswordTokenAsync(ForgotPasswordRequest model);
        Task<bool> ConfirmEmail(string encodedToken, string userid);
        Task<MessageResponse> ResetPasswordAsync(ResetPasswordRequest model);
        Task<MessageResponse> ChangePasswordAsync(ChangePasswordRequest model);
        Task<PaginatedResponse<ApplicationUserDto>> GetAllAsync(int pageIndex, int pageSize);
        Task<MessageResponse> SendResetPasswordToken(ForgotPasswordRequest model);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<IList<string>> GetUserRoles(ApplicationUser user);
        Task<List<ApplicationRole>> GetAllRoles();
        Task<MessageResponse> EditUser(EditUserRequest model);
        Task Logout();
    }

}
