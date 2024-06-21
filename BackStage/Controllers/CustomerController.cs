using BackStage.Dto;
using BackStage.Service;
using Microsoft.AspNetCore.Mvc;

namespace BackStage.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// 新增黑名單
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task AddBlackList(AddBlackListDto dto)
        {
             await _customerService.AddBlackList(dto);
        }

        [HttpPost]
        public async Task DeleteBlackList(DeleteBlackListDto dto)
        {
            await _customerService.DeleteBlackList(dto);
        }
    }
}
