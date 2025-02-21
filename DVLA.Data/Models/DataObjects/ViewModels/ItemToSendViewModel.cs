using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class ItemToSendViewModel
    {
        public long Id { get; set; }
        public bool IsSent { get; set; }
        public string responseId { get; set; }
    }
}
