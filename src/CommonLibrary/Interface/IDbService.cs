using CommonLibrary.Dto;

namespace CommonLibrary.Interface
{
    public interface IDbService
    {
        /// <summary>
        /// 新增預約紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<bool> AddDailyReserve(AddDailyReserveDto dto);

        /// <summary>
        /// 新增取消紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task AddCancelReserve(AddCancelReserveDto dto);

        /// <summary>
        /// 查詢取消紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<long> GetCancelRecord(GetCancelRecordDto dto);

        /// <summary>
        /// 取得預約紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<DailyReserveDto> GetDailyReserve(DailyReportDto dto);

        /// <summary>
        /// 新增黑名單
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task AddBlackList(AddBlackListDto dto);

        /// <summary>
        /// 移除黑名單
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task DeleteBlackList(DeleteBlackListDto dto);
    }
}
