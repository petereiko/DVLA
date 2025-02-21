using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    public class Reminder : BaseObjectInt64
    {
        public DateTime DayRan { get; set; }
        public bool IsRan { get; set; }
        public int ReminderType { get; set; }
    }
}
