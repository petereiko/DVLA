using DVLA.WindowsApplication.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.WindowsApplication.Business
{
    public static class ErrorLogger
    {
        public static void Log(Exception ex)
        {
            using (DVLAContext context=new DVLAContext())
            {
                ErrorLog log = new ErrorLog
                {
                    CreatedDate = DateTime.Now,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                context.ErrorLogs.Add(log);
                context.SaveChanges();
            }
        }
    }
}
