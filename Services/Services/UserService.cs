using Common.Resources;
using Data.Repositories;
using Entities.Base;
using Entities.Models;
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
        private IRepository<EmailRecord> _repoER;
        //private IRepository<NewsReceiver> _repoNR;
        private readonly IJwtService _jwtService;
        public UserService(IRepository<EmailRecord> repoER,/*IRepository<NewsReceiver> repoNR,*/ ILogger<UserService> logger, IRepository<User> repository, IJwtService jwtService, IRepository<UserRoles> repoUR, IRepository<Role> repoR) : base(logger)
        {
            _repoER = repoER;
            _repo = repository;
            _repoUR = repoUR;
            _repoR = repoR;
            _jwtService = jwtService;
            //_repoNR = repoNR;
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
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                throw new Exception(Resource.GeneralErrorTryAgain);
            }
        }

        public async Task<List<User>> GetUsers()
        {
            try
            {
                var users = _repo.TableNoTracking.ToList();

                if (users != null)
                    return users;
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                throw new Exception(Resource.GeneralErrorTryAgain);
            }
        }

        public Task<EmailRecord> GetEmailRecordById(int id)
        {
            try
            {
                var user = _repoER.TableNoTracking
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (user is null)
                    return null;

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                throw new Exception(Resource.GeneralErrorTryAgain);
            }
        }

        public async Task<List<string>> GetEmailRecordsId()
        {
            try
            {
                var emailReceivers = _repoER.TableNoTracking.Select(x=>x.Id.ToString()).ToList();

                if (emailReceivers != null)
                    return emailReceivers;
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                throw new Exception(Resource.GeneralErrorTryAgain);
            }
        }

        public async Task<List<EmailRecord>> GetEmailRecords()
        {
            try
            {
                var records = _repoER.TableNoTracking.ToList();

                if (records != null)
                    return records;
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                throw new Exception(Resource.GeneralErrorTryAgain);
            }
        }
    }

}
