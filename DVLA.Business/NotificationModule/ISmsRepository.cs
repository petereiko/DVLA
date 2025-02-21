using DVLA.Data;
using DVLA.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.NotificationModule
{
    public interface ISmsRepository
    {
        void SendAssessmentResult(string firstName, string mobileNumber, string referenceNumber, string assessmentResult, DVLADbContext context = null);
        Task SendRegistrationDetail(string firstName, string mobileNumber, string referenceNumber);

        Task SendPendingSms();

        Task<Tuple<bool,string>> SendSmsIntegration(string message, string mobileNumber);
        Task SendReminder(List<RemindersModel> reminders);
    }
}
