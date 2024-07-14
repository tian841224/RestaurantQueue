using CommonLibrary.Dto;
using CommonLibrary.Enum;
using CommonLibrary.Interface;
using CommonLibrary.Service;

namespace FrontStage.Service
{
    public class QueueService
    {
        private readonly RedisService _redisService;
        private readonly IDbService _dbService;
        private readonly ILogger<QueueService> _log;

        public QueueService(RedisService redisService, IDbService dbService, ILogger<QueueService> log)
        {
            _redisService = redisService;
            _dbService = dbService;
            _log = log;
        }

        public async Task<SpotTakeNumberResponseDto> SpotTakeNumber(SpotTackNumberDto dto)
        {
            try
            {
                //redis-上鎖
                await _redisService.AcquireLockAsync();
                //redis-抽號碼
                var number = await _redisService.GetLastNumber(new GetLastNumberDto { people = dto.people });
                //redis-取得目前順位
                var order = await _redisService.GetCustomerOrder(new GetCustomerOrder
                {
                    tableSize = dto.people >= 5 ? TableSizeEnum.Big :
                                dto.people >= 3 && dto.people < 5 ? TableSizeEnum.Medium :
                                TableSizeEnum.Small,
                    number = number,
                });
                //redis-加入排隊
                await _redisService.AddQueue(new TackNumberDto
                {
                    number = number,
                    ticketTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    takeWay = TakeWay.Spot,
                    phone = dto.phone,
                    people = dto.people,
                    order = order
                });
                //redis-解鎖
                await _redisService.ReleaseLockAsync();

                #region 傳送簡訊
                #endregion

                return new SpotTakeNumberResponseDto
                {
                    number = number,
                    order = order,
                };
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        public async Task<NetTakeNumberResponseDto> NetTakeNumber(NetTakeNumberDto dto)
        {
            try
            {
                #region 傳送驗證碼
                #endregion

                #region 驗證簡訊驗證碼
                #endregion

                //db-搜尋失約紀錄
                var record = await _dbService.GetCancelRecord(new GetCancelRecordDto { phone = dto.phone });
                if (record >= 3)
                {
                    return new NetTakeNumberResponseDto
                    {
                        message = "取號失敗：失約三次，已列入黑名單",
                    };
                }

                var tableSize = dto.people >= 5 ? TableSizeEnum.Big :
                                dto.people >= 3 && dto.people < 5 ? TableSizeEnum.Medium :
                                TableSizeEnum.Small;

                //redis-上鎖
                await _redisService.AcquireLockAsync();
                //redis-抽號碼
                var number = await _redisService.GetLastNumber(new GetLastNumberDto { people = dto.people });
                //redis-取得目前順位
                var order = await _redisService.GetCustomerOrder(new GetCustomerOrder
                {
                    tableSize = tableSize,
                    number = number,
                });
                //redis-加入排隊
                await _redisService.AddQueue(new TackNumberDto
                {
                    number = number,
                    ticketTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    takeWay = TakeWay.Internet,
                    phone = dto.phone,
                    people = dto.people,
                    order = order,
                    tableSize = tableSize
                });
                //redis-解鎖
                await _redisService.ReleaseLockAsync();

                #region 傳送簡訊
                #endregion

                return new NetTakeNumberResponseDto
                {
                    message = "取號成功",
                    number = number,
                    order = order,
                };
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        public async Task<WaitCountResponseDto> GetWaitCount()
        {
            try
            {
                return await _redisService.GetWaitCount();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        public async Task<int> GetWaitCount(GetWaitCountDto dto)
        {
            try
            {
                return await _redisService.GetWaitCount(dto);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        public async Task<int> GetCustomerOrder(GetCustomerOrder dto)
        {
            try
            {
                return await _redisService.GetCustomerOrder(dto);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        public async Task<QueueList> ConsumeNumber(ConsumeNumberDto dto)
        {
            try
            {
                //取下一位客人，並從Redis移除
                var customer = await _redisService.GetAndRemoveNextNumber(new GetNextNumberDto { tableSize = dto.tableSize });

                var tableSize = customer.people >= 5 ? TableSizeEnum.Big :
                     customer.people >= 3 && customer.people < 5 ? TableSizeEnum.Medium :
                     TableSizeEnum.Small;

                //db-儲存預約紀錄
                await _dbService.AddDailyReserve(new AddDailyReserveDto
                {
                    queueNumber = customer.queueNumber,
                    ticketTime = customer.ticketTime,
                    seatTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    takeWay = customer.takeWay,
                    phone = customer.phone,
                    people = customer.people,
                    order = customer.order,
                    tableSize = tableSize,
                    flag = FlagEnum.Finish,
                });

                return customer;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        public async Task CancelReserve(CancelReserveDto dto)
        {
            try
            {
                //redis-取得客人資訊
                var customer = await _redisService.GetCustomerByPhone(dto.phone);

                if (customer.phone == 0)
                {
                    return;
                }

                //redis-移除排隊號碼
                await _redisService.RemoveNumber(new RemoveNumberDto
                {
                    tableSize = customer.tableSize,
                    number = customer.queueNumber,
                });

                //db-紀錄失約
                await _dbService.AddCancelReserve(new AddCancelReserveDto
                {
                    phone = dto.phone,
                });

                //db-儲存預約紀錄
                await _dbService.AddDailyReserve(new AddDailyReserveDto
                {
                    queueNumber = customer.queueNumber,
                    ticketTime = customer.ticketTime,
                    takeWay = customer.takeWay,
                    tableSize = customer.people >= 5 ? TableSizeEnum.Big :
                                customer.people >= 3 && customer.people < 5 ? TableSizeEnum.Medium :
                                TableSizeEnum.Small,
                    phone = customer.phone,
                    people = customer.people,
                    order = customer.order,
                    flag = FlagEnum.Cancel,
                });
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }
    }
}
