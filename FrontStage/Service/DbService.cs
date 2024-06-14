using FrontStage.Dto;
using Microsoft.Data.Sqlite;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace FrontStage.Service
{
    /// <summary>
    /// 資料庫服務
    /// </summary>
    public class DbService
    {
        private readonly Lazy<SqliteConnection> _connection;
        private IConfiguration _configuration;
        public SqliteConnection _con => _connection.Value;

        public DbService() 
        {
            _connection = new Lazy<SqliteConnection>();
        }

        /// <summary>
        /// 建立DailyReserve資料表
        /// </summary>
        private async void InitDailyReserve()
        {
            try
            {
                using (_con)
                {
                    _con.Open();

                    // 建立 DailyReserve 資料表
                    string sql = @"CREATE TABLE IF NOT EXISTS DailyReserve (
                                        ID INTEGER PRIMARY KEY,
                                        Time TEXT,
                                        TakeWay INT,
                                        Phone TEXT,
                                        People TEXT,
                                        QueueNumber INT,
                                        Flag int,
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
        private async void InitCancelReserve()
        {
            try
            {
                using (_con)
                {
                    _con.Open();

                    // 建立 Customer 資料表
                    string sql = @"CREATE TABLE IF NOT EXISTS CancelReserve (
                                        ID INTEGER PRIMARY KEY,
                                        Phone TEXT,
                                        Count INT
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
            InitDailyReserve();
            int result = 0;

            try
            {
                using (_con)
                {
                    _con.Open();
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
               
            return result == 0 ? false : true ;
        }

        /// <summary>
        /// 新增取消紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task CancelReserve(AddCancelReserveDto dto)
        {
            InitCancelReserve();
            int result = 0;
            int cancelCount = 0;

            try
            {
                using (_con)
                {
                    _con.Open();

                    //搜尋失約紀錄
                    string selectSql = @"SELECT COUNT(1) CancelReserve WHERE Phone = @Phone";
                    using (var command = new SqliteCommand(selectSql, _con))
                    {
                        // 設定參數值
                        command.Parameters.AddWithValue("@Phone", dto.phone);

                        cancelCount = (int)await command.ExecuteScalarAsync();
                    }

                    if(cancelCount == 0)
                    {
                        string insertSql = @"INSERT INTO CancelReserve (Phone,Count) 
                                       VALUES (@Phone, 1)";
                        using (var command = new SqliteCommand(insertSql, _con))
                        {
                            // 設定參數值
                            command.Parameters.AddWithValue("@Phone", dto.phone);

                            result = await command.ExecuteNonQueryAsync();
                        }
                    }
                    else
                    {
                        string insertSql = @"UPDATE CancelReserve SET Count = @Count WHERE PHONE = @PHONE";
                        using (var command = new SqliteCommand(insertSql, _con))
                        {
                            // 設定參數值
                            command.Parameters.AddWithValue("@Phone", dto.phone);
                            command.Parameters.AddWithValue("@Count", cancelCount);
                            result = await command.ExecuteNonQueryAsync();
                        }
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
