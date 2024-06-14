using FrontStage.Enum;

namespace FrontStage.Dto
{
    public class AddDailyReserveDto
    {
        /// 取號號碼
        /// </summary>
        public int? number { get; set; }

        /// <summary>
        /// 取號時間
        /// </summary>
        public DateTime time { get; set; }

        /// <summary>
        /// 取號方式
        /// </summary>
        public TakeWay takeWay { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        public int phone { get; set; }

        /// <summary>
        /// 人數
        /// </summary>
        public int people { get; set; }

        /// <summary>
        /// 預約狀態
        /// </summary>
        public FlagEnum flag { get; set; }
    }
}
