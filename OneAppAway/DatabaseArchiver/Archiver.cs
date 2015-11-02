using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using SQLitePCL;
using Windows.UI.Popups;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace DatabaseArchiver
{
    public static class Archiver
    {
        public static readonly SQLite.Net.Platform.WinRT.SQLitePlatformWinRT Platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
        public static readonly string DBPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

        public static async Task Begin(SQLiteConnection connection, CancellationToken token)
        {
            if (ExecuteSQL(connection, "select count(*) from Agency").ExtractInt(1) == 0)
            {
                foreach (var agency in await ApiLayer.GetTransitAgencies(token))
                {
                    ExecuteSQL(connection, string.Format($"insert or replace into Agency(AgencyID, Name, URL) values('{agency.AgencyID.Escape()}', '{agency.Name.Escape()}', '{agency.URL.Escape()}')"));
                }
            }

            foreach (string agencyID in ExecuteSQL(connection, "select AgencyID from PendingAgency"))
            {
                foreach (var route in await ApiLayer.GetBusRoutesForAgency(agencyID, token))
                {
                    ExecuteSQL(connection, string.Format($"insert or replace into Route(RouteID, Name, Description, AgencyID) values('{route.RouteID.Escape()}', '{route.Name.Escape()}', '{route.Description.Escape()}', '{route.AgencyID.Escape()}')"));
                }
                ExecuteSQL(connection, $"delete from PendingAgency where PendingAgency.AgencyID = '{agencyID.Escape()}'");
            }

            foreach (string routeID in ExecuteSQL(connection, "select RouteID from PendingRoute"))
            {
                var stopsAndShapes = await ApiLayer.GetStopIDsAndShapesForRoute(routeID, token);
                foreach (var shape in stopsAndShapes.Item2)
                    ExecuteSQL(connection, $"insert or replace into Shape(Points, Length) values('{shape.Item1.Escape()}', '{shape.Item2.ToString()}');");
                foreach (var stopID in stopsAndShapes.Item1)
                {
                    ExecuteSQL(connection, string.Format($"insert or replace into StopRouteLink(StopID, RouteID) values('{stopID.Escape()}', '{routeID.Escape()}')"));
                }
                ExecuteSQL(connection, $"delete from PendingRoute where PendingRoute.RouteID = '{routeID.Escape()}'");
            }

            foreach (string stopID in ExecuteSQL(connection, "select distinct StopID from PendingStopRouteLink"))
            {
                var stop = await ApiLayer.GetBusStop(stopID, token);
                ExecuteSQL(connection, $"insert or replace into Stop(StopID, Code, Name, Latitude, Longitude, Direction, LocationType) values('{stop[0].Escape()}', '{stop[1].Escape()}', '{stop[2].Escape()}', '{stop[3].Escape()}', '{stop[4].Escape()}', '{stop[5].Escape()}', '{stop[6].Escape()}')");
                ExecuteSQL(connection, $"delete from PendingStopRouteLink where PendingStopRouteLink.StopID = '{stopID.Escape()}'");
            }

            //foreach (var agency in await ApiLayer.GetTransitAgencies(token))
            //{
            //    ExecuteSQL(connection, string.Format($"insert or replace into Agency(AgencyID, Name, URL) values('{agency.AgencyID.Escape()}', '{agency.Name.Escape()}', '{agency.URL.Escape()}')"));
            //    foreach (var route in await ApiLayer.GetBusRoutesForAgency(agency.AgencyID, token))
            //    {
            //        ExecuteSQL(connection, string.Format($"insert or replace into Route(RouteID, Name, Description, AgencyID) values('{route.RouteID.Escape()}', '{route.Name.Escape()}', '{route.Description.Escape()}', '{route.AgencyID.Escape()}')"));
            //        var stopsAndShapes = await ApiLayer.GetStopIDsAndShapesForRoute(route.RouteID, token);
            //        foreach (var shape in stopsAndShapes.Item2)
            //            ExecuteSQL(connection, $"insert or replace into Shape(Points, Length) values('{shape.Item1.Escape()}', '{shape.Item2.ToString()}');");
            //        foreach (var stopID in stopsAndShapes.Item1)
            //        {
            //            if (ExecuteSQL(connection, $"select count(*) from Stop where StopID = '{stopID.Escape()}'").ExtractInt(0) == 0)
            //            {
            //                var fullStop = await ApiLayer.GetBusStop(stopID, token);
            //                ExecuteSQL(connection, $"insert or replace into Stop(StopID, Code, Name, Latitude, Longitude, Direction, LocationType) values('{fullStop[0].Escape()}', '{fullStop[1].Escape()}', '{fullStop[2].Escape()}', '{fullStop[3].Escape()}', '{fullStop[4].Escape()}', '{fullStop[5].Escape()}', '{fullStop[6].Escape()}')");
            //            }
            //            ExecuteSQL(connection, string.Format($"insert or replace into StopRouteLink(StopID, RouteID) values('{stopID.Escape()}', '{route.RouteID.Escape()}')"));
            //        }
            //    }
            //}
        }

        public static void EnsureTablesAndTriggers(SQLiteConnection connection)
        {
            #region Table Creation
            ExecuteSQL(connection, @"create table if not exists PendingAgency(AgencyID varchar(15) primary key)");
            ExecuteSQL(connection, @"create table if not exists PendingRoute(RouteID varchar(15) primary key)");
            ExecuteSQL(connection, @"create table if not exists PendingStop(StopID varchar(15) primary key)");
            ExecuteSQL(connection, @"create table if not exists PendingStopRouteLink(StopID varchar(15), RouteID varchar(15), primary key(StopID, RouteID))");
            ExecuteSQL(connection, @"create table if not exists Agency(
                                AgencyID varchar(15) primary key not null unique,
                                Name varchar(64),
                                URL varchar(140));");
            ExecuteSQL(connection, @"create table if not exists Neighborhood(
                                NeighborhoodID varchar(4) primary key not null unique,
                                Name varchar(50),
                                CityName varchar(50))");
            ExecuteSQL(connection, @"create table if not exists StopGroup(
                                StopGroupID varchar(4) primary key not null unique,
                                Name varchar(50))");
            ExecuteSQL(connection, @"create table if not exists Route(
                                RouteID varchar(15) primary key not null unique,
                                Name varchar(64),
                                Description varchar(255),
                                AgencyID varchar(15),
                                foreign key(AgencyID) references Agency(AgencyID))");
            ExecuteSQL(connection, @"create table if not exists Stop(
                                StopID varchar(15) primary key not null unique,
                                Code varchar(15) not null,
                                Name varchar(140) not null,
                                Latitude decimal(3,8) not null,
                                Longitude decimal(3,8) not null,
                                Direction tinyint,
                                LocationType tinyint,
                                NeighborhoodID varchar(4),
                                StopGroupID varchar(4),
                                foreign key(NeighborhoodID) references Neighborhood(NeighborhoodID),
                                foreign key(StopGroupID) references StopGroup(StopGroupID))");
            ExecuteSQL(connection, @"create table if not exists StopRouteLink(
                                StopID varchar(15) not null,
                                RouteID varchar(15) not null,
                                primary key(StopID, RouteID),
                                foreign key(StopID) references Stop(StopID),
                                foreign key(RouteID) references Route(RouteID))");
            ExecuteSQL(connection, @"create table if not exists Trip(
                                TripID varchar(20) not null primary key,
                                Destination varchar(80),
                                NextTripID varchar(20),
                                PrevTripID varchar(20),
                                RouteID varchar(15),
                                DirectionID tinyint,
                                foreign key(NextTripID) references Trip(TripID),
                                foreign key(PrevTripID) references Trip(TripID),
                                foreign key(RouteID) references Route(RouteID))");
            ExecuteSQL(connection, @"create table if not exists Arrival(
                                TripID varchar(20) not null,
                                StopID varchar(15) not null,
                                Hour tinyint not null,
                                Minute tinyint not null,
                                ServiceDays tinyint not null,
                                LayoverTime tinyint not null,
                                primary key(TripID, StopID, Hour, Minute, ServiceDays),
                                foreign key(TripID) references Trip(TripID),
                                foreign key(StopID) references Stop(StopID))");
            ExecuteSQL(connection, @"create table if not exists Shape(
                                Points varchar(50000) not null,
                                Length int not null,
                                RouteID varchar(15),
                                TripID varchar(15),
                                foreign key(RouteID) references Route(RouteID)
                                foreign key(TripID) references Trip(TripID))");
            #endregion

            #region Trigger Creation
            ExecuteSQL(connection, @"create trigger if not exists AddPendingAgency before insert on Agency begin
                                    insert or replace into PendingAgency values (new.AgencyID); end");
            ExecuteSQL(connection, @"create trigger if not exists AddPendingRoute before insert on Route begin
                                    insert or replace into PendingRoute values (new.RouteID); end");
            ExecuteSQL(connection, @"create trigger if not exists AddPendingStopRouteLink before insert on StopRouteLink begin
                                    insert or replace into PendingStopRouteLink values (new.StopID, new.RouteID); end");
            ExecuteSQL(connection, @"create trigger if not exists AddPendingStop before insert on Stop begin
                                    insert or replace into PendingStop values (new.StopID); end");
            #endregion
        }

        public static SQLiteConnection GetConnection()
        {
            SQLiteConnection result = new SQLiteConnection(DBPath);
            EnsureTablesAndTriggers(result);
            return result;
        }

        public static string[,] ExecuteSQL(SQLiteConnection conn, string line, out string[] columns, out int numRows)
        {
            List<string[]> rows = new List<string[]>();
            using (var statement = conn.Prepare(line + ";"))
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

                    while(statement.Step() != SQLiteResult.DONE)
                    {
                        string[] row = new string[statement.ColumnCount];
                        for (int i = 0; i < statement.ColumnCount; i++)
                        {
                            row[i] = statement[i]?.ToString()?.UnEscape();
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

        public static string Escape(this string str, params char[] ignore)
        {
            if (str == null) return null;
            string result = str;
            for (int i = 0; i < EscapableChars.Length; i++)
            {
                if (!ignore.Contains(EscapableChars[i]))
                    result = result.Replace(EscapableChars[i].ToString(), EscapableChars[0].ToString() + EscapeIndices[i].ToString());
            }
            return result;
        }

        public static string UnEscape(this string str)
        {
            string result = str;
            for (int i = 0; i < EscapableChars.Length; i++)
            {
                result = result.Replace(EscapableChars[0].ToString() + EscapeIndices[i].ToString(), EscapableChars[i].ToString());
            }
            return result;
        }

        public static string EscapableChars = "/!~'_%^|()[]{}`@#?";
        public static string EscapeIndices = "0123456789abcdefghijklmnopqrstuvwxyz";

        public static int ExtractInt(this object[,] sqlResult, int def)
        {
            if (sqlResult.GetLength(0) != 1 || sqlResult.GetLength(1) != 1)
                return def;
            int res;
            if (int.TryParse(sqlResult[0, 0].ToString(), out res))
                return res;
            else
                return def;
        }
    }
}
