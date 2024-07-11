using CommonLibrary.Dto;
using CommonLibrary.Service;
using FrontStage.Service;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SpotTakeNumber(SpotTackNumberDto dto)
        {
            try
            {
                return Ok(await _queueService.SpotTakeNumber(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 網路取號
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> NetTakeNumber(NetTakeNumberDto dto)
        {
            try
            {
                return Ok(await _queueService.NetTakeNumber(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 取得目前排隊號碼
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetCustomerOrder(GetCustomerOrder dto)
        {
            try
            {
                return Ok(await _queueService.GetCustomerOrder(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 取得各桌位需等待人數
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetWaitCount()
        {
            try
            {
                return Ok(await _queueService.GetWaitCount());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 取得顧客需等待人數
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetWaitCount(GetWaitCountDto dto)
        {
            try
            {
                return Ok(await _queueService.GetWaitCount(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 入座
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConsumeNumber(ConsumeNumberDto dto)
        {
            try
            {
                return Ok(await _queueService.ConsumeNumber(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        /// <summary>
        /// 取消預約
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelReserve(CancelReserveDto dto)
        {
            try
            {
                await _queueService.CancelReserve(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
