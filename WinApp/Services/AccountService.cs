using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinApp.Data;
using WinApp.Models;

namespace WinApp.Services
{
    public class AccountService
    {
        public static async Task<MessageResponse> Login(LoginModel model)
        {
            MessageResponse result = new MessageResponse();
            try
            {
                using (DVLADBContext context=new DVLADBContext())
                {
                    SystemAdmin user = await context.SystemAdmins.FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);
                    if (user == null)
                    {
                        result.Message = "Invalid user credential";
                        return result;
                    }
                    if (user.IsActive != true)
                    {
                        result.Message = "Account has been deactivated";
                        return result;
                    }
                    result.Message = "Login successful";
                    result.Success = true;
                }
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
