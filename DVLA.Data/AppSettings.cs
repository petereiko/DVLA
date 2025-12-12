using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data
{
    public class AppSettings
    {
        public string AppName { get; set; }
        public string AppShortName { get; set; }
        public string BaseUrl { get; set; }
        public int CacheDuration { get; set; }
        public string PaystackSecretKey { get; set; }
        public string PaystackPublicKey { get; set; }
        public string Asiri { get; set; }
        public bool Online { get; set; }
        public string VerificationPortal { get; set; }
        public bool RunPushAssessmentResult { get; set; }
        public string ApiVerificationPushUrl { get; set; }
        public string ApiVerificationUpdateDocUrl { get; set; }
        public string ApiVerificationTransmitOpometristFirmsUrl { get; set; }
        public int PassportMaxSize { get; set; }
        public string ApiKey { get; set; }
    }
}
