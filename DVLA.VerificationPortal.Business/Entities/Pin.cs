using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Domain.Entities
{
    public class Pin
    {
        public long Id { get; set; }
        public string? PinVal { get; set; }
        public string? Serial { get; set; }
        public int UseCount { get; set; }
        public bool IsActive {  get; set; }
        public int? ApiClientId { get; set; }
        public virtual ApiClient? ApiClient { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
