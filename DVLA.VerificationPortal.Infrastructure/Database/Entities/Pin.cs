using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Database.Entities
{
    public class Pin
    {
        public long Id { get; set; }
        public string PinVal { get; set; } = string.Empty;
        public string Serial { get; set; } = string.Empty;
        public int UseCount { get; set; }
        public bool IsActive { get; set; }
        public int? ApiClientId { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation property (Foreign Key → ApiClients)
        public ApiClient? ApiClient { get; set; }
    }
}
