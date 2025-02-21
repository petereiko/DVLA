using DVLA.WindowsApplication.Data;
using DVLA.WindowsApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.WindowsApplication.Business
{
    public class AdminService
    {
        public static async Task<MessageResponse> Login(string email, string password)
        {
            MessageResponse response = new MessageResponse();
            try
            {
                using (DVLAContext context=new DVLAContext())
                {
                    response.Success = await context.SystemAdmins.AnyAsync(x => x.Email == email && x.Password == password && x.IsActive);
                    response.Message = response.Success ? "Login successful" : "Invalid credentials";
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
