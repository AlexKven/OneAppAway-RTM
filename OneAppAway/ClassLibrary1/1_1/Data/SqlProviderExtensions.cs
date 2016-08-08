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
    }
}
