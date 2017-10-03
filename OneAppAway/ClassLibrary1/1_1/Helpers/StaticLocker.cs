using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Helpers
{
    public static class StaticLocker
    {
        private static List<string> Locks = new List<string>();

        public static async Task Lock(string id)
        {
            bool locked = true;
            while (locked)
            {
                lock (Locks)
                {
                    locked = Locks.Contains(id);
                    if (!locked)
                        Locks.Add(id);
                }
                if (locked)
                    await Task.Delay(100);
            }
        }

        public static void Unlock(string id)
        {
            lock (Locks)
            {
                Locks.Remove(id);
            }
        }
    }
}
