using FrontStage.Dto;

namespace FrontStage.Service
{
    public class QueueService
    {
        private RedisService _redisService;

        public QueueService(RedisService redisService)
        {
            _redisService = redisService;
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

        public async Task<int> GetOrder(GetOrderDto dto)
        {
            return await _redisService.GetOrder(dto);
        }

        public async Task ConsumeNumber(ConsumeNumberDto dto)
        {
            //取下一位客人，並從Redis移除
            var customer = await _redisService.GetAndRemoveNextNumber(new GetNextNumberDto { tableSize = dto.tableSize});
            //將顧客資訊存進db儲存

        }
    }
}
