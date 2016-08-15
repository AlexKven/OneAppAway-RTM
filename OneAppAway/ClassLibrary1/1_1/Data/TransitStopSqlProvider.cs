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
            ColumnNames = new string[] { "ID", "Parent", "Latitude", "Longitude", "Direction", "Path", "Name", "Code", "Provider", "ProviderID" };
            ColumnDescriptors = new string[] { "varchar(20) primary key", "varchar(20)", "float", "float", "tinyint", "varchar(128)", "varchar(64)", "varchar(16)", "varchar(4)", "varchar(20)" };
        }

        protected override string GetColumn(TransitStop value, string columnName)
        {
            switch (columnName)
            {
                case "ID":
                    return value.ID;
                case "Parent":
                    return value.Parent;
                //case "Position":
                //    return value.Position.ToString("+000.000000;-000.000000;0000.000000");
                case "Latitude":
                    return double.IsNaN(value.Position.Latitude) ? null : value.Position.Latitude.ToString();
                case "Longitude":
                    return double.IsNaN(value.Position.Longitude) ? null : value.Position.Longitude.ToString();
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
            result.Position = new LatLon(row[2] == null ? double.NaN : double.Parse(row[2]), row[3] == null ? double.NaN : double.Parse(row[3]));
            result.Direction = row[4] == null ? StopDirection.Unspecified : (StopDirection)int.Parse(row[4]);
            result.Path = row[5];
            result.Name = row[6];
            result.Code = row[7];
            result.Provider = row[8];
            result.ProviderID = row[9];
            return result;
        }

        public override IEnumerable<TransitStop> Select(Func<string, string[,]> queryCallback, string conditions = null)
        {
            var results = GetObjects(queryCallback($"select * from {TableName}{(conditions == null ? "" : $" where {conditions}")};"));
            return results.Select(item =>
            {
                item.Children = queryCallback($"select ID from {TableName} where Parent = '{item.ID}';").GetColumn(0);
                return item;
            });
        }
    }
}
