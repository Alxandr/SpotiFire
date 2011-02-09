using System;

namespace SpotiFire.SpotifyLib
{
    internal class DelegateList<T> : DelegateArray<T>, IEditableArray<T>
    {
        private Action<T> addFunc;
        private Action<int> removeFunc;
        private Func<bool> readonlyFunc;
        public DelegateList(Func<int> getLength, Func<int, T> getIndex, Action<T> addFunc, Action<int> removeFunc, Func<bool> readonlyFunc)
            : base(getLength, getIndex)
        {
            this.addFunc = addFunc;
            this.removeFunc = removeFunc;
            this.readonlyFunc = readonlyFunc;
        }

        public void Add(T item)
        {
            addFunc(item);
        }

        public void Clear()
        {
            while (Count > 0)
                removeFunc(0);
        }

        public bool Contains(T item)
        {
            foreach (T itm in this)
                if (itm.Equals(item))
                    return true;
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (Count > array.Length - arrayIndex)
                throw new ArgumentException("Array to small");

            int i = arrayIndex;
            foreach (T item in this)
                array[i++] = item;
        }

        public bool IsReadOnly
        {
            get
            {
                return readonlyFunc();
            }
        }

        public bool Remove(T item)
        {
            bool found = false;
            int i = 0, size = getLength();
            while (!found && i < size)
                if (!this[i].Equals(item))
                    i++;
                else
                    found = true;

            if (!found)
                return false;
            removeFunc(i);
            return true;
        }
    }
}
