using DVLA.Data.Models.DataObjects.DTOs;
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

        bool Update(UserViewModel model, string updatedBy, out string responseMessage);
    }
}
