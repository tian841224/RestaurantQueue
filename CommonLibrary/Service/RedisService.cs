using CommonLibrary.Dto;
using CommonLibrary.Enum;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace CommonLibrary.Service
{
    public class RedisService
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<RedisService> _log;
        private readonly ConnectionMultiplexer _connection;
        private readonly Lazy<IDatabase> _redisDb;
        private readonly string _lockKey = "queueLockKey";
        private readonly TimeSpan _expiry = new TimeSpan(0, 0, 30);//設定失效時間為30秒
        private readonly string redisKey;
        public IDatabase redisDb => _redisDb.Value;

        public RedisService(IConfiguration configuration, ILogger<RedisService> log)
        {
            redisKey = $"queueList_{DateTime.Now.ToString("MMdd")}";
            _configuration = configuration;
            _connection = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis"));
            _redisDb = new Lazy<IDatabase>(() => _connection.GetDatabase());
            _log = log;
        }



        /// <summary>
        /// Redis上鎖
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AcquireLockAsync()
        {
            try
            {
                return await redisDb.StringSetAsync(_lockKey, redisKey, _expiry, When.NotExists);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Redis釋放鎖
        /// </summary>
        /// <returns></returns>
        public async Task ReleaseLockAsync()
        {
            try
            {
                await redisDb.KeyDeleteAsync(_lockKey);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 取得最後號碼
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> GetLastNumber(GetLastNumberDto dto)
        {
            try
            {

                var tableSize = dto.people >= 5 ? TableSizeEnum.Big :
                    dto.people >= 3 && dto.people < 5 ? TableSizeEnum.Medium :
                    TableSizeEnum.Small;

                var key = $"{redisKey}_{tableSize}";

                int number = 1;

                if (await redisDb.KeyExistsAsync(key))
                {
                    // 取得最後一筆號碼
                    var entries = await redisDb.SortedSetRangeByRankWithScoresAsync(key, -1, -1, Order.Ascending);
                    if (entries.Length > 0)
                    {
                        var entry = entries[0];
                        number = (int)entry.Score + 1;
                    }
                }
                return number;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 取得客人資訊
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<CustomerDto> GetCustomer(GetCustomerDto dto)
        {
            try
            {
                var key = $"{redisKey}_{dto.tableSize}";

                if (await redisDb.KeyExistsAsync(key))
                {
                    var element = (await redisDb.SortedSetRangeByScoreAsync($"{key}", dto.number, dto.number)).FirstOrDefault();
                    var customer = JsonSerializer.Deserialize<CustomerDto>(element);
                    return customer;
                }

                return new CustomerDto { };
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 取得下一位入座客人
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<QueueList> GetAndRemoveNextNumber(GetNextNumberDto dto)
        {
            try
            {
                var key = $"{redisKey}_{dto.tableSize}";

                //取第一筆
                var entry = await redisDb.SortedSetRangeByRankAsync(key, 0, 0);
                //判斷是否有值
                if (entry.Length > 0)
                {
                    var result = JsonSerializer.Deserialize<QueueList>(entry[0]);
                    // 移除 SortedSet 中的第一個元素
                    await redisDb.SortedSetRemoveAsync(key, entry);
                    return result;
                }

                return new QueueList { };
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 移除號碼
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task RemoveNumber(RemoveNumberDto dto)
        {
            try
            {
                var key = $"{redisKey}_{dto.tableSize}";

                if (await redisDb.KeyExistsAsync(key))
                {
                    var element = (await redisDb.SortedSetRangeByScoreAsync($"{key}", dto.number, dto.number)).FirstOrDefault();
                    await redisDb.SortedSetRemoveAsync(key, element);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 加入陣列
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task AddQueue(TackNumberDto dto)
        {
            try
            {
                //將參數轉為json
                string customerValue = JsonSerializer.Serialize(new QueueList
                {
                    queueNumber = dto.number.Value,
                    ticketTime = dto.ticketTime,
                    takeWay = dto.takeWay,
                    phone = dto.phone,
                    people = dto.people,
                    order = dto.order,
                    tableSize = dto.tableSize,
                });

                await redisDb.SortedSetAddAsync($"{redisKey}_{dto.tableSize}", customerValue, (int)dto.number);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 取得顧客順序
        /// </summary>
        public async Task<int> GetCustomerOrder(GetCustomerOrder dto)
        {
            try
            {
                var key = $"{redisKey}_{dto.tableSize}";

                return (await redisDb.SortedSetRangeByScoreAsync(key, double.NegativeInfinity, dto.number)).Count();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 取得各桌位需等待人數
        /// </summary>
        public async Task<WaitCountResponseDto> GetWaitCount()
        {
            try
            {
                var result = new WaitCountResponseDto();
                // 取得所有 TableSizeEnum 的值
                Array enumValues = System.Enum.GetValues(typeof(TableSizeEnum));

                // 列出所有的值
                foreach (TableSizeEnum value in enumValues)
                {
                    var number = (await redisDb.SortedSetRangeByRankWithScoresAsync($"{redisKey}_{value.ToString()}")).Count();
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
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 取得顧客需等待人數
        /// </summary>
        public async Task<int> GetWaitCount(GetWaitCountDto dto)
        {
            try
            {
                var customer = await GetCustomerByPhone(dto.phone);

                var number = await GetCustomerOrder(new GetCustomerOrder
                {
                    number = customer.queueNumber,
                    tableSize = customer.people >= 5 ? TableSizeEnum.Big :
                                customer.people >= 3 && customer.people < 5 ? TableSizeEnum.Medium :
                                TableSizeEnum.Small,
                });


                return number - 1;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        public async Task<QueueList> GetCustomerByPhone(int phone)
        {
            try
            {
                var customer = new QueueList();
                // 取得所有 TableSizeEnum 的值
                Array enumValues = System.Enum.GetValues(typeof(TableSizeEnum));
                List<SortedSetEntry[]> list = new List<SortedSetEntry[]>();

                // 列出所有的值
                foreach (TableSizeEnum value in enumValues)
                {
                    list.Add(await redisDb.SortedSetRangeByRankWithScoresAsync($"{redisKey}_{value.ToString()}"));
                }

                var sortedSetEntry = list.SelectMany(entries => entries)
                                          .Where(entry => entry.Element.ToString().Contains($"{phone}"))
                                          .FirstOrDefault();
                if (sortedSetEntry.Element.HasValue)
                {
                    customer = JsonSerializer.Deserialize<QueueList>(sortedSetEntry.Element);
                }

                return customer;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }
    }
}