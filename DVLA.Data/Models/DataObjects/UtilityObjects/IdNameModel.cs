using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.UtilityObjects
{
    public class IdNameModel<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }
    }
}
