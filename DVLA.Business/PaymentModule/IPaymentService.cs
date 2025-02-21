using DVLA.Data.Models.DataObjects.PaystackDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.PaymentModule
{
    public interface IPaymentService
    {
        VerificationResponse VerifyPayment(string reference);
        Task<InitiatePaymentResponse> InitiatePayment(InitiatePaymentRequest model);
    }
}
