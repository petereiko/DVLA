using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.UserModule
{
    public interface IUserRepository
    {
        List<UserViewModel> GetUsers(string roleName, string CreatedBy);
        List<UserViewModel> GetUsersByOptometristFirm(int OptometristFirmId);
        //List<UserViewModel> GetOptometristFirmUsers(int OptometristFirmId);

        UserViewModel GetUserDetails(string Id);
        Task<MessageResponse> UpdateAsync(UserViewModel model);

    }
}
