using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;

namespace DVLA.Business.TempPasswordModule
{
    public interface ITempPasswordService
    {
        Task<MessageResponse> Create(TempPasswordDto model);
    }
}
