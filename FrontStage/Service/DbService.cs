using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Entity;

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
        /// 初始化資料庫
        /// </summary>
        public void Init()
        {
            var sql = @"CREATE TABLE IF NOT EXISTS Customer (
                        ID INTEGER PRIMARY KEY,
                        Time TEXT,
                        TakeWay INT,
                        Phone TEXT,
                        People TEXT,
                        QueueNumber INT
                        )"
            ;
        }

        /// <summary>建立資料庫連線</summary>
        /// <param name="database">資料庫名稱</param>
        /// <returns></returns>
        public SqliteConnection OpenConnection()
        {

            if (_con.State == ConnectionState.Open) _con.Close();
            _con.Open();

            return _con;
        }
    }
}
