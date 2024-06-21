using BackStage.Dto;
using BackStage.Service;
using Microsoft.AspNetCore.Mvc;

namespace BackStage.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// 取得每日預約紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DailyReserveDto> DailyReport(DailyReportDto dto)
        {
            return await _reportService.DailyReport(dto);
        }

    }
}
