﻿//*dbusing SQLite.Net;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public static class DatabaseRetriever
    {
        public static IEnumerable<TransitStop> RetrieveStops(params string[] ids)
        {
            if (ids.Length == 0)
                return TransitStop.SqlProvider.Select(DatabaseManager.MemoryDatabase);
            else
            {
                StringBuilder whereBuilder = new StringBuilder();
                for (int i = 0; i < ids.Length; i++)
                {
                    if (i > 0)
                        whereBuilder.Append(" or ");
                    whereBuilder.Append($"ID = '{ids[i]}'");
                }
                return TransitStop.SqlProvider.Select(DatabaseManager.MemoryDatabase, whereBuilder.ToString());
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
