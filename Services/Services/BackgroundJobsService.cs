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
        private readonly SendMail _sendMail;

        public BackgroundJobsService(SendMail sendMail,IRepository<EmailRecord> repoEmail, ILogger<BackgroundJobsService> logger, IUserService userService) : base(logger)
        {
            _sendMail  = sendMail;
            _userService = userService;
            _RepoEmail = repoEmail;
        }

        public async Task<ServiceResult> SendEmailsToUsersInBackground(string subject, string body, List<int> receptionists, CancellationToken cancellationToken)
        {
            try
            {
                // Create a dictionary to store user IDs and their corresponding emails.
                Dictionary<int, string> usersData = new Dictionary<int, string>();

                // Iterate through the list of receptionists.
                foreach (int userId in receptionists)
                {
                    // Check if the user's email is already in the dictionary.
                    if (!usersData.ContainsKey(userId))
                    {
                        // Retrieve the user's email by their ID.
                        var user = await _userService.GetUserByUserId(userId);

                        if (user != null)
                        {
                            // User exists, add their email to the dictionary.
                            usersData.Add(userId, user.Email);
                        }
                    }
                }

                // Create a list of email records based on the user emails.
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

                // Enqueue the email sending tasks for all users.
                foreach (var userEmail in usersData.Values)
                {
                    BackgroundJob.Enqueue(() => SendEmailAsync(userEmail, subject, body));
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
            try
            {
                await _sendMail.SendAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                throw new Exception(Resource.GeneralErrorTryAgain);
            }
        }
    }
}
