using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public abstract class SqlProviderBase<T>
    {
        protected string TableName;
        protected string[] ColumnNames;
        protected string[] ColumnDescriptors;

        public string CreateTableQuery(bool ifNotExists = true)
        {
            StringBuilder queryBuilder = new StringBuilder("create table ");
            if (ifNotExists)
                queryBuilder.Append("if not exists ");
            queryBuilder.Append(TableName);
            queryBuilder.Append('(');
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                if (i > 0)
                    queryBuilder.Append(", ");
                queryBuilder.Append(ColumnNames[i]);
                queryBuilder.Append(' ');
                queryBuilder.Append(ColumnDescriptors[i]);
            }
            queryBuilder.Append(");");
            return queryBuilder.ToString();
        }

        public string InsertQuery(T val)
        {
            StringBuilder queryBuilder = new StringBuilder("insert into ");
            queryBuilder.Append(TableName);
            queryBuilder.Append(" values(");
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                if (i > 0)
                    queryBuilder.Append(", ");
                var str = GetColumn(val, ColumnNames[i]);
                if (str == null)
                    queryBuilder.Append("null");
                else
                {
                    queryBuilder.Append('\'');
                    queryBuilder.Append(str);
                    queryBuilder.Append('\'');
                }
            }
            queryBuilder.Append(");");
            return queryBuilder.ToString();
        }

        public IEnumerable<T> GetObjects(string[,] sqlResult, string[] columns)
        {
            int numRows = sqlResult.GetLength(1);
            T[] result = new T[numRows];
            int[] colIndices = new int[ColumnDescriptors.Length];
            for (int i = 0; i < ColumnDescriptors.Length; i++)
            {
                colIndices[i] = Math.Max(columns.Aggregate(-1, (acc, str) => acc > 0 ? acc : str == ColumnNames[i] ? -acc : acc - 1) - 1, -1);
            }
            for (int i = 0; i < numRows; i++)
            {
                string[] row = new string[ColumnDescriptors.Length];
                for (int j = 0; j < ColumnDescriptors.Length; j++)
                {
                    row[j] = colIndices[j] == -1 ? null : sqlResult[colIndices[j], i];
                }
                result[i] = ParseRow(row);
            }
            return result;
        }

        public IEnumerable<T> GetObjects(string[,] sqlResult)
        {
            return GetObjects(sqlResult, ColumnNames);
        }

        protected abstract string GetColumn(T value, string columnName);

        protected abstract T ParseRow(string[] row);

        public abstract IEnumerable<T> Select(Func<string, string[,]> queryCallback, string conditions = null);
    }
}