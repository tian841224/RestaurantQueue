using FrontStage.Enum;
using FrontStage.Extensions;

namespace FrontStage.Dto
{
    public class APIResult
    {
        /// <summary>
        /// 提示文字
        /// </summary>
        public string? SuccesDesc => IsSuccess.GetDescription();

        /// <summary>
        /// 是否成功
        /// </summary>
        public SuccessTypeEnum IsSuccess { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string? ErrorMsg { get; set; } = string.Empty;
    }
}
