using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.WindowsApplication.Models
{
    public class IdNameModel<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }
    }

}
