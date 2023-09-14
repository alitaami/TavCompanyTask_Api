using Common.Resources;
using Common.Utilities;
using Data.Repositories;
using Entities.Base;
using Entities.Models;
using Entities.Models.User;
using Hangfire;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
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
        private readonly SendMail _sendMail;

        public BackgroundJobsService(SendMail sendMail, IRepository<EmailRecord> repoEmail, ILogger<BackgroundJobsService> logger, IUserService userService) : base(logger)
        {
            _sendMail = sendMail;
            _userService = userService;
            _RepoEmail = repoEmail;
        }

        public async Task<ServiceResult> SendEmailsToUsersInBackground(string subject, string body, List<int> receptionists, CancellationToken cancellationToken)
        {
            try
            {
                // Split the list of receptionists into smaller batches.
                var batchSize = 100; // Adjust the batch size as needed.
                var batches = receptionists.Batch(batchSize);

                foreach (var batch in batches)
                {
                    // Create a dictionary to store user IDs and their corresponding emails for this batch.
                    var usersData = new Dictionary<int, string>();

                    // Iterate through the list of receptionists in this batch.
                    foreach (int userId in batch)
                    {
                        if (!usersData.ContainsKey(userId))
                        {
                            var user = await _userService.GetUserByUserId(userId);

                            if (user != null)
                            {
                                usersData.Add(userId, user.Email);
                            }
                        }
                    }

                    // Create email records for this batch.
                    var emailRecords = usersData.Values.Select(email => new EmailRecord
                    {
                        RecipientEmail = email,
                        Subject = subject,
                        Body = body,
                        SentTime = DateTime.UtcNow,
                        SeenTime = null
                    }).ToList();

                    // Add email records to the database.
                    await _RepoEmail.AddRangeAsync(emailRecords, cancellationToken);

                    // Enqueue the email sending tasks for this batch asynchronously.
                    var emailSendingTasks = usersData
                                           .Values.Select(userEmail =>
                                            BackgroundJob.Enqueue(() => SendEmailAsync(userEmail, subject, body)));
                }
                // Return a successful response.
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
            var retryPolicy = CreateRetryPolicy();

            await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    await _sendMail.SendAsync(email, subject, body);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, null, null);

                    throw new Exception(Resource.GeneralErrorTryAgain);
                }
            });
        }
        private AsyncRetryPolicy CreateRetryPolicy()
        {
            return Policy
                .Handle<Exception>() // Specify which exceptions to handle.
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
