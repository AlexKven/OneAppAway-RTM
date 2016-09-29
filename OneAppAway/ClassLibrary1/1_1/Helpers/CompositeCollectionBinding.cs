using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Helpers
{
    public class CompositeCollectionBinding<T, TKey>
    {
        private class Partition
        {
            public TKey Key { get; set; }
            public int Size { get; set; }
        }

        private IList<T> BoundList;
        private Dictionary<TKey, ObservableCollection<T>> CollectionsDictionary = new Dictionary<TKey, ObservableCollection<T>>();
        private List<Partition> CollectionPartitions = new List<Partition>();
        private Dictionary<TKey, WeakEventListener<CompositeCollectionBinding<T, TKey>, object, NotifyCollectionChangedEventArgs>> Listeners = new Dictionary<TKey, WeakEventListener<CompositeCollectionBinding<T, TKey>, object, NotifyCollectionChangedEventArgs>>();

        public CompositeCollectionBinding(IList<T> boundList)
        {
            BoundList = boundList;
            BoundList.Clear();
        }

        private Tuple<Partition, int> GetPartition(TKey key)
        {
            int partitionStart = 0;
            int partitionSize = 0;
            Partition currentPartition;
            int partitionIndex = -1;
            do
            {
                partitionStart += partitionSize;
                currentPartition = CollectionPartitions[++partitionIndex];
                partitionSize = currentPartition.Size;
            } while (!currentPartition.Key?.Equals(key) ?? false);
            return new Tuple<Partition, int>(currentPartition, partitionStart);
        }

        private void InsertItems(TKey key, int index, params T[] items)
        {
            var part = GetPartition(key);
            for (int i = 0; i < items.Length; i++)
            {
                BoundList.Insert(part.Item2 + index + i, items[i]);
            }
            part.Item1.Size += items.Length;
        }

        private void ReplaceItems(TKey key, int index, params T[] items)
        {
            var part = GetPartition(key);
            for (int i = 0; i < items.Length; i++)
            {
                BoundList[part.Item2 + index + i] = items[i];
            }
        }

        private void ClearItems(TKey key)
        {
            var part = GetPartition(key);
            for (int i = 0; i < part.Item1.Size; i++)
            {
                BoundList.RemoveAt(part.Item2 + part.Item2);
            }
            part.Item1.Size = 0;
        }

        private void RemoveItems(TKey key, int index, int numItems)
        {
            var part = GetPartition(key);
            for (int i = 0; i < numItems; i++)
            {
                BoundList.RemoveAt(part.Item2 + index);
            }
            part.Item1.Size -= numItems;
        }

        public void AddCollection(TKey key, ObservableCollection<T> collection)
        {
            CollectionsDictionary.Add(key, collection);
            Partition partition = new Partition() { Key = key, Size = 0 };
            CollectionPartitions.Add(partition);
            InsertItems(key, 0, collection.ToArray());

            WeakEventListener<CompositeCollectionBinding<T, TKey>, object, NotifyCollectionChangedEventArgs> listener = new WeakEventListener<CompositeCollectionBinding<T, TKey>, object, NotifyCollectionChangedEventArgs>(this);
            listener.OnEventAction = (_this, sender, e) => _this.Collection_CollectionChanged(sender, e);
            collection.CollectionChanged += listener.OnEvent;
            Listeners.Add(key, listener);
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var key = CollectionsDictionary.First(item => item.Value == sender).Key;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    InsertItems(key, e.NewStartingIndex, e.NewItems.Cast<T>().ToArray());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(key, e.OldStartingIndex, e.OldItems.Count);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ReplaceItems(key, e.NewStartingIndex, e.NewItems.Cast<T>().ToArray());
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();
                case NotifyCollectionChangedAction.Reset:
                    ClearItems(key);
                    InsertItems(key, 0, ((ObservableCollection<T>)sender).ToArray());
                    break;
            }
        }

        public void RemoveCollection(TKey key)
        {
            ClearItems(key);
            CollectionPartitions.RemoveAll(p => p.Key?.Equals(key) ?? false);
            CollectionsDictionary.Remove(key);
            Listeners[key].Detach();
            Listeners.Remove(key);
        }
    }
}
