using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.WindowsApplication.Models
{
    public class MessageResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }

    public class MessageResponse<T>
    {
        public T Result { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
