using BackStage.Dto;
using BackStage.Enum;
using Microsoft.Data.Sqlite;
using System.Data;

namespace BackStage.Service
{
    /// <summary>
    /// 資料庫服務
    /// </summary>
    public class DbService
    {
        private readonly SqliteConnection _con;
        private IConfiguration _configuration;

        public DbService(SqliteConnection con)
        {
            _con = con;
            InitDailyReserve();
            InitBlackList();
        }

        /// <summary>建立資料庫連線</summary>
        private SqliteConnection Open()
        {

            if (_con.State == ConnectionState.Open) _con.Close();
            _con.Open();

            return _con;
        }

        /// <summary>
        /// 建立DailyReserve資料表
        /// </summary>
        private void InitDailyReserve()
        {
            try
            {
                using (_con)
                {
                    Open();

                    // 建立 DailyReserve 資料表
                    string sql = @"CREATE TABLE IF NOT EXISTS DailyReserve (
                                        [ID] INTEGER PRIMARY KEY,
                                        [Time] TEXT,
                                        [TakeWay] INT,
                                        [Phone] TEXT,
                                        [People] TEXT,
                                        [Order] INT,
                                        [TableSize] TEXT,
                                        [QueueNumber] INT,
                                        [Flag] int
                                        )";
                    using (var command = new SqliteCommand(sql, _con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                _con.Dispose();
                Console.WriteLine(ex.ToString());
            }
        }

        private void InitBlackList()
        {
            try
            {
                using (_con)
                {
                    Open();

                    // 建立 Customer 資料表
                    string sql = @"CREATE TABLE IF NOT EXISTS BlackList (
                                        [ID] INTEGER PRIMARY KEY,
                                        [Phone] TEXT,
                                        [Cancel] INT,
                                        [Block] INT,
                                        )";
                    using (var command = new SqliteCommand(sql, _con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                _con.Dispose();
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 取得預約紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<List<DailyReserveDto>> GetDailyReserve(DailyReportDto dto)
        {
            List<DailyReserveDto> dailyReserves = new List<DailyReserveDto>();

            try
            {
                using (_con)
                {
                    Open();

                    string sql = @"SELECT * FROM DailyReserve 
                                   WHERE Time >= @StartTime AND Time <= @EndTime";

                    using (var command = new SqliteCommand(sql, _con))
                    {
                        // 設定參數值
                        command.Parameters.AddWithValue("@StartTime", dto.startTime);
                        command.Parameters.AddWithValue("@EndTime", dto.endTime);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                dailyReserves.Add(new DailyReserveDto
                                {
                                    number = reader.GetInt32(reader.GetOrdinal("QueueNumber")),
                                    time = DateTime.Parse(reader.GetString(reader.GetOrdinal("Time"))),
                                    takeWay = (TakeWayEnum)reader.GetInt32(reader.GetOrdinal("TakeWay")),
                                    phone = reader.GetInt32(reader.GetOrdinal("Phone")),
                                    people = reader.GetInt32(reader.GetOrdinal("People")),
                                    order = reader.GetInt32(reader.GetOrdinal("Order")),
                                    tableSize = (TableSizeEnum)reader.GetInt32(reader.GetOrdinal("TableSize"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _con.Dispose();
                Console.WriteLine(ex.ToString());
            }

            return dailyReserves;
        }
        /// <summary>
        /// 新增黑名單
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task AddBlackList(AddBlackListDto dto)
        {
            try
            {
                using (_con)
                {
                    Open();

                    string insertSql = @"INSERT INTO BlackList ([Phone],[Cancel],[Block]) 
                                            VALUES (@Phone,@Cancel,@Block)";
                    using (var command = new SqliteCommand(insertSql, _con))
                    {
                        // 設定參數值
                        command.Parameters.AddWithValue("@Phone", dto.phone);
                        command.Parameters.AddWithValue("@Cancel", 0);
                        command.Parameters.AddWithValue("@Block", 0);
                        await command.ExecuteNonQueryAsync();
                    }

                }
            }
            catch (Exception ex)
            {
                _con.Dispose();
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task DeleteBlackList(DeleteBlackListDto dto)
        {
            try
            {
                using (_con)
                {
                    Open();

                    string deleteSql = @"DELETE BlackList WHERE [Phone] = @Phone";

                    using (var command = new SqliteCommand(deleteSql, _con))
                    {
                        // 設定參數值
                        command.Parameters.AddWithValue("@Phone", dto.phone);
                        await command.ExecuteNonQueryAsync();
                    }

                }
            }
            catch (Exception ex)
            {
                _con.Dispose();
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
