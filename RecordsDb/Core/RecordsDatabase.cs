using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecordsDb.Interfaces;
using System.Data.SQLite;
using System.Data;
using System.Configuration;
using PackMan.Interfaces;

namespace RecordsDb.Core
{
    public class RecordsDatabase: IRecordsDatabase
    {
        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["RecordsDbString"].ConnectionString;
        }

        public void AddRecord(IPlayer player)
        {
            using (SQLiteConnection conn = new SQLiteConnection(GetConnectionString()))
            {
                conn.Open();
                string sql =
                    $"insert into highscores (name, score) values ('{Environment.MachineName}', {player.Score.ToString()})";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
            }
        }

        public DataTable SelectRecords()
        {
            using (SQLiteConnection conn = new SQLiteConnection(GetConnectionString()))
            {
                conn.Open();
                string sql = "select * from highscores order by score desc";
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = sql;
                SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult);
                DataTable dt = new DataTable();
                dt.Load(reader);
                return dt;
            }
        }

        public void DeleteRecords()
        {
            using (SQLiteConnection conn = new SQLiteConnection(GetConnectionString()))
            {
                conn.Open();
                string sql = "delete from highscores";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
            }
        }
    }
}
