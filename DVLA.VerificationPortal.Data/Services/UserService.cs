using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Requests;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApplicationUserDto> OnboardUserAsync(OnboardUserRequest request)
        {
            ApplicationUserDto result = await _userRepository.OnboardUserAsync(request);
            return result;

        }

        public async Task<MessageResponse> SendResetPasswordTokenAsync(ForgotPasswordRequest request)
        {
            MessageResponse result = await _userRepository.SendResetPasswordToken(request);
            return result;

        }

        public async Task<bool> ConfirmEmailAsync(string encodedToken, string userid)
        {
            bool result = await _userRepository.ConfirmEmail(encodedToken, userid);
            return result;

        }

        public async Task<MessageResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            MessageResponse result = await _userRepository.ResetPasswordAsync(request);
            return result;

        }

        public async Task<MessageResponse> LogoutAsync()
        {
            MessageResponse result = await _userRepository.Logout();
            return result;

        }

        public async Task<PaginatedResponse<ApplicationUserDto>> GetAllAsync(int pageIndex, int pageSize)
        {
            PaginatedResponse<ApplicationUserDto> response = await _userRepository.GetUsersAsync(pageIndex, pageSize);
            return response;
        }

        public List<RoleDto> GetAllRoles()
        {
            var response = _userRepository.GetAllRoles();
            return response;
        }


        public async Task<ApplicationUserDto> UpdateAsync(EditUserRequest request)
        {
            ApplicationUserDto result = await _userRepository.EditUser(request);
            return result;
        }

        public async Task<ApplicationUserDto> LoginAsync(LoginRequest request)
        {
            ApplicationUserDto result = await _userRepository.Login(request);
            return result;
        }

        public async Task<MessageResponse> ChangePasswordAsync(ChangePasswordRequest request)
        {
            MessageResponse result = await _userRepository.ChangePasswordAsync(request);
            return result;
        }

        public async Task<List<SelectListItem>> GetAllRolesAsync()
        {
            List<SelectListItem> roles = new();
            var result = _userRepository.GetAllRoles().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            }).ToList();

            return result;
        }

        public async Task<ApplicationUserDto> GetUserByEmailAsync(string email)
        {
            ApplicationUserDto result = await _userRepository.GetUserByEmail(email);
            return result;
        }

        public async Task<ApplicationUserDto> GetUserByIdAsync(string id)
        {
            ApplicationUserDto result = await _userRepository.GetUserById(id);
            return result;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string id)
        {
            string result = await _userRepository.GeneratePasswordResetToken(id);
            return result;
        }

        public async Task SeedRolesAsync()
        {
            await _userRepository.SeedRoles();
        }

        public async Task SeedSuperAdminAsync()
        {
            await _userRepository.SeedSuperAdmin();
        }
    }
}
