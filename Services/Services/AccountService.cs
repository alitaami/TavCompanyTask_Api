using Common.Resources;
using Common.Utilities;
using Data.Repositories;
using Entities.Base;
using Entities.Models.Roles;
using Entities.Models.User;
using Entities.ViewModels;
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
    public class AccountService : ServiceBase<AccountService>, IAccountService
    {
        private IRepository<User> _repo;
        private IRepository<UserRoles> _repoUR;
        private IRepository<Role> _repoR;
        private IJwtService _jwtService;
        public AccountService(IJwtService jwtService, ILogger<AccountService> logger, IRepository<User> repository, IRepository<UserRoles> repoUR, IRepository<Role> repoR) : base(logger)
        {
            _repo = repository;
            _repoUR = repoUR;
            _repoR = repoR;
            _jwtService = jwtService;
        }

        public async Task<ServiceResult> Login(TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            try
            {
                if (!tokenRequest.grant_type.Equals("password", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(ErrorCodeEnum.BadRequest, "OAuth flow should be password !!", null);

                // Check user credentials

                var passwordHash = SecurityHelper.GetSha256Hash(tokenRequest.password);

                var result = await _repo.Table
                    .Where(p => p.UserName == tokenRequest.username && p.PasswordHash == passwordHash && p.IsActive)
                    .FirstOrDefaultAsync(cancellationToken);

                if (result is null)
                    return NotFound(ErrorCodeEnum.NotFound, Resource.NotFound, null);///

                if (!result.IsActive)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.UserIsNotActive, null);///

                // Generate JWT token

                var token = await _jwtService.Generate(result);

                return Ok(token);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        public async Task<ServiceResult> UserSignUp(UserViewModel user, CancellationToken cancellationToken)
        {
            try
            {
                var checkDb = _repo.TableNoTracking
                 .Where(u => u.UserName == user.UserName || u.Email == user.Email);

                if (checkDb.Any())
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.AlreadyExists, null);///

                var PasswordHash = SecurityHelper.GetSha256Hash(user.Password);
                var u = new User
                {
                    UserName = user.UserName,
                    PasswordHash = PasswordHash,
                    FullName = user.FullName,
                    Age = user.Age,
                    Email = user.Email
                };

                await _repo.AddAsync(u, cancellationToken);

                if (await CheckRoleExistence(2))
                {
                    var userRole = new UserRoles
                    {
                        RoleId = 2,
                        UserId = u.Id
                    };
                    _repoUR.Add(userRole);
                }

                else
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.RoleNotFound, null);///

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);

            }
        }
        public async Task<bool> CheckRoleExistence(int roleId)
        {
            try
            {
                var exist = _repoR.TableNoTracking.Any(r => r.Id == roleId && !r.IsDelete);

                if (exist)
                    return true;

                else
                    return false;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);

                throw new Exception(Resource.GeneralErrorTryAgain);
            }
        }

    }
}
