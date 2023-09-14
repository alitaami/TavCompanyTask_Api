using Entities.Base;
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
        public Task<List<User>> GetUsers();
    }
}
