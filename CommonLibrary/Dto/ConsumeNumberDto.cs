
using CommonLibrary.Enum;

namespace CommonLibrary.Dto
{
    /// <summary>
    /// 入座DTO
    /// </summary>
    public class ConsumeNumberDto
    {
        /// <summary>
        /// 桌子尺寸 s = 小 , m = 中 , l = 大
        /// </summary>
        public TableSizeEnum tableSize { get; set; }
    }
}
