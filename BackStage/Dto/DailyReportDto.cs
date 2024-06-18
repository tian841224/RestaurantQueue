namespace BackStage.Dto
{
    public class DailyReportDto
    {
        /// <summary>
        /// 起始時間
        /// </summary>
        public DateTime? startTime { get; set; } = DateTime.UtcNow.AddDays(-1);

        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime? endTime { get; set; } = DateTime.UtcNow;
    }
}
