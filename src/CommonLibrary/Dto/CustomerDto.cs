using CommonLibrary.Enum;

namespace CommonLibrary.Dto
{
    /// <summary>
    /// 顧客資訊
    /// </summary>
    public class CustomerDto
    {
        /// <summary>
        /// 號碼
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 取號號碼
        /// </summary>
        public int number { get; set; }

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
    }
}
