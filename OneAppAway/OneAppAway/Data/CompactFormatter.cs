using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace OneAppAway
{
    public class CompactFormatReader
    {
        private string Data;
        private int Index = 0;

        public CompactFormatReader(string data)
        {
            Data = data;
        }

        public CompactFormatReader[] Next()
        {
            List<CompactFormatReader> result = new List<CompactFormatReader>();
            StringBuilder data = new StringBuilder();
            int level = 0;
            bool escaped = false;
            bool end = false;
            //bool indented = false;
            if (Index >= Data.Length || Data[Index] == ')')
                return null;
            //if (Data[Index] == '(')
            //{
            //    Index++;
            //    indented = true;
            //}
            while (!end && Index < Data.Length)
            {
                char nextChar = Data[Index];
                if (level == 0 && "|)".Contains(nextChar) && !escaped)
                    end = true;
                else
                {
                    if (nextChar == '(' && !escaped)
                    {
                        if (level == 0)
                        {
                            result.Add(new CompactFormatReader(data.ToString()));
                            data = new StringBuilder();
                            //Bad practice
                            escaped = false;
                            Index++;
                            level++;
                            continue;
                        }
                        level++;
                    }
                    else if (nextChar == ')' && !escaped)
                    {
                        level--;
                    }
                    if (nextChar == ',' && level == 0 && !escaped)
                    {
                        result.Add(new CompactFormatReader(data.ToString()));
                        data = new StringBuilder();
                    }
                    else if (nextChar == '|' && level == 0 && !escaped)
                    {
                        end = true;
                    }
                    else
                        data.Append(nextChar);
                }
                escaped = nextChar == '\\';
                Index++;
            }
            result.Add(new CompactFormatReader(data.ToString()));
            //if (indented)
            //    Index++;
            return result.ToArray();
        }

        public string ReadString()
        {
            if (Data[0] == '"' && Data.Length > 2)
            {
                StringBuilder result = new StringBuilder();
                int ind = 1;
                bool end = (Data[1] == '"');
                bool escaped = false;
                while (!end && ind < Data.Length)
                {
                    if (Data[ind] == '\\' && !escaped)
                        escaped = true;
                    else if (Data[ind] == '"' && !escaped)
                        end = true;
                    else
                    {
                        result.Append(Data[ind]);
                        escaped = false;
                    }
                    ind++;
                }
                return result.ToString();
            }
            else
            {
                return Data;
            }
        }

        public int ReadInt()
        {
            return int.Parse(ReadString());
        }
    }

    public class CompactFormatWriter
    {
        private StringBuilder Builder = new StringBuilder();
        private bool DelimiterNeeded = false;
        bool DelimiterJustPlaced = false;
        private string SpecialChars = "\"\\,()|";

        public void WriteString(string str)
        {
            if (DelimiterNeeded)
                Builder.Append(',');
            Builder.Append(str);
            DelimiterNeeded = true;
            DelimiterJustPlaced = false;
        }

        public void WriteInt(int num)
        {
            WriteString(num.ToString());
        }

        public void WriteQuotedString(string str)
        {
            StringBuilder newStringBuilder = new StringBuilder("\"");
            foreach (char chr in str)
            {
                if (SpecialChars.Contains(chr))
                    newStringBuilder.Append("\\");
                newStringBuilder.Append(chr);
            }
            newStringBuilder.Append("\"");
            WriteString(newStringBuilder.ToString());
        }

        public void NextItem()
        {
            TrimDelimiter();
            Builder.Append('|');
            DelimiterNeeded = false;
            DelimiterJustPlaced = true;
        }

        public void OpenParens()
        {
            Builder.Append("(");
            DelimiterNeeded = false;
        }

        public void  CloseParens()
        {
            TrimDelimiter();
            Builder.Append(")");
            DelimiterNeeded = false;
        }

        public void TrimDelimiter()
        {
            if (DelimiterJustPlaced)
                Builder.Remove(Builder.Length - 1, 1);
            DelimiterJustPlaced = false;
        }

        public override string ToString()
        {
            if (DelimiterJustPlaced)
                return Builder.ToString().Substring(0, Builder.Length - 1);
            else
                return Builder.ToString();
        }
    }

    public static class CompactFormatter
    {
        public static void FormatStops(List<BusStop> stops, CompactFormatWriter writer)
        {
            for (int i = 0; i < stops.Count; i++)
            {
                var stop = stops[i];
                writer.WriteString(stop.ID);
                writer.WriteInt((int)stop.Direction);
                writer.WriteString(stop.Position.Latitude.ToString());
                writer.WriteString(stop.Position.Longitude.ToString());
                writer.WriteQuotedString(stop.Name);
                writer.WriteString(stop.Code);
                writer.WriteInt(stop.LocationType);
                writer.OpenParens();
                foreach (var route in stop.Routes)
                {
                    writer.WriteString(route);
                    writer.NextItem();
                }
                writer.CloseParens();
                writer.NextItem();
            }
        }

        public static List<BusStop> DeformatStops(CompactFormatReader reader)
        {
            List<BusStop> result = new List<BusStop>();
            CompactFormatReader[] stopReader;
            List<string> routes;
            while ((stopReader = reader.Next()) != null)
            {
                BusStop stop = new BusStop();
                stop.ID = stopReader[0].ReadString();
                stop.Direction = (StopDirection)stopReader[1].ReadInt();
                stop.Position = new BasicGeoposition() { Latitude = double.Parse(stopReader[2].ReadString()), Longitude = double.Parse(stopReader[3].ReadString()) };
                stop.Name = stopReader[4].ReadString();
                stop.Code = stopReader[5].ReadString();
                stop.LocationType = stopReader[6].ReadInt();
                routes = new List<string>();
                CompactFormatReader[] routeReader;
                while ((routeReader = stopReader[7].Next()) != null)
                {
                    routes.Add(routeReader[0].ReadString());
                }
                stop.Routes = routes.ToArray();
                result.Add(stop);
            }
            return result;
        }

        public static void FormatRoutes(List<Tuple<BusRoute, string[], string[]>> routes, CompactFormatWriter writer)
        {
            for (int i = 0; i < routes.Count; i++)
            {
                var route = routes[i];
                writer.WriteString(route.Item1.ID);
                writer.WriteQuotedString(route.Item1.Name);
                writer.WriteQuotedString(route.Item1.Description);
                writer.WriteString(route.Item1.Agency);
                writer.OpenParens();
                foreach (var stop in route.Item2)
                {
                    writer.WriteString(stop);
                    writer.NextItem();
                }
                writer.CloseParens();
                writer.OpenParens();
                foreach (var shape in route.Item3)
                {
                    writer.WriteQuotedString(shape);
                    writer.NextItem();
                }
                writer.CloseParens();
                writer.NextItem();
            }
        }

        public static List<Tuple<BusRoute, string[], string[]>> DeformatRoutes(CompactFormatReader reader)
        {
            List<Tuple<BusRoute, string[], string[]>> result = new List<Tuple<BusRoute, string[], string[]>>();
            CompactFormatReader[] routeReader;
            List<string> stops;
            List<string> shapes;
            while ((routeReader = reader.Next()) != null)
            {
                BusRoute route = new BusRoute();
                route.ID = routeReader[0].ReadString();
                route.Name = routeReader[1].ReadString();
                route.Description = routeReader[2].ReadString();
                route.Agency = routeReader[3].ReadString();
                stops = new List<string>();
                shapes = new List<string>();
                CompactFormatReader[] subReader;
                while ((subReader = routeReader[4].Next()) != null)
                {
                    stops.Add(subReader[0].ReadString());
                }
                while ((subReader = routeReader[5].Next()) != null)
                {
                    shapes.Add(subReader[0].ReadString());
                }
                result.Add(new Tuple<BusRoute, string[], string[]>(route, stops.ToArray(), shapes.ToArray()));
            }
            return result;
        }

        public static void FormatAgencies(List<Tuple<TransitAgency, string[]>> agencies, CompactFormatWriter writer)
        {
            for (int i = 0; i < agencies.Count; i++)
            {
                var agency = agencies[i];
                writer.WriteString(agency.Item1.ID);
                writer.WriteQuotedString(agency.Item1.Name);
                writer.WriteQuotedString(agency.Item1.Url);
                writer.OpenParens();
                foreach (var route in agency.Item2)
                {
                    writer.WriteString(route);
                    writer.NextItem();
                }
                writer.CloseParens();
                writer.NextItem();
            }
        }

        public static List<Tuple<TransitAgency, string[]>> DeformatAgencies(CompactFormatReader reader)
        {
            List<Tuple<TransitAgency, string[]>> result = new List<Tuple<TransitAgency, string[]>>();
            CompactFormatReader[] agencyReader;
            List<string> routes;
            while ((agencyReader = reader.Next()) != null)
            {
                TransitAgency agency = new TransitAgency();
                agency.ID = agencyReader[0].ReadString();
                agency.Name = agencyReader[1].ReadString();
                agency.Url = agencyReader[2].ReadString();
                routes = new List<string>();
                CompactFormatReader[] routeReader;
                while ((routeReader = agencyReader[3].Next()) != null)
                {
                    routes.Add(routeReader[0].ReadString());
                }
                result.Add(new Tuple<TransitAgency, string[]>(agency, routes.ToArray()));
            }
            return result;
        }
    }
}
