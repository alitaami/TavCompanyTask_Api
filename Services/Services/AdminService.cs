using Common.Resources;
using Data.Repositories;
using Entities.Base;
using Entities.Models;
using Entities.Models.Roles;
using Entities.Models.User;
using Microsoft.Extensions.Logging;
using Services.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class AdminService : ServiceBase<AdminService>, IAdminService
    {
        private IRepository<User> _repo;
        //private IRepository<NewsReceiver> _repoNR;
        private IRepository<UserRoles> _repoUR;
        private IRepository<Role> _repoR;
        private IUserService _userService;
        private IBackgroundJobsService _backgroundJobsService;
        private IMemoryCachService _cache;
        public AdminService(IMemoryCachService memoryCach, IBackgroundJobsService backgroundJobsService, IUserService userService, ILogger<AdminService> logger,/* IRepository<NewsReceiver> repoNR,*/ IRepository<User> repository, IRepository<UserRoles> repoUR, IRepository<Role> repoR) : base(logger)
        {
            _cache = memoryCach;
            _backgroundJobsService = backgroundJobsService;
            _repo = repository;
            _repoUR = repoUR;
            _repoR = repoR;
            //_repoNR = repoNR;
            _userService = userService;
        }

        public async Task<ServiceResult> GetEmailRecordsList()
        {
            try
            {
                var key = Resource.CacheKeyOfReceivers;
                var users = _cache.GetReceiversFromCache(key);
                var records = new List<EmailRecord>();

                // checking => are there any related data in cache?
                if (users.Result is null)
                {
                    var res = await _userService.GetEmailRecords();

                    // adding them to cache
                    await _cache.AddReceiversToCache(res);

                    if (res is null)
                        return NotFound(ErrorCodeEnum.NotFound, Resource.EmailRecordNull, null);///

                    return Ok(res);
                }
                else
                {
                    foreach (var id in users.Result)
                    {
                        int emailRecordId = int.Parse(id);
                        var res = _userService.GetEmailRecordById(emailRecordId);

                        if (res is null)
                            return NotFound(ErrorCodeEnum.NotFound, Resource.EmailRecordNull, null);///

                        records.Add(res.Result);
                    }
                    return Ok(records);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }

        /* public async Task<ServiceResult> AddUsersToReceiverList(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var check = _repoNR.TableNoTracking.Where(x => x.UserId == userId);
                var check1 = _repo.TableNoTracking.Where(x => x.Id == userId);

                if (check.Any())
                    return BadRequest(ErrorCodeEnum.DuplicateError, Resource.IsExists, null);

                if (!check1.Any())
                    return BadRequest(ErrorCodeEnum.DuplicateError, Resource.NotFound, null);

                var user = _userService.GetUserByUserId(userId);

                if (user is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                var newsReceiver = new NewsReceiver()
                {
                    Email = user.Result.Email,
                    UserId = userId
                };
                await _repoNR.AddAsync(newsReceiver, cancellationToken);

                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        } */
        public async Task<ServiceResult> SendEmailInBackground(string subject, string body, List<int> receptionists, CancellationToken cancellationToken)
        {
            try
            {
                //var users = _userService.GetUsers();

                if (receptionists.Count == 0)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.NewsReceiverError, null);///

                await _backgroundJobsService.SendEmailsToUsersInBackground(subject, body, receptionists, cancellationToken);

                return Ok(Resource.EmailsSent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
    }
}
