using FrontStage.Enum;

namespace FrontStage.Dto
{
    public class GetCustomerDto
    {
        /// <summary>
        /// 桌子尺寸 s = 小 , m = 中 , l = 大
        /// </summary>
        public TableSizeEnum tableSize { get; set; }

        /// <summary>
        /// 號碼
        /// </summary>
        public int number { get; set; }
    }
}
