using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using Windows.Storage;
using System.IO;
using SQLite.Net.Attributes;
using Windows.UI.Popups;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseArchiver
{
    public static class Archiver
    {
        public static readonly SQLite.Net.Platform.WinRT.SQLitePlatformWinRT Platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
        public static readonly string DBPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.sqlite");

        public static async void Begin(SQLiteConnection connection)
        {
            connection.CreateTable<Agency>(SQLite.Net.Interop.CreateFlags.None);
            //var res = connection.Query<Test1>("SELECT * FROM Test1;");
            //string str = "";
            //foreach (Test1 tst in res)
            //{
            //    str += tst.ToString();
            ////}
            //MessageDialog dialog = new MessageDialog(str);
            //dialog.ShowAsync();
            foreach (var agency in await ApiLayer.GetTransitAgencies(new System.Threading.CancellationToken()))
            {
                connection.InsertOrReplace(agency);
            }
        }
    }
}
