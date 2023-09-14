using Common.Resources;
using Common.Utilities;
using Data.Repositories;
using Entities.Base;
using Entities.Models;
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
        private readonly IRepository<EmailRecord> _RepoEmail;
        public BackgroundJobsService(IRepository<EmailRecord> repoEmail, ILogger<BackgroundJobsService> logger, IUserService userService) : base(logger)
        {
            _userService = userService;
            _RepoEmail = repoEmail;
        }

        public async Task<ServiceResult> SendEmailsToUsersInBackground(string subject, string body, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userService.GetNewsUsers(); // Assuming GetNewsUsers() returns a list of users.

                var emailRecords = users.Select(user => new EmailRecord
                {
                    RecipientEmail = user.Email,
                    Subject = subject,
                    Body = body,
                    SentTime = DateTime.UtcNow, // Record the sent time.
                    SeenTime = null // Initialize seen time as null.
                }).ToList();

                // Use Entity Framework Core to add all email records to the database in a single batch.
                await _RepoEmail.AddRangeAsync(emailRecords, cancellationToken);
               
                var emailSendingTasks = users.Select(user => SendEmailAsync(user.Email, subject, body)).ToList();

                // Now, enqueue the email sending tasks for all users in a single batch.
                foreach (var task in emailSendingTasks)
                {
                    BackgroundJob.Enqueue(() => task);
                }

                // Await the completion of all email sending tasks.
                await Task.WhenAll(emailSendingTasks);
                
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
