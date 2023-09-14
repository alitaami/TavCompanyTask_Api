using Entities.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interface;

namespace TavCompanyTask_Api.Controllers
{
    /// <summary>
    /// Controller for Admin
    /// </summary>
    public class AdminController : APIControllerBase
    {
        private readonly IAdminService _adminService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adminService"></param>
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> AddUsersToReceiverList(int userId, CancellationToken cancellationToken)
        {
            var result = await _adminService.AddUsersToReceiverList(userId, cancellationToken);
            return APIResponse(result);
        }

        public async Task<IActionResult> SendEmailForReceivers([FromBody] EmailRequest request)
        {
            var result = await _adminService.SendEmailInBackground(request.Subject, request.Body);
            return APIResponse(result);
        }

    }
}
