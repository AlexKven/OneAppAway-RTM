using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public static class SqlProviderExtensions
    {
        public static string SqlEscape(this string str, params char[] ignore)
        {
            if (str == null) return null;
            string result = str;
            for (int i = 0; i < EscapableChars.Length; i++)
            {
                if (!ignore.Contains(EscapableChars[i]))
                    result = result.Replace(EscapableChars[i].ToString(), EscapableChars[0].ToString() + EscapeIndices[i].ToString());
            }
            return result;
        }

        public static string SqlUnEscape(this string str)
        {
            string result = str;
            for (int i = 0; i < EscapableChars.Length; i++)
            {
                result = result.Replace(EscapableChars[0].ToString() + EscapeIndices[i].ToString(), EscapableChars[i].ToString());
            }
            return result;
        }

        public static readonly string EscapableChars = "/!~'_%^|()[]{}`@#?";
        public static readonly string EscapeIndices = "0123456789abcdefghijklmnopqrstuvwxyz";

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

        public static string[] GetColumn(this string[,] sqlResult, int column)
        {
            int size = sqlResult.GetLength(1);
            string[] result = new string[size];
            for (int i = 0; i < size; i++)
                result[i] = sqlResult[column, i];
            return result;
        }
    }
}
