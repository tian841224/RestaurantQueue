using RestaurantQueue.Dto;
using RestaurantQueue.Enum;

namespace RestaurantQueue.Service
{
    public class QueueService
    {
        private RedisService _redisService;

        public QueueService(RedisService redisService) 
        {
            _redisService = redisService;
        }

        public async Task<int> SpotTakeNumber(SpotTackNumberDto dto)
        {
            //設定redis
            _redisService.GetDatabase();
            //redis-上鎖
            await _redisService.AcquireLockAsync();
            //redis-抽號碼
            var number = await _redisService.GetNumber();
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

            #region 傳送簡訊
            #endregion

            return number;
        }
    }
}
