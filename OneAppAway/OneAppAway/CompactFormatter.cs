using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway
{
    public class CompactFormatReader
    {
        private string Data;
        private int Index = 0;
        private bool InQuotation = false;
        private string SpecialChars = "\"\\,()|";

        public CompactFormatReader(string data)
        {
            Data = data;
        }

        public CompactFormatReader[] Next()
        {
            List<CompactFormatReader> result = new List<CompactFormatReader>();
            StringBuilder data = new StringBuilder();
            int level = 0;
            bool escaped = false;
            bool end = false;
            //bool indented = false;
            if (Index >= Data.Length || Data[Index] == ')')
                return null;
            //if (Data[Index] == '(')
            //{
            //    Index++;
            //    indented = true;
            //}
            while (!end && Index < Data.Length)
            {
                char nextChar = Data[Index];
                if (level == 0 && "|)".Contains(nextChar) && !escaped)
                    end = true;
                else
                {
                    if (nextChar == '(' && !escaped)
                    {
                        if (level == 0)
                        {
                            result.Add(new CompactFormatReader(data.ToString()));
                            data = new StringBuilder();
                            //Bad practice
                            escaped = false;
                            Index++;
                            level++;
                            continue;
                        }
                        level++;
                    }
                    else if (nextChar == ')' && !escaped)
                    {
                        level--;
                    }
                    if (nextChar == ',' && level == 0 && !escaped)
                    {
                        result.Add(new CompactFormatReader(data.ToString()));
                        data = new StringBuilder();
                    }
                    else if (nextChar == '|' && level == 0 && !escaped)
                    {
                        end = true;
                    }
                    else
                        data.Append(nextChar);
                }
                escaped = nextChar == '\\';
                Index++;
            }
            result.Add(new CompactFormatReader(data.ToString()));
            //if (indented)
            //    Index++;
            return result.ToArray();
        }

        public string ReadString()
        {
            if (Data[0] == '"' && Data.Length > 2)
            {
                StringBuilder result = new StringBuilder();
                int ind = 1;
                bool end = (Data[1] == '"');
                bool escaped = false;
                while (!end && ind < Data.Length)
                {
                    if (Data[ind] == '\\' && !escaped)
                        escaped = true;
                    else if (Data[ind] == '"' && !escaped)
                        end = true;
                    else
                    {
                        result.Append(Data[ind]);
                        escaped = false;
                    }
                    ind++;
                }
                return result.ToString();
            }
            else
            {
                return Data;
            }
        }

        public int ReadInt()
        {
            return int.Parse(ReadString());
        }
    }

    public class CompactFormatWriter
    {
        private StringBuilder Builder = new StringBuilder();
        private bool DelimiterNeeded = false;
        bool DelimiterJustPlaced = false;
        private string SpecialChars = "\"\\,()|";

        public void WriteString(string str)
        {
            if (DelimiterNeeded)
                Builder.Append(',');
            Builder.Append(str);
            DelimiterNeeded = true;
            DelimiterJustPlaced = false;
        }

        public void WriteInt(int num)
        {
            WriteString(num.ToString());
        }

        public void WriteQuotedString(string str)
        {
            StringBuilder newStringBuilder = new StringBuilder("\"");
            foreach (char chr in str)
            {
                if (SpecialChars.Contains(chr))
                    newStringBuilder.Append("\\");
                newStringBuilder.Append(chr);
            }
            newStringBuilder.Append("\"");
            WriteString(newStringBuilder.ToString());
        }

        public void NextItem()
        {
            TrimDelimiter();
            Builder.Append('|');
            DelimiterNeeded = false;
            DelimiterJustPlaced = true;
        }

        public void OpenParens()
        {
            Builder.Append("(");
            DelimiterNeeded = false;
        }

        public void  CloseParens()
        {
            TrimDelimiter();
            Builder.Append(")");
            DelimiterNeeded = false;
        }

        public void TrimDelimiter()
        {
            if (DelimiterJustPlaced)
                Builder.Remove(Builder.Length - 1, 1);
            DelimiterJustPlaced = false;
        }

        public override string ToString()
        {
            if (DelimiterJustPlaced)
                return Builder.ToString().Substring(0, Builder.Length - 1);
            else
                return Builder.ToString();
        }
    }
}
