using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Maps;

namespace OneAppAway
{
    [DataContract]
    public class FavoriteArrival
    {
        [DataMember]
        public string Route { get; set; }
        [DataMember]
        public string Stop { get; set; }
        [DataMember]
        public string Destination { get; set; }
        [DataMember]
        public string CustomName { get; set; }
        [DataMember]
        public LocationContext[] Contexts { get; set; }

        public bool IsInContext(ContextLocation location)
        {
            if (Contexts == null && Contexts.Length == 0)
                return false;
            else
                return Contexts.Any(ctxt => ctxt.IsInContext(location));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is FavoriteArrival)) return false;
            FavoriteArrival typedObj = (FavoriteArrival)obj;
            return typedObj.Route == Route && typedObj.Stop == Stop && typedObj.Destination == Destination;
        }

        public override int GetHashCode()
        {
            return (int)(Route.GetHashCode() / 3.0) + (int)(Stop.GetHashCode() / 3.0) + (int)(Destination.GetHashCode() / 3.0);
        }
    }

    [DataContract]
    public abstract class LocationContext
    {
        public abstract bool IsInContext(ContextLocation location);

        [DataMember]
        public ContextLocation RelativeLocation { get; set; }
    }

    [DataContract]
    public class CityContext : LocationContext
    {
        public override bool IsInContext(ContextLocation location)
        {
            return location.City == RelativeLocation.City;
        }
    }

    [DataContract]
    public class DistanceContext : LocationContext
    {
        [DataMember]
        public double Distance { get; set; }
        
        public override bool IsInContext(ContextLocation location)
        {
            return ContextLocation.Distance(RelativeLocation, location) <= Distance;
        }
    }

    [DataContract]
    public class CardninalDirectionContext : LocationContext
    {
        [DataMember]
        public CardinalDirection Direction { get; set; }

        public override bool IsInContext(ContextLocation location)
        {
            switch (Direction)
            {
                case CardinalDirection.North:
                    return location.Latitude >= RelativeLocation.Latitude;
                case CardinalDirection.South:
                    return location.Latitude <= RelativeLocation.Latitude;
                case CardinalDirection.East:
                    return location.Longitude >= RelativeLocation.Longitude;
                case CardinalDirection.West:
                    return location.Longitude <= RelativeLocation.Longitude;
            }
            return false;
        }
    }
}
