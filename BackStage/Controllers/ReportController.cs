using BackStage.Dto;
using BackStage.Service;
using Microsoft.AspNetCore.Mvc;

namespace BackStage.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReportController : ControllerBase
    {
        private readonly DbService _dbService;

        public ReportController(DbService dbService)
        {
            _dbService = dbService;
        }

        /// <summary>
        /// 取得每日預約紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<DailyReserveDto>> DailyReport(DailyReportDto dto)
        {
            return await _dbService.GetDailyReserve(dto);
        }
    }
}
