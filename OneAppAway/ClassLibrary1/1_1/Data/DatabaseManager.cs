using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SQLitePCL;
using OneAppAway._1_1.Data;

#if WINDOWS_UWP
using SQLitePCL;
using OneAppAway._1_1.Data;
#endif

namespace OneAppAway._1_1
{
    public static class DatabaseManager
    {
        private static SQLiteConnection _MemoryDatabase;
        private static SQLiteConnection _DiskDatabase;

        public static SQLiteConnection MemoryDatabase => _MemoryDatabase;

        public static void Initialize(string diskDBLocation)
        {
            _MemoryDatabase = new SQLiteConnection(":memory:");
            if (diskDBLocation != null)
                _DiskDatabase = new SQLiteConnection(diskDBLocation);
        }
    }
}
