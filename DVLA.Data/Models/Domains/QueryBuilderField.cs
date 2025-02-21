using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    public class QueryBuilderField : BaseObjectInt64
    {
        public string Sequence { get; set; }
        public long QueryBuilderID { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}
