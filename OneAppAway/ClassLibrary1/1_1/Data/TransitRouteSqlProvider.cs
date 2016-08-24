using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public class TransitRouteSqlProvider : SqlProviderBase<TransitRoute>
    {
        public TransitRouteSqlProvider()
        {
            TableName = "TransitRoute";
            ColumnNames = new string[] { "ID", "Name", "Description", "Agency", "Provider", "ProviderID" };
            ColumnDescriptors = new string[] { "varchar(20) primary key", "varchar(64)", "varchar(200)", "varchar(16)", "varchar(4)", "varchar(16)" };
        }

        protected override string GetColumn(TransitRoute value, string columnName)
        {
            switch (columnName)
            {
                case "ID":
                    return value.ID;
                case "Name":
                    return value.Name;
                case "Description":
                    return value.Description;
                case "Agency":
                    return value.Agency;
                case "Provider":
                    return value.Provider;
                case "ProviderID":
                    return value.ProviderID;
                default:
                    return null;
            }
        }

        protected override TransitRoute ParseRow(string[] row)
        {
            TransitRoute result = new TransitRoute();
            result.ID = row[0];
            result.Name = row[1];
            result.Description = row[2];
            result.Agency = row[3];
            result.Provider = row[4];
            result.ProviderID = row[5];
            return result;
        }
    }
}
