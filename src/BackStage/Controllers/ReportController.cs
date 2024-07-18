using BackStage.Service;
using CommonLibrary.Dto;
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
        public async Task<IActionResult> DailyReport(DailyReportDto dto)
        {
            try
            {
                return Ok(await _reportService.DailyReport(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
