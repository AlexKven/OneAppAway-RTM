using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OneAppAway._1_1.Helpers
{
    public class BindingHelperCollection : BindingHelper, IList<BindingHelper>
    {
        private List<BindingHelper> InternalList = new List<BindingHelper>();

        public BindingHelper this[int index]
        {
            get
            {
                return InternalList[index];
            }

            set
            {
                FrameworkElement element = null;
                Element?.TryGetTarget(out element);
                if (element != null)
                    InternalList[index].Deregister();
                InternalList[index] = value;
                if (element != null)
                    InternalList[index].Register();
            }
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Add(BindingHelper item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(BindingHelper item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(BindingHelper[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<BindingHelper> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(BindingHelper item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, BindingHelper item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(BindingHelper item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        internal override void Deregister()
        {
            throw new NotImplementedException();
        }

        internal override void Register()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
