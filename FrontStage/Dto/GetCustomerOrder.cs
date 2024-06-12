using FrontStage.Enum;

namespace FrontStage.Dto
{
    public class GetCustomerOrder
    {
        /// <summary>
        /// 桌子尺寸 s = 小 , m = 中 , l = 大
        /// </summary>
        public TableSizeEnum tableSize { get; set; }

        /// <summary>
        /// 當前號碼
        /// </summary>
        public int number {  get; set; }

        /// <summary>
        /// 人數
        /// </summary>
        public int people { get; set; }
    }
}
