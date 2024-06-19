using BackStage.Dto;
using BackStage.Service;
using Microsoft.AspNetCore.Mvc;

namespace BackStage.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CustomerController : ControllerBase
    {
        private readonly DbService _dbService;

        public CustomerController(DbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public async Task AddBlackList(AddBlackListDto dto)
        {
             await _dbService.AddBlackList(dto);
        }
    }
}
