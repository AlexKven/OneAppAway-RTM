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
        private static SQLite.Net.Platform.WinRT.SQLitePlatformWinRT Platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
        private static string DBPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.sqlite");

        public static void Begin()
        {
            using (var connection = new SQLiteConnection(Platform, DBPath))
            {
                connection.CreateTable<Test1>(SQLite.Net.Interop.CreateFlags.None);
                connection.CreateTable<Test2>(SQLite.Net.Interop.CreateFlags.None);
                connection.Insert(new Test1() { Name = "Alex" });
                connection.Insert(new Test1() { Name = "Nik" });
                connection.Insert(new Test2());
                var res = connection.Query<Test1>("SELECT * FROM Test1;");
                string str = "";
                foreach (Test1 tst in res)
                {
                    str += tst.ToString();
                }
                MessageDialog dialog = new MessageDialog(str);
                dialog.ShowAsync();
            }
        }
    }

    public class Test1
    {
        [PrimaryKey][AutoIncrement]
        public int Key { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Key.ToString() + ", " + Name;
        }
    }

    public class Test2
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Key { get; set; }

        [ForeignKey("Test1.Key")]
        public int FK { get; set; }
    }
}
