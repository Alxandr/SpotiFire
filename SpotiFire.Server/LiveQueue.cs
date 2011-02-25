using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using SpotiFire.SpotifyLib;

namespace SpotiFire.Server
{
    [Serializable, ComVisible(false), DebuggerDisplay("Count = {Count}")]
    public class LiveQueue<T> : IEnumerable<T>, ICollection, IEnumerable
    {
        [NonSerialized]
        private IArray<T> feed = null;
        private int index = 0;
        private bool randomOrder = false;
        private bool repeat = false;

        private Queue<T> backlog = new Queue<T>();
        private Queue<T> queue = new Queue<T>();
        private Queue<T> customQueue = new Queue<T>();

        private int queueLength = 50;
        private int backlogLength = 20;

        [NonSerialized]
        private List<int> remaining = new List<int>();

        [NonSerialized]
        private Random random = new Random();

        [NonSerialized]
        private object syncRoot = null;
        private int version = 0;

        public IArray<T> Feed
        {
            set
            {
                feed = value;
                RebuildList(true, true);
            }
        }

        public int Index
        {
            set
            {
                index = value;
                RebuildList(true, true);
            }
        }

        public LiveQueue(IArray<T> feed = null)
        {
            Feed = feed;
        }

        private void RebuildList(bool removeQueue, bool removeCustomQueue)
        {
            version++;
            if (removeQueue)
            {
                queue.Clear();
                backlog.Clear();
                remaining.Clear();
            }

            if (removeCustomQueue)
                customQueue.Clear();

            if (feed == null)
                return;

            bool first = true;
            do
            {
                if (randomOrder)
                {
                    FeedArray(remaining, 0, feed.Count);
                    if (first)
                    {
                        remaining.Remove(index);
                        first = false;
                    }
                    while (remaining.Count > 0 && queue.Count < queueLength)
                    {
                        int nextIndex = random.Next(remaining.Count);
                        queue.Enqueue(feed[remaining[nextIndex]]);
                        remaining.RemoveAt(nextIndex);
                    }
                }
                else
                {
                    for (int i = index + 1, l = feed.Count; i < l; i++)
                        if (queue.Count < queueLength)
                            queue.Enqueue(feed[i]);
                        else
                        {
                            FeedArray(remaining, i, feed.Count);
                            break;
                        }
                }
            } while (repeat && queue.Count < queueLength);
        }

        private void FeedArray(List<int> list, int start, int stop)
        {
            for (int i = start; i < stop; i++)
                list.Add(i);
        }

        private void FeedItems()
        {
            version++;
            if (!repeat && remaining.Count == 0)
                return;

            while (queue.Count < queueLength && (repeat || remaining.Count > 0))
            {
                if (repeat && remaining.Count == 0)
                    FeedArray(remaining, 0, feed.Count);

                int index = randomOrder ? random.Next(remaining.Count) : 0;
                queue.Enqueue(feed[remaining[index]]);
                remaining.RemoveAt(index);
            }

            while (backlog.Count > backlogLength)
                backlog.Dequeue();
        }

        public void Clear()
        {
            version++;
            customQueue.Clear();
        }

        public bool Contains(T item)
        {
            return backlog.Contains(item) || queue.Contains(item) || customQueue.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            int length = backlog.Count + customQueue.Count + queue.Count;
            if (length - arrayIndex < this.Count)
                throw new ArgumentException("Invalid length");

            foreach (T t in this)
                array[arrayIndex++] = t;
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            int length = backlog.Count + customQueue.Count + queue.Count;
            if (length - arrayIndex < this.Count)
                throw new ArgumentException("Invalid length");

            foreach (T t in this)
                array.SetValue(t, arrayIndex++);
        }

        public T Dequeue()
        {
            T ret;
            if (customQueue.Count > 0)
                ret = customQueue.Dequeue();
            else
                ret = queue.Dequeue();
            backlog.Enqueue(ret);
            FeedItems();
            return ret;
        }

        public void Enqueue(T item)
        {
            customQueue.Enqueue(item);
        }

        internal T GetItem(int i)
        {
            if (i < backlog.Count)
                return backlog.ToArray()[i];
            i -= backlog.Count;

            if (i < customQueue.Count)
                return customQueue.ToArray()[i];
            i -= customQueue.Count;

            return queue.ToArray()[i];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public T[] ToArray()
        {
            T[] ret = new T[Count];
            CopyTo(ret, 0);
            return ret;
        }

        public void TrimExcess()
        {
            queue.TrimExcess();
            customQueue.TrimExcess();
            backlog.TrimExcess();
        }

        public int Count
        {
            get
            {
                return customQueue.Count + queue.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (this.syncRoot == null)
                    Interlocked.CompareExchange<object>(ref this.syncRoot, new object(), null);
                return this.syncRoot;
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private LiveQueue<T> q;
            private Queue<T>.Enumerator currentEnum;
            private int currentArr;

            internal Enumerator(LiveQueue<T> q)
            {
                this.q = q;
                currentEnum = default(Queue<T>.Enumerator);
                currentArr = -1;
            }

            public void Dispose()
            {
                this.currentArr = -2;
                this.currentEnum = default(Queue<T>.Enumerator);
            }

            public bool MoveNext()
            {
                if (currentArr == -2)
                    return false;

                bool inThis = currentEnum.MoveNext();
                if (!inThis)
                    currentArr++;
                else
                    return true;

                currentEnum.Dispose();

                switch (currentArr)
                {
                    case 0:
                        currentEnum = q.backlog.GetEnumerator();
                        break;
                    case 1:
                        currentEnum = q.customQueue.GetEnumerator();
                        break;
                    case 2:
                        currentEnum = q.queue.GetEnumerator();
                        break;
                    default:
                        return false;
                }
                return MoveNext();
            }

            public T Current
            {
                get
                {
                    return currentEnum.Current;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Reset()
            {
                currentEnum.Dispose();
                currentArr = -1;
                currentEnum = default(Queue<T>.Enumerator);
            }
        }
    }
}
