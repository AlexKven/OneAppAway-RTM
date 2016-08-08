using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public class TransitStopSqlProvider : SqlProviderBase<TransitStop>
    {
        public TransitStopSqlProvider()
        {
            TableName = "TransitStop";
            ColumnNames = new string[] { "ID", "Parent", "Position", "Direction", "Path", "Name", "Code", "Provider", "ProviderID" };
            ColumnDescriptors = new string[] { "varchar(20) primary key", "varchar(20)", "char(26)", "tinyint", "varchar(128)", "varchar(64)", "varchar(16)", "varchar(4)", "varchar(20)" };
        }

        protected override string GetColumn(TransitStop value, string columnName)
        {
            switch (columnName)
            {
                case "ID":
                    return value.ID;
                case "Parent":
                    return value.Parent;
                case "Position":
                    return value.Position.ToString("+000.000000;-000.000000;0000.000000");
                case "Direction":
                    return ((int)value.Direction).ToString();
                case "Path":
                    return value.Path.SqlEscape();
                case "Name":
                    return value.Name.SqlEscape();
                case "Code":
                    return value.Code;
                case "Provider":
                    return value.Provider;
                case "ProviderID":
                    return value.ProviderID;
                default:
                    return null;
            }
        }

        protected override TransitStop ParseRow(string[] row)
        {
            TransitStop result = new TransitStop();
            result.ID = row[0];
            result.Parent = row[1];
            result.Position = row[2] == null ? new LatLon(double.NaN, double.NaN) : LatLon.Parse(row[2]);
            result.Direction = row[3] == null ? StopDirection.Unspecified : (StopDirection)int.Parse(row[3]);
            result.Path = row[4];
            result.Name = row[5];
            result.Code = row[6];
            result.Provider = row[7];
            result.ProviderID = row[8];
            return result;
        }
    }
}
