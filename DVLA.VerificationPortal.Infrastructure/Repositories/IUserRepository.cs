
using DVLA.VerificationPortal.Infrastructure.Models;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Requests;
using DVLA.VerificationPortal.Shared.Responses;

namespace DVLA.VerificationPortal.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        //Task<MessageResponse> ChangePasswordAsync(ChangePasswordRequest model);
        //Task<bool> ConfirmEmail(string encodedToken, string userid);
        //Task<MessageResponse> EditUser(EditUserRequest model);
        //Task<string> GeneratePasswordResetToken(string id);
        //List<RoleDto> GetAllRoles();
        //Task<List<ApplicationUserDto>> GetAllUsers();
        Task<List<string>> GetRolesAsync(ApplicationUserDto user);
        Task<ApplicationUserDto> GetUserByEmail(string email);
        //Task<PaginatedResponse<ApplicationUserDto>> GetUsersAsync(int pageIndex1, int pageSize1);
        //Task<List<ApplicationUserDto>> GetUsersInRole(string roleName);
        //Task<MessageResponse> Login(LoginRequest model);
        //Task<MessageResponse> Logout();
        //Task<MessageResponse> OnboardUserAsync(OnboardUserRequest model);
        //Task<MessageResponse> ResetPasswordAsync(ResetPasswordRequest model);
        //Task<MessageResponse> SendResetPasswordToken(ForgotPasswordRequest model);
    }
}