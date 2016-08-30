using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    internal class StringPairComparer : IComparer<Tuple<string, string>>
    {
        public int Compare(Tuple<string, string> x, Tuple<string, string> y)
        {
            var result = x.Item1.CompareTo(y.Item1);
            if (result == 0)
                return x.Item2.CompareTo(y.Item2);
            return result;
        }
    }

    public class MemoryCache : IDisposable
    {
        private string ID;
        private static int NumCaches = 0;
        public MemoryCache(string id = null)
        {
            ID = id ?? (NumCaches++).ToString();
        }

        public void Add(params TransitStop[] stops)
        {
            foreach (var stop in stops)
            {
                var association = new Tuple<string, string>(ID, stop.ID);
                if (!PageAssociations.Contains((association)))
                {
                    PageAssociations.Add(association);
                    if (!StopCache.ContainsKey(stop.ID))
                        StopCache.Add(stop.ID, stop);
                }
            }
        }

        public IEnumerable<TransitStop> GetStops()
        {
            foreach (var key in PageAssociations.Where(ass => ass.Item1 == ID).Select(ass => ass.Item2))
            {
                yield return StopCache[key];
            }
        }

        public void Dispose()
        {
            PageAssociations.RemoveWhere(item => item.Item1 == ID);
            Clean();
        }

        private static void Clean()
        {
            var keys = StopCache.Keys.ToList();
            foreach (var item in PageAssociations)
                keys.Remove(item.Item2);
            foreach (var key in keys)
                StopCache.Remove(key);
        }

        private static SortedSet<Tuple<string, string>> PageAssociations = new SortedSet<Tuple<string, string>>(new StringPairComparer());

        private static Dictionary<string, TransitStop> StopCache = new Dictionary<string, TransitStop>();
        public static TransitStop? GetStop(string id)
        {
            if (StopCache.ContainsKey(id))
                return StopCache[id];
            return null;
        }
    }
}
