using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace OneAppAway
{
    public class Correction
    {
        public static ScheduledArrival Correct(ScheduledArrival arrival)
        {
            //List<Tuple<string, string, string>> corrections = new List<Tuple<string, string, string>>()
            //{
            //    new Tuple<string, string, string>("19_21", "19_1376", "To Olympia"),
            //    new Tuple<string, string, string>("19_21", "19_1377", "To Olympia"),
            //    new Tuple<string, string, string>("19_21", "19_1378", "To Olympia"),
            //    new Tuple<string, string, string>("19_21", "19_1379", "To Olympia"),
            //    new Tuple<string, string, string>("19_21", "19_2399", "To Olympia"),
            //    new Tuple<string, string, string>("19_21", "19_1380", "To Olympia"),
            //    new Tuple<string, string, string>("19_21", "19_1381", "To Olympia"),
            //    new Tuple<string, string, string>("19_21", "19_1382", "To Olympia"),
            //    new Tuple<string, string, string>("19_38", "19_1391", "To Olympia via Lacey TC"),
            //    new Tuple<string, string, string>("19_37", "19_1383", "To Olympia via Lacey TC"),
            //    new Tuple<string, string, string>("19_37", "19_1384", "To Olympia via Lacey TC"),
            //    new Tuple<string, string, string>("19_37", "19_1385", "To Olympia via Lacey TC"),
            //    new Tuple<string, string, string>("19_37", "19_1386", "To Olympia via Lacey TC"),
            //    new Tuple<string, string, string>("19_37", "19_2274", "To Olympia via Lacey TC"),
            //    new Tuple<string, string, string>("19_37", "19_1387", "To Olympia via Lacey TC"),
            //    new Tuple<string, string, string>("19_37", "19_1389", "To Olympia via Lacey TC"),
            //    new Tuple<string, string, string>("19_37", "19_1390", "To Olympia via Lacey TC"),
            //    new Tuple<string, string, string>("19_37", "19_2401", "To Olympia via Lacey TC"),
            //    new Tuple<string, string, string>("19_37", "19_1388", "To Olympia via Lacey TC"),
            //    new Tuple<string, string, string>("19_39", "19_2412", "To Tumwater via Olympia"),
            //    new Tuple<string, string, string>("19_39", "19_2413", "To Tumwater via Olympia"),
            //    new Tuple<string, string, string>("19_39", "19_2414", "To Tumwater via Olympia"),
            //    new Tuple<string, string, string>("19_39", "19_2415", "To Tumwater via Olympia"),
            //    new Tuple<string, string, string>("19_39", "19_2416", "To Tumwater via Olympia"),
            //    new Tuple<string, string, string>("19_39", "19_2417", "To Tumwater via Olympia"),
            //    new Tuple<string, string, string>("19_39", "19_2418", "To Tumwater via Olympia"),
            //    new Tuple<string, string, string>("19_39", "19_2419", "To Tumwater via Olympia"),
            //    new Tuple<string, string, string>("19_39", "19_2410", "To Tumwater via Olympia"),
            //    new Tuple<string, string, string>("19_39", "19_2421", "To Tumwater via Olympia"),
            //    new Tuple<string, string, string>("19_39", "19_2422", "To Tumwater via Olympia")
            //};
            //foreach (var correction in corrections)
            //{
            //    if (correction.Item1 == arrival.Route && correction.Item2 == arrival.Trip)
            //        return new ScheduledArrival() { Destination = correction.Item3, Route = arrival.Route, Trip = arrival.Trip, ScheduledDepartureTime = arrival.ScheduledDepartureTime, Stop = arrival.Stop };
            //}
            return arrival;
        }
    }
}
