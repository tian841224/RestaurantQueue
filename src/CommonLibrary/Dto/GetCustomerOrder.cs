using CommonLibrary.Enum;

namespace CommonLibrary.Dto
{
    public class GetCustomerOrder
    {
        /// <summary>
        /// 當前號碼
        /// </summary>
        public int number { get; set; }

        /// <summary>
        /// 桌次
        /// </summary>
        public TableSizeEnum tableSize { get; set; }
    }
}
