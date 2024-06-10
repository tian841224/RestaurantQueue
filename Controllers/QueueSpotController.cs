using Microsoft.AspNetCore.Mvc;
using RestaurantQueue.Dto;
using RestaurantQueue.Service;

namespace RestaurantQueue.Controllers
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
        [HttpGet]
        public int SpotTakeNumber(SpotTackNumberDto dto)
        {
            return _queueService.SpotTakeNumber(dto);
        }
    }
}
