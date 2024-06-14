using FrontStage.Enum;

namespace FrontStage.Dto
{
    public class CancelReserveDto
    {
        /// <summary>
        /// 桌子尺寸 s = 小 , m = 中 , l = 大
        /// </summary>
        public TableSizeEnum tableSize { get; set; }

        /// <summary>
        /// 取號號碼
        /// </summary>
        public int number { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        public int phone { get; set; }
    }
}
