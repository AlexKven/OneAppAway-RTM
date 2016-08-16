using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace OneAppAway._1_1.Data
{
    public sealed class SqlProvider<T> : SqlProviderBase<T> where T : new()
    {
        private TypeInfo Info;
        private Tuple<PropertyInfo, SqlProviderPropertySpecification>[] PropertyMappings;
        private Dictionary<string, Tuple<int, int>> ColumnMappings;

        public SqlProvider(params SqlProviderPropertySpecification[] descriptions)
        {
            List<string> columnNames = new List<string>();
            List<string> columnDescriptors = new List<string>();
            List<Tuple<PropertyInfo, SqlProviderPropertySpecification>> propertyMappings = new List<Tuple<PropertyInfo, SqlProviderPropertySpecification>>();
            ColumnMappings = new Dictionary<string, Tuple<int, int>>();
            Info = typeof(T).GetTypeInfo();
            TableName = typeof(T).Name;
            foreach  (var property in Info.DeclaredProperties)
            {
                var attrib = descriptions.FirstOrDefault(desc => desc.GetPropertyName() == property.Name);
                if (attrib != null)
                {
                    int colNum = 0;
                    for (int i = 0; i < attrib.GetColumnCount(); i++)
                    {
                        var cn = attrib.GetColumnName(i);
                        cn = cn ?? (colNum++ == 0 ? property.Name : property.Name + colNum.ToString());
                        var cd = attrib.GetSqlType(i);
                        cd = cd + (attrib.IsPrimaryKey ? " primary key" : "");
                        columnNames.Add(cn);
                        columnDescriptors.Add(cd);
                        ColumnMappings.Add(cn, new Tuple<int, int>(propertyMappings.Count, i));
                    }
                    propertyMappings.Add(new Tuple<PropertyInfo, SqlProviderPropertySpecification>(property, attrib));
                }
            }
            ColumnNames = columnNames.ToArray();
            ColumnDescriptors = columnDescriptors.ToArray();
            PropertyMappings = propertyMappings.ToArray();
        }

        private Dictionary<string, string> RapidColumnCache = new Dictionary<string, string>();
        private object RapidCachedValue;

        protected override string GetColumn(T value, string columnName)
        {
            string result;
            if (Object.ReferenceEquals(RapidCachedValue, value) && RapidColumnCache.ContainsKey(columnName))
            {
                result = RapidColumnCache[columnName];
                RapidColumnCache.Remove(result);
                System.Diagnostics.Debug.WriteLine($"Retrieved from rapid cache {value.ToString()}, column = {columnName}.");
                if (RapidColumnCache.Count == 0)
                {
                    RapidCachedValue = null;
                    System.Diagnostics.Debug.WriteLine($"Deleted rapid cache {value.ToString()}.");
                }
                return result;
            }
            var cMapping = ColumnMappings[columnName];
            var pMapping = PropertyMappings[cMapping.Item1];
            var columnNames = pMapping.Item2.GetColumnNames();
            var columnValues = pMapping.Item2.GetToSql()(pMapping.Item1.GetValue(value));
            if (columnNames.Length > 1)
            {
                System.Diagnostics.Debug.WriteLine($"Rapid caching {value.ToString()}, column = {columnName}.");
                RapidCachedValue = value;
                RapidColumnCache.Clear();
                for (int i = 0; i < columnNames.Length; i++)
                {
                    if (i == cMapping.Item2)
                        continue;
                    RapidColumnCache.Add(columnNames[i], columnValues[i]);
                }
            }
            return columnValues[cMapping.Item2];
        }

        protected override T ParseRow(string[] row)
        {
            Dictionary<string, string[]> valueMappings = new Dictionary<string, string[]>();
            List<string> curValueMapping = null;
            int curValueMappingNumber = -1;
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                var columnMapping = ColumnMappings[ColumnNames[i]];
                if (columnMapping.Item1 != curValueMappingNumber)
                {
                    if (curValueMapping != null)
                        valueMappings.Add(PropertyMappings[curValueMappingNumber].Item1.Name, curValueMapping.ToArray());
                    curValueMapping = new List<string>();
                    curValueMappingNumber = columnMapping.Item1;
                }
                curValueMapping.Add(row[i]);
            }
            if (curValueMapping != null)
                valueMappings.Add(PropertyMappings[PropertyMappings.Length - 1].Item1.Name, curValueMapping.ToArray());
            object result = new T();
            foreach (var propertyMapping in PropertyMappings)
                propertyMapping.Item1.SetValue(result, propertyMapping.Item2.GetFromSql()(valueMappings[propertyMapping.Item1.Name]));
            return (T)result;
        }

        public override void CreateTable(Func<string, string[,]> queryCallback, bool ifNotExists = true)
        {
            base.CreateTable(queryCallback, ifNotExists);
        }

        public override void Insert(T value, Func<string, string[,]> queryCallback, bool replace = false)
        {
            base.Insert(value, queryCallback, replace);
        }

        public Func<Func<string, string[,]>, string, Func<Func<string, string[,]>, string, IEnumerable<T>>, IEnumerable<T>> CustomSelect;
        public override IEnumerable<T> Select(Func<string, string[,]> queryCallback, string conditions = null)
        {
            if (CustomSelect == null)
                return base.Select(queryCallback, conditions);
            else
            {
                return CustomSelect(queryCallback, conditions, (qc, c) => base.Select(qc, c));
            }
        }
    }
    
    public class SqlProviderPropertySpecification
    {
        public SqlProviderPropertySpecification(string propertyName, string[] columnNames, string[] sqlTypes, Func<object, string[]> toSql, Func<string[], object> fromSql)
        {
            _PropertyName = propertyName;
            _ColumnNames = columnNames;
            _SqlTypes = sqlTypes;
            _ToSql = toSql;
            _FromSql = fromSql;
        }

        public SqlProviderPropertySpecification(string propertyName, string columnName, string sqlType, Func<object, string> toSql, Func<string, object> fromSql)
            : this(propertyName, new string[] { columnName }, new string[] { sqlType }, obj => new string[] { toSql(obj) }, sql => fromSql(sql[0])) { }

        public SqlProviderPropertySpecification(string propertyName, string columnName, string sqlType, Func<string, object> fromSql)
            : this(propertyName, columnName, sqlType , obj => obj?.ToString(), fromSql) { }

        public SqlProviderPropertySpecification(string propertyName, string sqlType, Func<object, string> toSql, Func<string, object> fromSql)
            : this(propertyName, null, sqlType, toSql, fromSql) { }

        public SqlProviderPropertySpecification(string propertyName, string sqlType, Func<string, object> fromSql)
            : this(propertyName, null, sqlType, fromSql) { }

        public SqlProviderPropertySpecification(string propertyName, string sqlType)
            : this(propertyName, sqlType, sql => sql) { }

        private string _PropertyName;
        public string GetPropertyName() => _PropertyName;

        private string[] _SqlTypes;
        public string[] GetSqlTypes() => _SqlTypes;
        public string GetSqlType(int index) => _SqlTypes[index];

        private string[] _ColumnNames = null;
        public string[] GetColumnNames() => _ColumnNames;
        public string GetColumnName(int index) => _ColumnNames[index];

        private Func<object, string[]> _ToSql;
        public Func<object, string[]> GetToSql() => _ToSql;

        private Func<string[], object> _FromSql;
        public Func<string[], object> GetFromSql() => _FromSql;

        public int GetColumnCount() => _ColumnNames.Length;

        public bool IsPrimaryKey;
        public bool IsEscapeSafe;
    }
}
