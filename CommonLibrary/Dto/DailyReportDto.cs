namespace CommonLibrary.Dto
{
    public class DailyReportDto
    {
        /// <summary>
        /// 起始時間
        /// </summary>
        public DateTime? startTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime? endTime { get; set; } = DateTime.Now.AddDays(1);
    }
}
