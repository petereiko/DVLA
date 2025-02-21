using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Models
{
    public class MessageResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class MessageResponse<T> : MessageResponse
    {
        public T Result { get; set; }
    }
}
