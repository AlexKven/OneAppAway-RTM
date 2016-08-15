//*dbusing SQLite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace OneAppAway._1_1.Data
{
    public static class DatabaseRetriever
    {
        private static TransitStopSqlProvider StopProvider = new TransitStopSqlProvider();
        public static IEnumerable<TransitStop> RetrieveStops(params string[] ids)
        {
            if (ids.Length == 0)
                return StopProvider.Select(query => DatabaseManager.ExecuteSQL(DatabaseManager.MemoryDatabase, query));
            else
            {
                StringBuilder whereBuilder = new StringBuilder();
                for (int i = 0; i < ids.Length; i++)
                {
                    if (i > 0)
                        whereBuilder.Append(" or ");
                    whereBuilder.Append($"ID = '{ids[i]}'");
                }
                return StopProvider.Select(query => DatabaseManager.ExecuteSQL(DatabaseManager.MemoryDatabase, query), whereBuilder.ToString());
            }
        }
    }

    //public static class DatabaseManager_Old
    //{
    //    public static readonly SQLite.Net.Platform.WinRT.SQLitePlatformWinRT Platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
    //    public static readonly string DBPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
    //    private static SQLiteConnection Connection = null;

    //    public static SQLiteConnection GetConnection()
    //    {
    //        if (Connection == null)
    //            Connection = new SQLiteConnection(Platform, DBPath);
    //        return Connection;

    //    }

    //    public static void DisposeConnection()
    //    {
    //        Connection?.Dispose();
    //    }
    //}
}
