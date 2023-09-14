using Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Interface
{
    public interface IAdminService
    {
        //public Task<ServiceResult> AddUsersToReceiverList(int userId, CancellationToken cancellationToken);
        public Task<ServiceResult> SendEmailInBackground(string subject, string body,List<int> receptionists, CancellationToken cancellationToken);

    }
}
