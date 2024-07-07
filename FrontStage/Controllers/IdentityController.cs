using FrontStage.Dto;
using FrontStage.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FrontStage.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IdentityController : ControllerBase
    {
        private readonly IdentityService _identityService;

        public IdentityController(IdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpGet]
        public IActionResult Login(string role)
        {
            try
            {
                return Ok(_identityService.GenerateToken(role));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
