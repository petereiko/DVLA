using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.BaseFolder
{
    public interface IBaseOjectWithoutAuth<T>
    {
        T Id { get; set; }
        DateTime CreatedDate { get; set; }
        bool IsActive { get; set; }
        bool IsDeleted { get; set; }

        [Timestamp]
        byte[] RowVersion { get; set; }
    }
}
