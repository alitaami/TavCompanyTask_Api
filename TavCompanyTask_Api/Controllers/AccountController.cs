using Entities.Base;
using Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Services.Services.Interface;
using System.Net;
using System.Net.Mime;

namespace TavCompanyTask_Api.Controllers
{
    [AllowAnonymous]
    /// <summary>
    /// Controller for Login and SignUp operations
    /// </summary>
    public class AccountController : APIControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(ILogger<AccountController> logger, IAccountService accountService)
        {
            _logger = logger;
            _accountService = accountService;
        }
        /// <summary>
        /// Login ( using JWT )
        /// </summary>
        /// <param name="tokenRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns> 
        /// 
        [HttpPost] 
        [ProducesResponseType(typeof(JwtToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromForm] TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            var result = await _accountService.Login(tokenRequest, cancellationToken);
            return APIResponse(result);
        }

        /// <summary>
        /// SignUp users
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns> 
        /// 
        [HttpPost]
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SignUp([FromBody] UserViewModel user, CancellationToken cancellationToken)
        {
            var result = await _accountService.UserSignUp(user, cancellationToken);

            return APIResponse(result);
        }
    }
}
