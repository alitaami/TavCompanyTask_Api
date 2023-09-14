using Entities.Base;
using Entities.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interface;
using System.Net.Mime;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace TavCompanyTask_Api.Controllers
{
    /// <summary>
    /// Controller for Admin
    /// </summary>
    /// 
    [Authorize(Roles = "1")]
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
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost] 
        [ProducesResponseType(typeof(Ok), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SendEmailForReceivers([FromBody] EmailRequest request,CancellationToken cancellationToken)
        {  
            var result = await _adminService.SendEmailInBackground(request.Subject, request.Body, request.Recipients, cancellationToken);
            return APIResponse(result);
        }

    }
}
