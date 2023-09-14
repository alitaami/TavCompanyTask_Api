using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interface;

namespace TavCompanyTask_Api.Controllers
{
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
        public async Task<IActionResult> Login([FromBody] TokenRequest tokenRequest, CancellationToken cancellationToken)
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
        public async Task<IActionResult> SignUp([FromBody] UserViewModel user, CancellationToken cancellationToken)
        {
            var result = await _accountService.UserSignUp(user, cancellationToken);

            return APIResponse(result);
        }
    }
}
