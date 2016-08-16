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

        #region Public
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

        public virtual IEnumerable<T> Select(Func<string, string[,]> queryCallback, string conditions = null)
        {
            return GetObjects(queryCallback($"select * from {TableName}{(conditions == null ? "" : $" where {conditions}")};"));
        }

        public virtual void Insert(T value, Func<string, string[,]> queryCallback, bool replace = false)
        {
            queryCallback(InsertQuery(value, replace));
        }

        public virtual void CreateTable(Func<string, string[,]> queryCallback, bool ifNotExists = true)
        {
            queryCallback(CreateTableQuery(ifNotExists));
        }
        #endregion

        #region Protected
        protected string CreateTableQuery(bool ifNotExists)
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

        protected abstract string GetColumn(T value, string columnName);

        protected abstract T ParseRow(string[] row);

        public string InsertQuery(T val, bool replace)
        {
            StringBuilder queryBuilder;
            if (replace)
                queryBuilder = new StringBuilder("insert or replace into ");
            else
                queryBuilder = new StringBuilder("insert or ignore into ");
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
        #endregion
    }
}