using FrontStage.Dto;
using Microsoft.Data.Sqlite;
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
            InitDailyReserve();
            InitBlackList();
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
                                        [TicketTime] TEXT,
                                        [SeatTime] TEXT,
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

        /// <summary>
        /// 建立Reserve資料表
        /// </summary>
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
                                        [Block] INT
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

        /// <summary>建立資料庫連線</summary>
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
            int result = 0;

            try
            {
                using (_con)
                {
                    Open();
                    string sql = @"INSERT INTO DailyReserve ([TicketTime],[SeatTime], [TakeWay], [Phone], [People], [QueueNumber], [Order], [TableSize], [Flag]) 
                                       VALUES (@TicketTime,@SeatTime, @TakeWay, @Phone, @People, @QueueNumber, @Order, @TableSize, @Flag)";
                    using (var command = new SqliteCommand(sql, _con))
                    {
                        // 設定參數值
                        command.Parameters.AddWithValue("@QueueNumber", dto.number);
                        command.Parameters.AddWithValue("@TicketTime", dto.ticketTime);
                        command.Parameters.AddWithValue("@SeatTime", dto.seatTime);
                        command.Parameters.AddWithValue("@TakeWay", dto.takeWay);
                        command.Parameters.AddWithValue("@Phone", dto.phone);
                        command.Parameters.AddWithValue("@People", dto.people);
                        command.Parameters.AddWithValue("@Order", dto.order);
                        command.Parameters.AddWithValue("@TableSize", dto.tableSize);
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
            try
            {
                using (_con)
                {
                    Open();

                    //搜尋失約紀錄
                    string selectSql = @"SELECT [Cancel] From BlackList WHERE [Phone] = @Phone";
                    using (var command = new SqliteCommand(selectSql, _con))
                    {
                        // 設定參數值
                        command.Parameters.AddWithValue("@Phone", dto.phone);

                       var cancel = await command.ExecuteScalarAsync();

                        //若沒紀錄則新增
                        if (cancel == null)
                        {
                            string insertSql = @"INSERT INTO BlackList ([Phone],[Cancel],[Block]) 
                                            VALUES (@Phone,@Cancel,@Block)";
                            using (var com = new SqliteCommand(insertSql, _con))
                            {
                                // 設定參數值
                                com.Parameters.AddWithValue("@Phone", dto.phone);
                                com.Parameters.AddWithValue("@Cancel", 0);
                                com.Parameters.AddWithValue("@Block", 0);
                                await com.ExecuteNonQueryAsync();
                            }
                        }
                        else
                        {
                            string updateSql = @"UPDATE BlackList SET [Cancel] = @Cancel , [Block] = @Block
                                                 WHERE [Phone] = @Phone ";
                            using (var com = new SqliteCommand(updateSql, _con))
                            {
                                // 設定參數值
                                com.Parameters.AddWithValue("@Phone", dto.phone);
                                com.Parameters.AddWithValue("@Cancel", (Int64)cancel + 1);
                                com.Parameters.AddWithValue("@Block", (Int64)cancel + 1 == 3 ? 1 : 0);
                                await com.ExecuteNonQueryAsync();
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
        }

        /// <summary>
        /// 查詢取消紀錄
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<long> GetCancelRecord(GetCancelRecordDto dto)
        {
            try
            {
                using (_con)
                {
                    Open();

                    //搜尋失約紀錄
                    string selectSql = @"SELECT [Cancel] From BlackList WHERE [Phone] = @Phone";
                    using (var command = new SqliteCommand(selectSql, _con))
                    {
                        // 設定參數值
                        command.Parameters.AddWithValue("@Phone", dto.phone);

                        var count = await command.ExecuteScalarAsync();

                        if (count != null)
                        {
                            return (long)count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _con.Dispose();
                Console.WriteLine(ex.ToString());
            }

            return 0;
        }
    }
}
