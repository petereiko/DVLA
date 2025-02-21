using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.OptometristFirmModule
{
    public interface IOptometristService
    {
        Task<PaginationResponseModel<List<OptometristFirmViewModel>>> GetAllOptometricFirms(PaginationRequestModel model);
        Task<OptometristFirmViewModel> GetOptometricFirm(int id);
        Task<MessageResponse> CreateOptometricFirm(OptometristFirmViewModel model);
        Task<MessageResponse> UpdateOptometricFirm(OptometristFirmViewModel model);
        Task<MessageResponse> ChangeStatus(int optometristFirmId);
    }
}
