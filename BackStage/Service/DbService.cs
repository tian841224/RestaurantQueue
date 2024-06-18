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
        }

        /// <summary>建立資料庫連線</summary>
        private SqliteConnection Open()
        {

            if (_con.State == ConnectionState.Open) _con.Close();
            _con.Open();

            return _con;
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

    }
}
