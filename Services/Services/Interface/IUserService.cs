using Entities.Base;
using Entities.Models;
using Entities.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Interface
{
    public interface IUserService
    {
        public Task<User> GetUserByUserId(int id);
        public Task<EmailRecord> GetEmailRecordById(int id);
        public Task<List<User>> GetUsers();
        public Task<List<string>> GetEmailRecordsId();
        public Task<List<EmailRecord>> GetEmailRecords();
    }
}
