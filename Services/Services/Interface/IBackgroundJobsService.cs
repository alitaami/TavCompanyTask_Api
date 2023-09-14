using Common.Utilities;
using Entities.Base;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Interface
{
    public interface IBackgroundJobsService
    {
        public Task<ServiceResult> SendEmailsToUsersInBackground(string subject, string body,CancellationToken cancellationToken);
        public Task SendEmailAsync(string email, string subject, string body);

    }
}
