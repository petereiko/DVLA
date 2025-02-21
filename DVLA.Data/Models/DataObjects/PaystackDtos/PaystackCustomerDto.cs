using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.PaystackDtos
{
    public class PaystackCustomerDto
    {
        public long id {  get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string customer_code { get; set; }
        public string phone {  get; set; }
        public object metadata { get; set; }
        public string risk_action { get; set; }
        public object international_format_phone { get; set; }
        //"id": 200409834,
        //    "first_name": null,
        //    "last_name": null,
        //    "email": "johnsaliu@gmail.com",
        //    "customer_code": "CUS_75d0qcgv8jwts3s",
        //    "phone": null,
        //    "metadata": null,
        //    "risk_action": "default",
        //    "international_format_phone": null
    }
}
