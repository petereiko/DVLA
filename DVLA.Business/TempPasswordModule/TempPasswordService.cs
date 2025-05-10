using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DVLA.Business.TempPasswordModule
{
    public class TempPasswordService : ITempPasswordService
    {
        private readonly DVLADbContext _context;
        private readonly ILogger<TempPasswordService> _logger;
        private readonly IUserService _userService;

        public TempPasswordService(DVLADbContext context, ILogger<TempPasswordService> logger, IUserService userService)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        public async Task<MessageResponse> Create(TempPasswordDto model)
        {
            MessageResponse response = new();
            try
            {
                response = await _userService.AdminResetPasswordAsync(model.Password, model.UserId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                response.Message = e.Message;
            }
            return response;
        }
    }
}
