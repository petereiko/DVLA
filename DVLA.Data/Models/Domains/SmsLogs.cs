using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    [Table("SmsLogs")]
    public class SmsLog:BaseObjectInt64
    {
        public string MobileNumber { get; set; }
        public string Message { get; set; }
        public bool IsSent { get; set; } = false;
        public string ResponseId { get; set; }
        public int RetryCount { get; set; }
    }
}
