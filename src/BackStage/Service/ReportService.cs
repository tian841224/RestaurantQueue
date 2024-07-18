using CommonLibrary.Dto;
using CommonLibrary.Interface;
using CommonLibrary.Service;
using static CommonLibrary.Dto.DailyReserveDto;

namespace BackStage.Service
{
    public class ReportService
    {
        private readonly IDbService _dbService;
        private IConfiguration _configuration;
        private readonly ILogger<ReportService> _log;

        public ReportService(DbService dbService, IConfiguration configuration, ILogger<ReportService> log)
        {
            _dbService = dbService;
            _configuration = configuration;
            _log = log;
        }

        public async Task<DailyReserveDto> DailyReport(DailyReportDto dto)
        {
            try
            {
                var dailyReserveList = await _dbService.GetDailyReserve(dto);

                //計算平均等待時間
                if (dailyReserveList.DailyReserves.Any())
                {
                    var avgWaitTime = TimeSpan.FromSeconds(dailyReserveList.DailyReserves.Average(d => d.waitTime.TotalSeconds));
                    dailyReserveList.AvgWaitTime = avgWaitTime.ToString(@"hh\:mm\:ss");
                }

                // 讀取餐期設定
                var mealPeriodTime = _configuration.GetSection("MealPeriodTime");
                // 取得餐期時間
                var timeKeys = mealPeriodTime.GetChildren().Select(x => x.Key).ToList();

                foreach (var key in timeKeys)
                {
                    string startTime = mealPeriodTime[$"{key}:Start"];
                    string endTime = mealPeriodTime[$"{key}:End"];

                    // 過濾符合特定時間範圍的資料
                    var recordsInRange = dailyReserveList.DailyReserves
                        .Where(d => d.ticketTime.TimeOfDay >= TimeSpan.Parse(startTime) && d.ticketTime.TimeOfDay < TimeSpan.Parse(endTime))
                        .ToList();

                    if (recordsInRange.Any())
                    {
                        // 計算符合範圍內的等待時間總和
                        var totalWaitTime = TimeSpan.FromSeconds(recordsInRange.Sum(d => d.waitTime.TotalSeconds) / recordsInRange.Count);

                        dailyReserveList.MealPeriod.Add(new MealPeriodTime
                        {
                            Count = recordsInRange.Count,
                            TimeRange = $"{startTime} - {endTime}",
                            AvgWaitTime = totalWaitTime.ToString(@"hh\:mm\:ss")
                        });
                    }
                }

                var recordsOutsideRange = dailyReserveList.DailyReserves
                                        .Where(d => !timeKeys.Any(key =>
                                            d.ticketTime.TimeOfDay >= TimeSpan.Parse(mealPeriodTime[$"{key}:Start"]) &&
                                            d.ticketTime.TimeOfDay < TimeSpan.Parse(mealPeriodTime[$"{key}:End"])))
                                        .ToList();

                if (recordsOutsideRange.Any())
                {
                    var totalWaitTime = TimeSpan.FromSeconds(recordsOutsideRange.Sum(d => d.waitTime.TotalSeconds) / recordsOutsideRange.Count);
                    dailyReserveList.MealPeriod.Add(new MealPeriodTime
                    {
                        Count = recordsOutsideRange.Count,
                        TimeRange = "OutMealPeriod",
                        AvgWaitTime = totalWaitTime.ToString(@"hh\:mm\:ss")
                    });
                }

                return dailyReserveList;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }
    }
}
