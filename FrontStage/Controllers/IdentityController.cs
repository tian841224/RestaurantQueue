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
        public IdentityResultDto Login(string role)
        {
            return _identityService.GenerateToken(role);
        }
    }
}
