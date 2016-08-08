using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

#if WINDOWS_UWP
using SQLitePCL;
using OneAppAway._1_1.Data;
#endif

namespace OneAppAway._1_1
{
    internal static class DatabaseManager
    {
#if WINDOWS_UWP
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
#endif
        public static T ToNumber<T>(this string[,] sqlResult) where T : struct
        {
            if (sqlResult.GetLength(0) == 0 || sqlResult.GetLength(1) == 0)
                throw new IndexOutOfRangeException("sqlResult doesn't contain any elements.");
            T result;
            if (TryToNumber<T>(sqlResult, out result))
                return result;
            else
                throw new FormatException($"{sqlResult[0, 0]} could not be parsed into a {typeof(T).FullName}.");
        }

        public static T ToNumber<T>(this string[,] sqlResult, T def) where T : struct
        {
            T result;
            return TryToNumber<T>(sqlResult, out result) ? result : def;
        }

        public static bool TryToNumber<T>(this string[,] sqlResult, out T result) where T : struct
        {
            //System.
            if (sqlResult.GetLength(0) > 0 || sqlResult.GetLength(1) > 0)
            {
                bool success;
                string type = typeof(T).FullName;
                if (type == "System.Int32")
                {
                    int tResult;
                    success = int.TryParse(sqlResult[0, 0], out tResult);
                    result = (T)(object)tResult;
                    return success;
                }
                else if (type == "System.Int64")
                {
                    long tResult;
                    success = long.TryParse(sqlResult[0, 0], out tResult);
                    result = (T)(object)tResult;
                    return success;
                }
                else if (type == "System.Byte")
                {
                    byte tResult;
                    success = byte.TryParse(sqlResult[0, 0], out tResult);
                    result = (T)(object)tResult;
                    return success;
                }
                else if (type == "System.Int16")
                {
                    short tResult;
                    success = short.TryParse(sqlResult[0, 0], out tResult);
                    result = (T)(object)tResult;
                    return success;
                }
                else if (type == "System.Double")
                {
                    double tResult;
                    success = double.TryParse(sqlResult[0, 0], out tResult);
                    result = (T)(object)tResult;
                    return success;
                }
                else if (type == "System.Single")
                {
                    float tResult;
                    success = float.TryParse(sqlResult[0, 0], out tResult);
                    result = (T)(object)tResult;
                    return success;
                }
                else if (type == "System.Decimal")
                {
                    decimal tResult;
                    success = decimal.TryParse(sqlResult[0, 0], out tResult);
                    result = (T)(object)tResult;
                    return success;
                }
                else if (type == "System.Boolean")
                {
                    bool tResult;
                    success = bool.TryParse(sqlResult[0, 0], out tResult);
                    result = (T)(object)tResult;
                    return success;
                }
                else
                if (type == "System.UInt32")
                {
                    uint tResult;
                    success = uint.TryParse(sqlResult[0, 0], out tResult);
                    result = (T)(object)tResult;
                    return success;
                }
                else if (type == "System.UInt64")
                {
                    ulong tResult;
                    success = ulong.TryParse(sqlResult[0, 0], out tResult);
                    result = (T)(object)tResult;
                    return success;
                }
                else if (type == "System.SByte")
                {
                    sbyte tResult;
                    success = sbyte.TryParse(sqlResult[0, 0], out tResult);
                    result = (T)(object)tResult;
                    return success;
                }
                else if (type == "System.UInt16")
                {
                    ushort tResult;
                    success = ushort.TryParse(sqlResult[0, 0], out tResult);
                    result = (T)(object)tResult;
                    return success;
                }
            }
            result = new T();
            return false;
        }
    }
}
