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

        public static string[,] ExecuteSQL(SQLiteConnection conn, string line, out string[] columns, out int numRows)
        {
            List<string[]> rows = new List<string[]>();
            using (var statement = conn.Prepare(line))
            {
                if (statement.ColumnCount == 0)
                {
                    statement.Step();
                    columns = new string[0];
                    numRows = 0;
                    return null;
                }
                else
                {
                    columns = new string[statement.ColumnCount];
                    for (int i = 0; i < statement.ColumnCount; i++)
                        columns[i] = statement.ColumnName(i);

                    while (statement.Step() != SQLiteResult.DONE)
                    {
                        string[] row = new string[statement.ColumnCount];
                        for (int i = 0; i < statement.ColumnCount; i++)
                        {
                            row[i] = statement[i]?.ToString()?.SqlUnEscape();
                        }
                        rows.Add(row);
                    }
                }
            }
            numRows = rows.Count;
            string[,] result = new string[columns.Length, numRows];
            for (int y = 0; y < numRows; y++)
            {
                for (int x = 0; x < columns.Length; x++)
                {
                    result[x, y] = rows[y][x];
                }
            }
            return result;
        }

        public static string[,] ExecuteSQL(SQLiteConnection conn, string line, out string[] columns)
        {
            int dummy;
            return ExecuteSQL(conn, line, out columns, out dummy);
        }

        public static string[,] ExecuteSQL(SQLiteConnection conn, string line)
        {
            string[] dummy;
            return ExecuteSQL(conn, line, out dummy);
        }
    }
}
