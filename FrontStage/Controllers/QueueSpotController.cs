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
        /// 網路取號
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<NetTakeNumberResponseDto> NetTakeNumber(NetTakeNumberDto dto)
        {
            return await _queueService.NetTakeNumber(dto);
        }

        /// <summary>
        /// 取得目前排隊號碼
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> GetCustomerOrder(GetCustomerOrder dto)
        {
            return await _queueService.GetCustomerOrder(dto);
        }

        /// <summary>
        /// 取得各桌位需等待人數
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<WaitCountResponseDto> GetWaitCount()
        {
            return await _queueService.GetWaitCount();
        }

        /// <summary>
        /// 取得顧客需等待人數
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> GetWaitCount(GetWaitCountDto dto)
        {
            return await _queueService.GetWaitCount(dto);
        }

        /// <summary>
        /// 入座
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<QueueList> ConsumeNumber(ConsumeNumberDto dto)
        {
            return await _queueService.ConsumeNumber(dto);
        }
        
        /// <summary>
        /// 取消預約
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task CancelReserve(CancelReserveDto dto)
        {
           await _queueService.CancelReserve(dto);
        }
    }
}
