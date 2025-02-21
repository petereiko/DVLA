using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.BaseFolder
{
    public class BaseObjectInt32WithoutAuth : IBaseOjectWithoutAuth<Int32>
    {
        public int Id { get ; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
