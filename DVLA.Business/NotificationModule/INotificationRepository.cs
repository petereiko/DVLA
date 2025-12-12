using DVLA.Data;
using DVLA.Data.Models;
using DVLA.Data.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.NotificationModule
{
    public interface INotificationRepository
    {
        void SendNewAccountCreated(ApplicationUser model, string password, DVLADbContext context = null);
        void SendPasswordReset(ApplicationUser model, string callbackUrl);

        void SendForgotPassword(ApplicationUser model, string callbackUrl);
        void SlotRequestNotification(ApplicationUser model, int slotQuantity, string requestId, string callbackUrl);
        void SlotApprovalNotification(ApplicationUser model, int slotQuantity, string callbackUrl);
        void SlotDeclineNotification(ApplicationUser model, int slotQuantity, string callbackUrl);
        void SendAssessmentResult(string firstName, string mobileNumber, string referenceNumber, string assessmentResult, string emailTo, DVLADbContext context = null);
        Task SendRegistrationDetail(string firstName, string mobileNumber, string referenceNumber, string emailTo);
        Task SendReminder(List<RemindersModel> reminders);
        void Dispose();
    }
}
