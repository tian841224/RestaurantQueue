using FrontStage.Dto;
using FrontStage.Service;
using Microsoft.AspNetCore.Mvc;

namespace FrontStage.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class QueueSpotController : ControllerBase
    {
        private QueueService _queueService;

        public QueueSpotController(RedisService redisService, QueueService queueService)
        {
            _queueService = queueService;
        }

        /// <summary>
        /// 現場取號
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<SpotTakeNumberResponseDto> SpotTakeNumber(SpotTackNumberDto dto)
        {
            return await _queueService.SpotTakeNumber(dto);
        }

        /// <summary>
        /// 取得目前排隊號碼
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> GetOrder(GetOrderDto dto)
        {
            return await _queueService.GetOrder(dto);
        }

        /// <summary>
        /// 入座
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task ConsumeNumber(ConsumeNumberDto dto)
        {

        }
    }
}
