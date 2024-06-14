using FrontStage.Dto;
using FrontStage.Enum;
using StackExchange.Redis;
using System;
using System.Text.Json;

namespace FrontStage.Service
{
    public class RedisService
    {

        private readonly IConfiguration _configuration;
        private static Lazy<ConnectionMultiplexer> _connection;
        private static IDatabase _redisDb;
        private readonly string _lockKey = "queueLockKey";
        private readonly TimeSpan _expiry = new TimeSpan(0, 0, 30);//設定失效時間為30秒
        private readonly string date = DateTime.UtcNow.Month.ToString() + DateTime.UtcNow.Day.ToString();
        private readonly string redisKey ;

        public RedisService(IConfiguration configuration)
        {
            redisKey = $"queueList_{date}";
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
            _redisDb = Connection.GetDatabase(db);
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
        /// 取得最後號碼
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> GetLastNumber()
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
        /// 取得客人資訊
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<CustomerDto> GetCustomer(GetCustomerDto dto)
        {
            var key = $"{dto.tableSize}_{redisKey}";

            if (await _redisDb.KeyExistsAsync(key))
            {
                var element = (await _redisDb.SortedSetRangeByScoreAsync($"{key}", dto.number, dto.number)).FirstOrDefault();
                var customer = JsonSerializer.Deserialize<CustomerDto>(element);
                return customer;
            }

            return new CustomerDto { };
        }

        /// <summary>
        /// 取得下一位入座客人
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<QueueList> GetAndRemoveNextNumber(GetNextNumberDto dto)
        {
            var key = $"{dto.tableSize}_{redisKey}";
            //取第一筆
            var entry = await _redisDb.SortedSetRangeByRankAsync(key, 0, 0);
            //判斷是否有值
            if (entry.Length > 0)
            {
                var result = JsonSerializer.Deserialize<QueueList>(entry[0]);
                // 移除 SortedSet 中的第一個元素
                await _redisDb.SortedSetRemoveAsync(key, entry);
                return result;
            }

            return new QueueList { };
        }

        /// <summary>
        /// 移除號碼
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task RemoveNumber(RemoveNumberDto dto)
        {
            var key = $"{dto.tableSize}_{redisKey}";

            if (await _redisDb.KeyExistsAsync(key))
            {
                var element = (await _redisDb.SortedSetRangeByScoreAsync($"{key}", dto.number, dto.number)).FirstOrDefault();
                await _redisDb.SortedSetRemoveAsync($"key", element);
            }
        }
        /// <summary>
        /// 加入陣列
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task AddQueue(TackNumberDto dto)
        {
            var tableSize = dto.people >= 5 ? TableSizeEnum.Big :
                            dto.people >= 3 && dto.people < 5 ? TableSizeEnum.Medium :
                            TableSizeEnum.Small;

            //將參數轉為json
            string customerValue = JsonSerializer.Serialize(new QueueList
            {
                number = dto.number.Value,
                time = dto.time,
                takeWay = dto.takeWay,
                phone = dto.phone,
                people = dto.people,
                tableSize = tableSize,
            });

            await _redisDb.SortedSetAddAsync($"{tableSize}_{redisKey}", customerValue, (int)dto.number);
        }

        /// <summary>
        /// 取得顧客順序
        /// </summary>
        public async Task<int> GetCustomerOrder(GetCustomerOrder dto)
        {
            var key = $"{dto.tableSize}_{redisKey}";

            var tableSize = dto.people >= 5 ? TableSizeEnum.Big :
                dto.people >= 3 && dto.people < 5 ? TableSizeEnum.Medium :
                TableSizeEnum.Small;

            return (await _redisDb.SortedSetRangeByScoreAsync(key, 0, dto.number)).Count();
        }

        /// <summary>
        /// 取得各桌位需等待人數
        /// </summary>
        public async Task<WaitCountResponseDto> GetWaitCount()
        {
            var result = new WaitCountResponseDto();
            // 取得所有 TableSizeEnum 的值
            Array enumValues = System.Enum.GetValues(typeof(TableSizeEnum));

            // 列出所有的值
            foreach (TableSizeEnum value in enumValues)
            {
                var number = (int)(await _redisDb.SortedSetRangeByRankWithScoresAsync($"{value.ToString()}_{redisKey}", 0, 0))[0].Score;
                switch (value)
                {
                    case TableSizeEnum.Big:
                        result.Big = number;
                        break;

                    case TableSizeEnum.Medium:
                        result.Medium = number;
                        break;

                    case TableSizeEnum.Small:
                        result.Small = number;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// 取得顧客需等待人數
        /// </summary>
        public async Task<int> GetWaitCount(GetWaitCountDto dto)
        {
            var key = $"{dto.tableSize}_{redisKey}";
            var score = 0;
            var smallestEntryWithScore = await _redisDb.SortedSetRangeByRankWithScoresAsync(key, 0, 0);

            if (smallestEntryWithScore.Length > 0)
            {
                score = (int)smallestEntryWithScore[0].Score;
            }

            return score;
        }
    }
}