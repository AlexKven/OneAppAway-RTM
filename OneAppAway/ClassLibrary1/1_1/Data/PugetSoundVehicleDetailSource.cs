using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OneAppAway._1_1.Helpers;

namespace OneAppAway._1_1.Data
{
    //Light rail image taken from
    //https://southseattleemerald.files.wordpress.com/2015/04/3445792161_057434888f_b.jpg
    public class PugetSoundVehicleDetailSource
    {
        [DataContract]
        public class VehicleDetailGroup
        {
            [DataMember]
            [XmlAttribute]
            public int NumberPrefix { get; set; }
            [DataMember]
            [XmlAttribute]
            public int NumberMin { get; set; }
            [DataMember]
            [XmlAttribute]
            public int NumberMax { get; set; }
            [DataMember]
            [XmlArray]
            public int[] IncludedNumbers { get; set; }
            [DataMember]
            [XmlArray]
            public int[] ExcludedNumbers { get; set; }
            [DataMember]
            [XmlAttribute]
            public string Name { get; set; }
            [DataMember]
            [XmlAttribute]
            public string Capacity { get; set; }
            [DataMember]
            [XmlAttribute]
            public string OperatedBy { get; set; }
            [DataMember]
            [XmlAttribute]
            public string FundedBy { get; set; }
            [DataMember]
            [XmlAttribute]
            public string Image { get; set; }

            public bool TryGetVehicleDetails(RealTimeArrival arrival, out VehicleDetail detail)
            {
                detail = new VehicleDetail();
                if (arrival.Vehicle != null)
                {
                    string[] parts = arrival.Vehicle.Split('_');
                    if (parts.Length == 2)
                    {
                        int prefix;
                        int postfix;
                        if (int.TryParse(parts[0], out prefix) && int.TryParse(parts[1], out postfix) && ((NumberPrefix == prefix && NumberMin <= postfix && NumberMax >= postfix && !(ExcludedNumbers?.Contains(postfix) ?? false)) || (IncludedNumbers?.Contains(postfix) ?? false)))
                        {
                            detail.AgencyFunded = FundedBy;
                            detail.AgencyOperated = OperatedBy;
                            detail.VehicleName = Name;
                            detail.SeatedCapacity = Capacity.ToString();
                            detail.ImageUri = new Uri(Image);
                            detail.VehicleNumber = postfix.ToString();
                            return true;
                        }
                    }
                }
                return false;
            }

            public static IEnumerable<Type> GetSerializationTypes()
            {
                return new[] { typeof(string), typeof(int), typeof(int[]) };
            }
        }

        private const string LinkRouteId = "40_100479";

        private VehicleDetailGroup[] VehicleGroups;
        private string[] ThreeCarLinkTrips;

        public PugetSoundVehicleDetailSource()
        {
            var assembly = Assembly.Load(new AssemblyName("CommonClasses"));
            XmlSerializer deserializer = new XmlSerializer(typeof(VehicleDetailGroup[]), new XmlAttributeOverrides(), VehicleDetailGroup.GetSerializationTypes().ToArray(), new XmlRootAttribute("VehicleDetailGroups"), null);
            using (var stream = assembly.GetManifestResourceStream(@"OneAppAway._1_1.XmlDataSources.PugetSoundBusVehicleData.xml"))
            {
                VehicleGroups = (VehicleDetailGroup[])deserializer.Deserialize(stream);
            }
            deserializer = new XmlSerializer(typeof(string[]), new XmlAttributeOverrides(), new[] { typeof(string) }, new XmlRootAttribute("ThreeCarTrips"), null);
            using (var stream = assembly.GetManifestResourceStream(@"OneAppAway._1_1.XmlDataSources.LinkLightRailThreeCarTrainsSchedule.xml"))
            {
                ThreeCarLinkTrips = (string[])deserializer.Deserialize(stream);
            }
        }

        private VehicleDetail GetLinkVehicleDetails(RealTimeArrival arrival)
        {
            var day = arrival.ScheduledArrivalTime?.GetServiceDay();
            bool threeCars = false;
            if (day.HasValue && (day.Value == ServiceDay.Saturday || day.Value == ServiceDay.Sunday))
                threeCars = true;
            else if (ThreeCarLinkTrips.Contains(arrival.Trip))
                threeCars = true;
            VehicleDetail result = new VehicleDetail() { AgencyFunded = "Sound Transit", AgencyOperated = "King County Metro", SeatedCapacity = "Unknown", VehicleName = (threeCars ? "3" : "2") + " Car Link Train", VehicleNumber = "Unknown" };
            result.ImageUri = new Uri(threeCars ? "http://ww2.hdnux.com/photos/03/71/66/1023397/4/628x471.jpg" : "https://upload.wikimedia.org/wikipedia/commons/thumb/8/8d/2-car_Central_Link_train_in_Tukwila.jpg/320px-2-car_Central_Link_train_in_Tukwila.jpg");
            return result;
        }

        public VehicleDetail? GetVehicleDetails(RealTimeArrival arrival)
        {
            if (arrival.Route == LinkRouteId)
                return GetLinkVehicleDetails(arrival);
            VehicleDetail result;
            foreach (var group in VehicleGroups)
            {
                if (group.TryGetVehicleDetails(arrival, out result))
                    return result;
            }
            if (arrival.Vehicle != null)
            {
                result = new VehicleDetail();
                string[] parts = arrival.Vehicle.Split('_');
                if (parts.Length == 2)
                {
                    int prefix;
                    int postfix;
                    if (int.TryParse(parts[0], out prefix) && int.TryParse(parts[1], out postfix))
                    {
                        switch (prefix)
                        {
                            case 1:
                                result.AgencyFunded = result.AgencyOperated = "King County Metro";
                                break;
                            case 40:
                                result.AgencyFunded = "Sound Transit";
                                result.AgencyOperated = "Unknown";
                                break;
                        }
                        result.VehicleNumber = postfix.ToString();
                        result.VehicleName = "Unknown";
                        result.SeatedCapacity = "Unknown";
                        return result;
                    }
                }
            }
            return null;
        }

        public static PugetSoundVehicleDetailSource Instance { get; } = new PugetSoundVehicleDetailSource();
    }
}
