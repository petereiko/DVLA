using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.PaystackDtos
{
    public class VerificationData
    {
        public long Id { get; set; }
        public string domain { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public string receipt_number { get; set; }
        public decimal amount { get; set; }
        public string message { get; set; }
        public string gateway_response {  get; set; }
        public DateTime? paid_at { get; set; }
        public DateTime created_at { get; set; }
        public string channel {  get; set; }
        public string currency { get; set; }
        public string ip_address { get; set; }
        public object metadata {  get; set; }
        public object log { get; set; }
        public object fees { get; set; }
        public object fees_split { get; set; }
        public object authorization {  get; set; }
        public PaystackCustomerDto customer { get; set; }
        public object plan { get; set; }
        public object split {  get; set; }
        public object order_id { get; set; }
        public DateTime? paidAt { get; set; }
        public DateTime createdAt { get; set;}
        public decimal requested_amount { get; set; }
        public object pos_transaction_data { get; set; }
        public object source { get; set; }
        public object fees_breakdown { get; set; }
        public object connect { get; set; }
        public DateTime transaction_date { get; set; }
        public object plan_object { get; set; }
        public object subaccount { get; set; }
    }
}
