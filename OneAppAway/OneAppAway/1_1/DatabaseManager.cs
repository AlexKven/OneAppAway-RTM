using SQLite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace OneAppAway._1_1
{
    public static class DatabaseManager
    {
        public static readonly SQLite.Net.Platform.WinRT.SQLitePlatformWinRT Platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
        public static readonly string DBPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
        private static SQLiteConnection Connection = null;

        public static SQLiteConnection GetConnection()
        {
            if (Connection == null)
                Connection = new SQLiteConnection(Platform, DBPath);
            return Connection;
        }

        public static void DisposeConnection()
        {
            Connection?.Dispose();
        }
    }
}
