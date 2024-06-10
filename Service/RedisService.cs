using RestaurantQueue.Dto;
using RestaurantQueue.Enum;
using StackExchange.Redis;
using System.Text.Json;

namespace RestaurantQueue.Service
{
    public class RedisService
    {

        private readonly IConfiguration _configuration;
        private static Lazy<ConnectionMultiplexer> _connection;
        private static IDatabase _redisDb;
        private readonly string _lockKey = "queueLockKey";
        private readonly TimeSpan _expiry = new TimeSpan(0, 0, 30);//設定失效時間為30秒
        private readonly string redisKey = "queueList";

        public RedisService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis"));
            });
        }

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return _connection.Value;
            }
        }

        /// <summary>
        /// 選擇連結DB
        /// </summary>
        /// <param name="db"></param>
        public void GetDatabase(int db = -1)
        {
            _redisDb  = Connection.GetDatabase(db);
        }

        /// <summary>
        /// Redis上鎖
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AcquireLockAsync()
        {
            return await _redisDb.StringSetAsync(_lockKey, redisKey, _expiry, When.NotExists);
        }

        /// <summary>
        /// Redis釋放鎖
        /// </summary>
        /// <returns></returns>
        public async Task ReleaseLockAsync()
        {
            await _redisDb.KeyDeleteAsync(_lockKey);
        }

        /// <summary>
        /// 取得號碼
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> GetNumber()
        {
            int number = 1;

            if (await _redisDb.KeyExistsAsync(redisKey))
            {
                // 取得最後一筆號碼
                var entries = await _redisDb.SortedSetRangeByRankWithScoresAsync(redisKey, -1, -1, Order.Descending);
                if (entries.Length > 0)
                {
                    var entry = entries[0];
                    number = (int)entry.Score + 1;
                }
            }
            return number;
        }



        /// <summary>
        /// 移除號碼
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> RemoveNumber()
        {
            int number = 1;

            if (await _redisDb.KeyExistsAsync(redisKey))
            {
                // 取得最後一筆號碼
                var entries = await _redisDb.SortedSetRangeByRankWithScoresAsync(redisKey, -1, -1, Order.Descending);
                if (entries.Length > 0)
                {
                    var entry = entries[0];
                    number = (int)entry.Score + 1;
                }
            }
            return number;
        }

        /// <summary>
        /// 加入陣列
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task AddQueue(TackNumberDto dto)
        {
            //將參數轉為json
            string customerValue = JsonSerializer.Serialize(new QueueList
            {
                number = dto.number.Value,
                time = dto.time,
                takeWay = dto.takeWay,
                phone = dto.phone,
                people = dto.people,
                tableSize =
                    dto.people >= 5 ? TableSizeEnum.Big :
                    dto.people >= 3 && dto.people < 5 ? TableSizeEnum.Medium :
                    TableSizeEnum.Small
            });

            await _redisDb.SortedSetAddAsync(redisKey, customerValue, (int)dto.number);
        }
    }

}
