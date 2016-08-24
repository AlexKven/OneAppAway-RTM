using Newtonsoft.Json;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Data.Json;
using Windows.Data.Xml.Dom;

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

        public virtual IEnumerable<T> Select(SQLiteConnection connection, string conditions = null)
        {
            return GetObjects(ExecuteSQL(connection, $"select * from {TableName}{(conditions == null ? "" : $" where {conditions}")};"));
        }

        public virtual void Insert(T value, SQLiteConnection connection, bool replace = false)
        {
            ExecuteSQL(connection, InsertQuery(value, replace));
        }

        public virtual void CreateTable(SQLiteConnection connection, bool ifNotExists = true)
        {
            ExecuteSQL(connection, CreateTableQuery(ifNotExists));
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

        #region Static
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
        #endregion
    }
}