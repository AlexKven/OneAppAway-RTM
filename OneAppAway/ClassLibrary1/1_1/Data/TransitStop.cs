using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public struct TransitStop
    {
        //private static SqlProvider<TransitStop> _SqlProvider;
        //public static SqlProvider<TransitStop> SqlProvider
        //{
        //    get
        //    {
        //        if (_SqlProvider == null)
        //            _SqlProvider = new SqlProvider<TransitStop>(
        //                new SqlProviderPropertySpecification("ID", "varchar(20)") { IsPrimaryKey = true },
        //                new SqlProviderPropertySpecification("Parent", "varchar(20)"),
        //                new SqlProviderPropertySpecification("Position", new string[] { "Latitude", "Longitude" },
        //                    new string[] { "float", "float" },
        //                    val => new string[] { double.IsNaN(((LatLon)val).Latitude) ? null : ((LatLon)val).Latitude.ToString(),
        //                                            double.IsNaN(((LatLon)val).Longitude) ? null : ((LatLon)val).Longitude.ToString()},
        //                    sql => new LatLon(sql[0] == null ? double.NaN : double.Parse(sql[0]), sql[1] == null ? double.NaN : double.Parse(sql[1]))),
        //                new SqlProviderPropertySpecification("Direction", "tinyint",
        //                    obj => ((int)((StopDirection)obj)).ToString(),
        //                    sql => sql == null ? StopDirection.Unspecified : (StopDirection)int.Parse(sql)),
        //                new SqlProviderPropertySpecification("Path", "varchar(128)"),
        //                new SqlProviderPropertySpecification("Name", "varchar(64)"),
        //                new SqlProviderPropertySpecification("Code", "varchar(16)"),
        //                new SqlProviderPropertySpecification("Provider", "varchar(4)"),
        //                new SqlProviderPropertySpecification("ProviderID", "varchar(20)")
        //                )
        //            {
        //                CustomSelect = (queryCallback, conditions, baseFunc) =>
        //                {
        //                    var results = baseFunc(queryCallback, conditions);
        //                    return results.Select(item =>
        //                    {
        //                        item.Children = queryCallback($"select ID from TransitStop where Parent = '{item.ID}';").GetColumn(0);
        //                        return item;
        //                    });
        //                }
        //            };
        //        return _SqlProvider;
        //    }
        //}

        private static TransitStopSqlProvider _SqlProvider = new TransitStopSqlProvider();
        public static TransitStopSqlProvider SqlProvider => _SqlProvider;

        public string ID { get; set; }
        public string Parent { get; set; }
        public LatLon Position { get; set; }
        public StopDirection Direction { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Provider { get; set; }
        public string ProviderID { get; set; }

        public string[] Children { get; set; }
        //public int LocationType { get; set; }
        public string[] Routes { get; set; }

        public static bool operator ==(TransitStop lhs, TransitStop rhs)
        {
            return lhs.ID == rhs.ID;
        }

        public static bool operator !=(TransitStop lhs, TransitStop rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is TransitStop)) return false;
            return this == (TransitStop)obj;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}
