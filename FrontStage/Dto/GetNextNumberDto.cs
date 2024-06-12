using FrontStage.Enum;

namespace FrontStage.Dto
{
    public class GetNextNumberDto
    {
        /// <summary>
        /// 桌子尺寸 s = 小 , m = 中 , l = 大
        /// </summary>
        public TableSizeEnum tableSize { get; set; }
    }
}
