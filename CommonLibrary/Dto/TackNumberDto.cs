using CommonLibrary.Enum;

namespace CommonLibrary.Dto
{
    /// <summary>
    /// 現場取號DTO
    /// </summary>
    public class TackNumberDto
    {
        /// <summary>
        /// 取號號碼
        /// </summary>
        public int? number { get; set; }

        /// <summary>
        /// 取號時間
        /// </summary>
        public string ticketTime { get; set; }


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
        /// 目前順位
        /// </summary>
        public int order { get; set; }

        /// <summary>
        /// 桌次
        /// </summary>
        public TableSizeEnum tableSize { get; set; }
    }
}
