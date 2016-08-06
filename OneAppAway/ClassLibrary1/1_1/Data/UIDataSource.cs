using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneAppAway._1_1.Data
{
    public abstract class UIDataSource
    {
        #region Instance
        public event EventHandler DataChanged;
        public event EventHandler Decommissioned;

        protected virtual void OnDataChanged()
        {
            DataChanged?.Invoke(this, new EventArgs());
        }

        public void Decommission()
        {
            Decommissioned?.Invoke(this, new EventArgs());
        }
        #endregion

        #region Static
        private static List<Tuple<object, object>> RegisteredViews = new List<Tuple<object, object>>();
        private static Dictionary<object, List<UIDataSource>> RegisterDataSources = new Dictionary<object, List<UIDataSource>>();

        public static void RegisterView(object view, object parentView)
        {
            if (view == null)
                throw new ArgumentException("Value cannot be null.", "view");
            if (parentView != null && !RegisterDataSources.ContainsKey(parentView))
                throw new ArgumentOutOfRangeException("parentView", "The specified parent view is not registered.");
            if (RegisterDataSources.ContainsKey(view))
                throw new ArgumentException("View is already registered.", "view");
            RegisteredViews.Add(new Tuple<object, object>(view, parentView));
            RegisterDataSources.Add(view, new List<UIDataSource>());
        }

        public static void RegisterView(object view)
        {
            RegisterView(view, null);
        }

        public static void RegisterDataSource(UIDataSource source, object view)
        {
            if (!RegisterDataSources.ContainsKey(view))
                throw new ArgumentOutOfRangeException("view", "View is not registered.");
            var collection = RegisterDataSources[view];
            if (collection.Contains(view))
                throw new ArgumentException("The data source is already registered.", "source");
            collection.Add(source);
        }

        public static void RemoveView(object view)
        {
            var children = RegisteredViews.Where(itm => itm.Item2 == view).Select(tpl => tpl.Item1);
            foreach (var child in children)
                RemoveView(child);
            var collection = RegisterDataSources[view];
            foreach (var source in collection)
            {
                source.Decommission();
            }
            collection.Clear();
            RegisterDataSources.Remove(view);
            RegisteredViews.RemoveAll(itm => itm.Item1 == view);
        }
        #endregion
    }
}