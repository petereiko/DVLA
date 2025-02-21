using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinApp.Data;

namespace WinApp.Services
{
    public class ErrorLogger
    {
        public static void Log(Exception exception) 
        {
            using (DVLADBContext context=new DVLADBContext())
            {
                ErrorLog log = new ErrorLog
                {
                    CreatedDate = DateTime.Now,
                    Errormsg = exception.Message, 
                     Extype = exception.StackTrace
                };
                context.ErrorLogs.Add(log);
                context.SaveChanges();
            }
        }
    }
}
