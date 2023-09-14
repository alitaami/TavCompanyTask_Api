using Common.Resources;
using Data.Repositories;
using Entities.Base;
using Entities.Models.Roles;
using Entities.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class UserService : ServiceBase<UserService>, IUserService
    {

        private IRepository<User> _repo;
        private IRepository<UserRoles> _repoUR;
        private IRepository<Role> _repoR;
        private IRepository<NewsReceiver> _repoNR;
        private readonly IJwtService _jwtService;
        public UserService(IRepository<NewsReceiver> repoNR, ILogger<UserService> logger, IRepository<User> repository, IJwtService jwtService, IRepository<UserRoles> repoUR, IRepository<Role> repoR) : base(logger)
        {
            _repo = repository;
            _repoUR = repoUR;
            _repoR = repoR;
            _jwtService = jwtService;
            _repoNR = repoNR;
        }

        public async Task<User> GetUserByUserId(int id)
        {
            try
            {
                var user = await _repo.TableNoTracking
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (user != null)
                    return user;

                else
                    throw new Exception(Resource.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                throw new Exception(Resource.GeneralErrorTryAgain);
            }
        }

        public async Task<List<NewsReceiver>> GetNewsUsers()
        {
            try
            {
                var users = _repoNR.TableNoTracking.ToList();

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                throw new Exception(Resource.GeneralErrorTryAgain);
            }
        }
    }

}
