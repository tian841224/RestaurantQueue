using FrontStage.Dto;
using FrontStage.Enum;
using System.Numerics;

namespace FrontStage.Service
{
    public class QueueService
    {
        private readonly RedisService _redisService;
        private readonly DbService _dbService;

        public QueueService(RedisService redisService, DbService dbService)
        {
            _redisService = redisService;
            _dbService = dbService;
        }

        public async Task<SpotTakeNumberResponseDto> SpotTakeNumber(SpotTackNumberDto dto)
        {
            //設定redis
            _redisService.GetDatabase();
            //redis-上鎖
            await _redisService.AcquireLockAsync();
            //redis-抽號碼
            var number = await _redisService.GetLastNumber();
            //redis-儲存顧客資訊
            await _redisService.AddQueue(new TackNumberDto
            {
                number = number,
                time = DateTime.Now,
                takeWay = dto.takeWay,
                phone = dto.phone,
                people = dto.people,
            });
            //redis-解鎖
            await _redisService.ReleaseLockAsync();
            //redis-取得目前順位
            var order = await _redisService.GetCustomerOrder(new GetCustomerOrder
            {
                people = dto.people,
                number = number,
            });

            #region 傳送簡訊
            #endregion

            return new SpotTakeNumberResponseDto
            {
                number = number,
                order = order,
            };
        }

        public async Task<NetTakeNumberResponseDto> NetTakeNumber(NetTakeNumberDto dto)
        {
            #region 傳送驗證碼
            #endregion

            #region 驗證簡訊驗證碼
            #endregion

            //設定redis
            _redisService.GetDatabase();
            //redis-上鎖
            await _redisService.AcquireLockAsync();
            //redis-抽號碼
            var number = await _redisService.GetLastNumber();
            //redis-儲存顧客資訊
            await _redisService.AddQueue(new TackNumberDto
            {
                number = number,
                time = DateTime.Now,
                takeWay = dto.takeWay,
                phone = dto.phone,
                people = dto.people,
            });
            //redis-解鎖
            await _redisService.ReleaseLockAsync();
            //redis-取得目前順位
            var order = await _redisService.GetCustomerOrder(new GetCustomerOrder
            {
                people = dto.people,
                number = number,
            });

            #region 傳送簡訊
            #endregion

            return new NetTakeNumberResponseDto
            {
                number = number,
                order = order,
            };
        }

        public async Task<WaitCountResponseDto> GetWaitCount()
        {
            return await _redisService.GetWaitCount();
        }

        public async Task<int> GetWaitCount(GetWaitCountDto dto)
        {
            return await _redisService.GetWaitCount(dto);
        }

        public async Task<int> GetCustomerOrder(GetCustomerOrder dto)
        {
            return await _redisService.GetCustomerOrder(dto);
        }

        public async Task<QueueList> ConsumeNumber(ConsumeNumberDto dto)
        {
            //取下一位客人，並從Redis移除
            var customer = await _redisService.GetAndRemoveNextNumber(new GetNextNumberDto { tableSize = dto.tableSize});

            //將顧客資訊存進db儲存
            await _dbService.AddDailyReserve(new AddDailyReserveDto
            {
                number = customer.number,
                time = customer.time,
                takeWay = customer.takeWay,
                phone = customer.phone,
                people = customer.people,
                flag = FlagEnum.Finish,
            });

            return customer;
        }

        public async Task CancelReserve(CancelReserveDto dto)
        {
            //redis-取得客人資訊
            var customer = await _redisService.GetCustomer(new GetCustomerDto {
                tableSize = dto.tableSize,
                number = dto.number });

            //redis-移除排隊號碼
            await _redisService.RemoveNumber(new RemoveNumberDto { 
                tableSize = dto.tableSize,
                number = dto.number,
                phone = dto.phone,
            });

            //db-紀錄失約
            await _dbService.CancelReserve(new AddCancelReserveDto
            {
                phone = dto.phone,
            });

            //db-儲存預約紀錄
            await _dbService.AddDailyReserve(new AddDailyReserveDto
            {
                number = customer.number,
                time = customer.time,
                takeWay = customer.takeWay,
                phone = customer.phone,
                people = customer.people,
                flag = FlagEnum.Cancel,
            });
        }
    }
}
