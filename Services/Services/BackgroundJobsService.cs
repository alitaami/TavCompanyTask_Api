using Common.Resources;
using Common.Utilities;
using Entities.Base;
using Entities.Models.User;
using Hangfire;
using Microsoft.Extensions.Logging;
using Services.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class BackgroundJobsService : ServiceBase<BackgroundJobsService>, IBackgroundJobsService
    {
        private readonly IUserService _userService;
        public BackgroundJobsService(ILogger<BackgroundJobsService> logger, IUserService userService) : base(logger)
        {
            _userService = userService;
        }

        public async Task<ServiceResult> SendEmailsToUsersInBackground(string subject, string body)
        {
            try
            {
                var users = await _userService.GetNewsUsers(); // Assuming GetNewsUsers() returns a list of users.

                foreach (var user in users)
                {
                    BackgroundJob.Enqueue(() => SendEmailAsync(user.Email, subject, body));
                }

                return Ok(Resource.EmailsSent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            try
            {
                await SendMail.SendAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                throw new Exception(Resource.GeneralErrorTryAgain);
            }
        }
    }
}
