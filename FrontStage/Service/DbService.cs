using FrontStage.Dto;
using Microsoft.Data.Sqlite;
using StackExchange.Redis;
using System.Data;

namespace FrontStage.Service
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
        }

        /// <summary>
        /// 建立DailyReserve資料表
        /// </summary>
        private async Task InitDailyReserve()
        {
            try
            {
                using (_con)
                {
                    Open();

                    // 建立 DailyReserve 資料表
                    string sql = @"CREATE TABLE IF NOT EXISTS DailyReserve (
                                        ID INTEGER PRIMARY KEY,
                                        Time TEXT,
                                        TakeWay INT,
                                        Phone TEXT,
                                        People TEXT,
                                        QueueNumber INT,
                                        Flag int
                                        )";
                    using (var command = new SqliteCommand(sql, _con))
                    {
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

        /// <summary>
        /// 建立Reserve資料表
        /// </summary>
        private async Task InitCancelReserve()
        {
            try
            {
                using (_con)
                {
                    Open();

                    // 建立 Customer 資料表
                    string sql = @"CREATE TABLE IF NOT EXISTS CancelReserve (
                                        ID INTEGER PRIMARY KEY,
                                        Time TEXT,
                                        Phone TEXT
                                        )";
                    using (var command = new SqliteCommand(sql, _con))
                    {
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

        /// <summary>建立資料庫連線</summary>
        /// <param name="database">資料庫名稱</param>
        /// <returns></returns>
        private SqliteConnection Open()
        {

            if (_con.State == ConnectionState.Open) _con.Close();
            _con.Open();

            return _con;
        }

        /// <summary>
        /// 新增預約紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> AddDailyReserve(AddDailyReserveDto dto)
        {
            await InitDailyReserve();
            int result = 0;

            try
            {
                using (_con)
                {
                    Open();
                    string sql = @"INSERT INTO DailyReserve (Time, TakeWay, Phone, People, QueueNumber,Flag) 
                                       VALUES (@Time, @TakeWay, @Phone, @People, @QueueNumber, @Flag)";
                    using (var command = new SqliteCommand(sql, _con))
                    {
                        // 設定參數值
                        command.Parameters.AddWithValue("@Time", dto.time);
                        command.Parameters.AddWithValue("@TakeWay", dto.takeWay);
                        command.Parameters.AddWithValue("@Phone", dto.phone);
                        command.Parameters.AddWithValue("@People", dto.people);
                        command.Parameters.AddWithValue("@QueueNumber", dto.number);
                        command.Parameters.AddWithValue("@Flag", dto.flag);
                        result = await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _con.Dispose();
                Console.WriteLine(ex.ToString());
            }

            return result == 0 ? false : true;
        }

        /// <summary>
        /// 新增取消紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task AddCancelReserve(AddCancelReserveDto dto)
        {
            await InitCancelReserve();

            try
            {
                using (_con)
                {
                    Open();

                    string insertSql = @"INSERT INTO CancelReserve (Phone,Time) 
                                            VALUES (@Phone, @Time)";
                    using (var command = new SqliteCommand(insertSql, _con))
                    {
                        // 設定參數值
                        command.Parameters.AddWithValue("@Phone", dto.phone);
                        command.Parameters.AddWithValue("@Time", DateTime.UtcNow);
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

        /// <summary>
        /// 查詢取消紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> GetCancelRecord(GetCancelRecordDto dto)
        {
            await InitCancelReserve();
            int count = 0;

            try
            {
                using (_con)
                {
                    Open();

                    //搜尋失約紀錄
                    string selectSql = @"SELECT COUNT(1) CancelReserve WHERE Phone = @Phone";
                    using (var command = new SqliteCommand(selectSql, _con))
                    {
                        // 設定參數值
                        command.Parameters.AddWithValue("@Phone", dto.phone);

                        count = (int)await command.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _con.Dispose();
                Console.WriteLine(ex.ToString());
            }

            return count;
        }
    }
}
