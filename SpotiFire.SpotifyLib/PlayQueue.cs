using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpotiFire
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A spotify playqueue, mimicing the functionallity of the standard Play Queue in Spotify. </summary>
    ///
    /// <remarks>   Aleksander, 18.02.2013. </remarks>
    ///-------------------------------------------------------------------------------------------------
    public class PlayQueue : IEnumerable<Track>, ICollection, IEnumerable
    {
        static Track[] _empty = new Track[0];

        Track[] _backLog = _empty;
        int _backLogHead;
        int _backLogTail;
        int _backLogSize;

        Track[] _immidiate = _empty;
        int _immidiateHead;
        int _immidiateTail;
        int _immidiateSize;

        Track[] _queue = _empty;
        int _queueHead;
        int _queueTail;
        int _queueSize;

        int _backLogCapasity = 20;
        int _queueCapasity = 30;

        IEnumerable<Track> _seed;
        IEnumerator<Track> _currentSeed;

        bool _shuffle;
        bool _repeat;

        int _version;

        object _syncRoot;

        SubQueue _backLogQueue;
        SubQueue _immidiateQueue;
        SubQueue _queueQueue;

        public PlayQueue()
        {
            _backLogQueue = new SubQueue(this, EnumeratorType.BackLog);
            _immidiateQueue = new SubQueue(this, EnumeratorType.Immidiate);
            _queueQueue = new SubQueue(this, EnumeratorType.Queue);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets a value indicating whether to shuffle the tracks. </summary>
        ///
        /// <value> true if shuffle, false if not. </value>
        ///-------------------------------------------------------------------------------------------------
        public bool Shuffle
        {
            get { return _shuffle; }
            set { _shuffle = value; ResetQueue(); }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets a value indicating whether to repeat the tracks. </summary>
        ///
        /// <value> true if repeat, false if not. </value>
        ///-------------------------------------------------------------------------------------------------
        public bool Repeat
        {
            get { return _repeat; }
            set { _repeat = value; ResetQueue(); }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the seed. </summary>
        ///
        /// <value> The seed. </value>
        ///-------------------------------------------------------------------------------------------------
        public IEnumerable<Track> Seed
        {
            get { return _seed; }
            set { _seed = value; ResetQueue(); }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the number of elements contained in the <see cref="PlayQueue" />.
        /// </summary>
        ///
        /// <value>
        /// The number of elements contained in the <see cref="PlayQueue" />.
        /// </value>
        ///-------------------------------------------------------------------------------------------------
        public int Count
        {
            get
            {
                Populate();
                return _immidiateSize + _queueSize;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets a value indicating whether this queue is empty. </summary>
        ///
        /// <value> true if this object is empty, false if not. </value>
        ///-------------------------------------------------------------------------------------------------
        public bool IsEmpty
        {
            get
            {
                if (_immidiateSize + _queueSize == 0)
                    Populate(1);
                return _queueSize == 0;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the back log. </summary>
        ///
        /// <value> The back log. </value>
        ///-------------------------------------------------------------------------------------------------
        public SubQueue BackLog
        {
            get { return _backLogQueue; }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the immidiate. </summary>
        ///
        /// <value> The immidiate. </value>
        ///-------------------------------------------------------------------------------------------------
        public SubQueue Immidiate
        {
            get { return _immidiateQueue; }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the queue. </summary>
        ///
        /// <value> The queue. </value>
        ///-------------------------------------------------------------------------------------------------
        public SubQueue Queue
        {
            get { return _queueQueue; }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" />
        /// is synchronized (thread safe).
        /// </summary>
        ///
        /// <value>
        /// true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread
        /// safe); otherwise, false.
        /// </value>
        ///-------------------------------------------------------------------------------------------------
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets an object that can be used to synchronize access to the
        /// <see cref="T:System.Collections.ICollection" />.
        /// </summary>
        ///
        /// <value>
        /// An object that can be used to synchronize access to the
        /// <see cref="T:System.Collections.ICollection" />.
        /// </value>
        ///-------------------------------------------------------------------------------------------------
        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                return _syncRoot;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Clears this object to its blank/initial state. </summary>
        ///
        /// <remarks>   Aleksander, 18.02.2013. </remarks>
        ///-------------------------------------------------------------------------------------------------
        public void Clear()
        {
            ClearBackLog();
            ClearImmidiate();
            ClearQueue();

            _version += 1;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void ClearBackLog()
        {
            if (_backLogHead >= _backLogTail)
            {
                Array.Clear(_backLog, _backLogHead, _backLog.Length - _backLogHead);
                Array.Clear(_backLog, 0, _backLogTail);
            }
            else
            {
                Array.Clear(_backLog, 0, _backLogTail);
            }

            _backLogHead = 0;
            _backLogTail = 0;
            _backLogSize = 0;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void ClearImmidiate()
        {
            if (_immidiateHead >= _immidiateTail)
            {
                Array.Clear(_immidiate, _immidiateHead, _immidiate.Length - _immidiateHead);
                Array.Clear(_immidiate, 0, _immidiateTail);
            }
            else
            {
                Array.Clear(_immidiate, 0, _immidiateTail);
            }

            _immidiateHead = 0;
            _immidiateTail = 0;
            _immidiateSize = 0;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void ClearQueue()
        {
            if (_queueHead >= _queueTail)
            {
                Array.Clear(_queue, _queueHead, _queue.Length - _queueHead);
                Array.Clear(_queue, 0, _queueTail);
            }
            else
            {
                Array.Clear(_queue, 0, _queueTail);
            }

            _queueHead = 0;
            _queueTail = 0;
            _queueSize = 0;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Query if this object contains the given item. </summary>
        ///
        /// <remarks>   Aleksander, 18.02.2013. </remarks>
        ///
        /// <param name="item"> The Track to test for containment. </param>
        ///
        /// <returns>   true if the object is in this collection, false if not. </returns>
        ///-------------------------------------------------------------------------------------------------
        public bool Contains(Track item)
        {
            var eq = EqualityComparer<Track>.Default;
            throw new NotImplementedException("PlayQueue.Contains is yet to be implemented");
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Copies the elements of the <see cref="PlayQueue" /> to an
        /// <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        ///
        /// <remarks>   Aleksander, 18.02.2013. </remarks>
        ///
        /// <exception cref="ArgumentNullException">       Thrown when one or more required arguments
        /// are null. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside
        /// the required range. </exception>
        /// <exception cref="ArgumentException">           Thrown when one or more arguments have
        /// unsupported or illegal values. </exception>
        ///
        /// <param name="array">      The one-dimensional array that is the destination of the elements
        /// copied from <see cref="PlayQueue" />. The array must have zero-based indexing. </param>
        /// <param name="arrayIndex"> The zero-based index in <paramref name="array" /> at which copying
        /// begins. </param>
        ///-------------------------------------------------------------------------------------------------
        public void CopyTo(Track[] array, int arrayIndex)
        {
            CopyToInternal(array, arrayIndex);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an
        /// <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        ///
        /// <remarks>   Aleksander, 18.02.2013. </remarks>
        ///
        /// <exception cref="ArgumentNullException">       Thrown when one or more required arguments
        /// are null. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside
        /// the required range. </exception>
        /// <exception cref="ArgumentException">           Thrown when one or more arguments have
        /// unsupported or illegal values. </exception>
        ///
        /// <param name="array">      The one-dimensional array that is the destination of the elements
        /// copied from <see cref="PlayQueue" />. The array must have zero-based indexing. </param>
        /// <param name="arrayIndex"> The zero-based index in <paramref name="array" /> at which copying
        /// begins. </param>
        ///-------------------------------------------------------------------------------------------------
        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            CopyToInternal(array, arrayIndex);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        void CopyToInternal(Array array, int arrayIndex)
        {
            Populate();
            int size, length;

            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            int arrlength = array.Length;
            if (arrlength - arrayIndex < Count)
                throw new ArgumentException("Invalid offset and length");

            // backlog
            arrayIndex += CopyBackLogToInternal(array, arrayIndex);

            // Immidiate
            arrayIndex += CopyImmidiateToInternal(array, arrayIndex);

            // Queue
            arrayIndex += CopyQueueToInternal(array, arrayIndex);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        void CopyBackLogTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            CopyBackLogToInternal(array, arrayIndex);
        }
        
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        int CopyBackLogToInternal(Array array, int arrayIndex)
        {
            int arrlength = array.Length;
            if (arrlength - arrayIndex < _backLogSize)
                throw new ArgumentException("Invalid offset and length");

            int size = _backLogSize;
            if (size != 0)
            {
                int length = Math.Min(_backLog.Length - _backLogHead, size);
                Array.Copy(_backLog, _backLogHead, array, arrayIndex, length);
                if (size - length > 0)
                    Array.Copy(_backLog, 0, array, arrayIndex + length, size - length);
            }
            return size;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        void CopyImmidiateTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            CopyImmidiateToInternal(array, arrayIndex);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        int CopyImmidiateToInternal(Array array, int arrayIndex)
        {
            int arrlength = array.Length;
            if (arrlength - arrayIndex < _immidiateSize)
                throw new ArgumentException("Invalid offset and length");

            int size = _immidiateSize;
            if (size != 0)
            {
                int length = Math.Min(_immidiate.Length - _immidiateHead, size);
                Array.Copy(_immidiate, _immidiateHead, array, arrayIndex, length);
                if (size - length > 0)
                    Array.Copy(_immidiate, 0, array, arrayIndex + length, size - length);
            }
            return size;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        void CopyQueueTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            CopyQueueToInternal(array, arrayIndex);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        int CopyQueueToInternal(Array array, int arrayIndex)
        {
            int arrlength = array.Length;
            if (arrlength - arrayIndex < _queueSize)
                throw new ArgumentException("Invalid offset and length");

            int size = _queueSize;
            if (size != 0)
            {
                int length = Math.Min(_queue.Length - _queueHead, size);
                Array.Copy(_queue, _queueHead, array, arrayIndex, length);
                if (size - length > 0)
                    Array.Copy(_queue, 0, array, arrayIndex + length, size - length);
                arrayIndex += size;
            }
            return size;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Removes the head object from this queue. </summary>
        ///
        /// <remarks>   Aleksander, 18.02.2013. </remarks>
        ///
        /// <exception cref="InvalidOperationException"> The <see cref="PlayQueue" /> is empty. </exception>
        ///
        /// <returns>   The head object from this queue. </returns>
        ///-------------------------------------------------------------------------------------------------
        public Track Dequeue()
        {
            Track t;
            if (_immidiateSize != 0)
            {
                return Dequeue(ref _immidiate, ref _immidiateHead, ref _immidiateTail, ref _immidiateSize);
            }

            if (_queueSize == 0)
                Populate(1);
            var track = Dequeue(ref _queue, ref _queueHead, ref _queueTail, ref _queueSize);
            if (_backLogSize == _backLogCapasity)
                Dequeue(ref _backLog, ref _backLogHead, ref _backLogTail, ref _backLogSize);
            Enqueue(ref _backLog, ref _backLogHead, ref _backLogTail, ref _backLogSize, track);
            return track;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds an object onto the end of this queue. </summary>
        ///
        /// <remarks>   Aleksander, 18.02.2013. </remarks>
        ///
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        ///
        /// <param name="track">    The track. </param>
        ///-------------------------------------------------------------------------------------------------
        public void Enqueue(Track track)
        {
            Enqueue(ref _immidiate, ref _immidiateHead, ref _immidiateTail, ref _immidiateSize, track);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Returns the top-of-stack object without removing it. </summary>
        ///
        /// <remarks>   Aleksander, 18.02.2013. </remarks>
        ///
        /// <exception cref="InvalidOperationException"> The <see cref="PlayQueue" /> is empty. </exception>
        ///
        /// <returns>   The current top-of-stack object. </returns>
        ///-------------------------------------------------------------------------------------------------
        public Track Peek()
        {
            if (_immidiateSize != 0)
                return Peek(ref _immidiate, ref _immidiateHead, ref _immidiateTail, ref _immidiateSize);

            if (_queueSize == 0)
                Populate(1);
            return Peek(ref _queue, ref _queueHead, ref _queueTail, ref _queueSize);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Convert this object into an array representation. </summary>
        ///
        /// <remarks>   Aleksander, 18.02.2013. </remarks>
        ///
        /// <returns>   This object as a Track[]. </returns>
        ///-------------------------------------------------------------------------------------------------
        public Track[] ToArray()
        {
            Track[] tracks = new Track[Count];
            CopyTo(tracks, 0);
            return tracks;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Returns an enumerator that iterates through a collection. </summary>
        ///
        /// <remarks>   Aleksander, 18.02.2013. </remarks>
        ///
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator{Track}" /> object that can be used to iterate through
        /// the collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        public IEnumerator<Track> GetEnumerator()
        {
            return new Enumerator(this);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Returns an enumerator that iterates through a collection. </summary>
        ///
        /// <remarks>   Aleksander, 18.02.2013. </remarks>
        ///
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
        /// the collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        private void Enqueue(ref Track[] array, ref int head, ref int tail, ref int size, Track track)
        {
            if (track == null)
                throw new ArgumentNullException("track");

            if (size == array.Length)
            {
                int length = size * 2;
                if (length < size + 4)
                    length = size + 4;
                SetCapacity(ref array, ref head, ref tail, ref size, length);
            }
            array[tail] = track;
            tail = (tail + 1) % array.Length;
            size += 1;
            _version += 1;
        }

        private Track Dequeue(ref Track[] array, ref int head, ref int tail, ref int size)
        {
            if (size == 0)
                throw new InvalidOperationException("The queue is empty");

            Track t = array[head];
            array[head] = null;
            head = (head + 1) % array.Length;
            size -= 1;
            _version += 1;
            return t;
        }

        private Track Peek(ref Track[] array, ref int head, ref int tail, ref int size)
        {
            if (size == 0)
                throw new InvalidOperationException("The queue is empty");

            return array[head];
        }

        private void SetCapacity(ref Track[] array, ref int head, ref int tail, ref int size, int capacity)
        {
            Track[] newArr = new Track[capacity];
            if (size != 0)
            {
                if (head >= tail)
                {
                    Array.Copy(array, head, newArr, 0, array.Length - head);
                    Array.Copy(array, 0, newArr, array.Length - head, tail);
                }
                else
                {
                    Array.Copy(array, head, newArr, 0, size);
                }
            }
            array = newArr;
            head = 0;
            tail = size;
            _version += 1;
        }

        private void ResetQueue()
        {
            _currentSeed = null;
            ClearQueue();
        }

        private void Populate(int count = -1)
        {
            if (_seed == null)
                return;

            while (count != 0 && _queueSize < _queueCapasity)
            {
                if (_currentSeed == null)
                {
                    if (_shuffle)
                    {
                        Random r = new Random();
                        _currentSeed = _seed.ToList().OrderBy(i => r.NextDouble()).GetEnumerator();
                    }
                    else
                    {
                        _currentSeed = _seed.ToList().GetEnumerator();
                    }
                }

                if (_currentSeed.MoveNext())
                {
                    Enqueue(ref _queue, ref _queueHead, ref _queueTail, ref _queueSize, _currentSeed.Current);
                }
                else
                {
                    if (_repeat)
                    {
                        _currentSeed = null;
                    }
                    else
                    {
                        return;
                    }
                }

                count--;
            }
        }

        internal enum EnumeratorType
        {
            All,
            BackLog,
            Immidiate,
            Queue
        }

        public struct SubQueue : IEnumerable<Track>, ICollection, IEnumerable
        {
            PlayQueue _queue;
            EnumeratorType _type;

            public int Count
            {
                get
                {
                    switch (_type)
                    {
                        case EnumeratorType.BackLog: return _queue._backLogSize;
                        case EnumeratorType.Immidiate: return _queue._immidiateSize;
                        case EnumeratorType.Queue: return _queue._queueSize;
                        default: throw new InvalidOperationException();
                    }
                }
            }

            internal SubQueue(PlayQueue queue, EnumeratorType type)
            {
                _queue = queue;
                _type = type;
            }

            ///-------------------------------------------------------------------------------------------------
            /// <summary>
            /// Copies the elements of the <see cref="SubQueue" /> to an
            /// <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
            /// </summary>
            ///
            /// <remarks>   Aleksander, 18.02.2013. </remarks>
            ///
            /// <exception cref="ArgumentNullException">       Thrown when one or more required arguments
            /// are null. </exception>
            /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside
            /// the required range. </exception>
            /// <exception cref="ArgumentException">           Thrown when one or more arguments have
            /// unsupported or illegal values. </exception>
            ///
            /// <param name="array">      The one-dimensional array that is the destination of the elements
            /// copied from <see cref="PlayQueue" />. The array must have zero-based indexing. </param>
            /// <param name="arrayIndex"> The zero-based index in <paramref name="array" /> at which copying
            /// begins. </param>
            ///-------------------------------------------------------------------------------------------------
            public void CopyTo(Track[] array, int arrayIndex)
            {
                CopyToInternal(array, arrayIndex);
            }

            public IEnumerator<Track> GetEnumerator()
            {
                return new Enumerator(_queue, _type);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_queue, _type);
            }

            ///-------------------------------------------------------------------------------------------------
            /// <summary>
            /// Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an
            /// <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
            /// </summary>
            ///
            /// <remarks>   Aleksander, 18.02.2013. </remarks>
            ///
            /// <exception cref="ArgumentNullException">       Thrown when one or more required arguments
            /// are null. </exception>
            /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside
            /// the required range. </exception>
            /// <exception cref="ArgumentException">           Thrown when one or more arguments have
            /// unsupported or illegal values. </exception>
            ///
            /// <param name="array">      The one-dimensional array that is the destination of the elements
            /// copied from <see cref="PlayQueue" />. The array must have zero-based indexing. </param>
            /// <param name="arrayIndex"> The zero-based index in <paramref name="array" /> at which copying
            /// begins. </param>
            ///-------------------------------------------------------------------------------------------------
            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                CopyToInternal(array, arrayIndex);
            }


            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            void CopyToInternal(Array array, int index)
            {
                switch (_type)
                {
                    case EnumeratorType.BackLog: 
                        _queue.CopyBackLogTo(array, index);
                        break;

                    case EnumeratorType.Immidiate:
                        _queue.CopyImmidiateTo(array, index);
                        break;

                    case EnumeratorType.Queue:
                        _queue.CopyQueueTo(array, index);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            object ICollection.SyncRoot
            {
                get { return ((ICollection)_queue).SyncRoot; }
            }
        }

        public struct Enumerator : IEnumerator<Track>, IEnumerator, IDisposable
        {
            PlayQueue _queue;
            EnumeratorType _type;
            int _state;
            int _version;
            int _index;
            Track _current;

            public Track Current
            {
                get
                {
                    if (_index < 0)
                    {
                        if (_index != -1)
                            throw new InvalidOperationException("Enumeration ended");
                        else
                            throw new InvalidOperationException("Enumeration not started");
                    }
                    return _current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            internal Enumerator(PlayQueue queue, EnumeratorType type = EnumeratorType.All)
            {
                _queue = queue;
                _type = type;
                _state = 0;
                _index = 0;
                _version = _queue._version;
                _current = null;
            }

            public void Dispose()
            {
                _index = -2;
                _current = null;
            }

            public bool MoveNext()
            {
                if (_version != _queue._version)
                    throw new InvalidOperationException("Version has changed");

            start:
                if (_index != -2)
                {
                    _index += 1;
                    EnumeratorType type;
                    if (_type == EnumeratorType.All)
                    {
                        switch (_state)
                        {
                            case 0: // backlog
                                type = EnumeratorType.BackLog;
                                break;

                            case 1: // immidiate
                                type = EnumeratorType.Immidiate;
                                break;

                            case 2:
                                type = EnumeratorType.Queue;
                                break;

                            default:
                                throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        type = _type;
                    }

                    bool result;
                    switch (type)
                    {
                        case EnumeratorType.BackLog:
                            result = Fetch(_queue._backLog, _queue._backLogHead, _queue._backLogTail, _queue._backLogSize);
                            if (!result && _type == EnumeratorType.All)
                            {
                                _state += 1;
                                _index = -1;
                                goto start;
                            }
                            return result;

                        case EnumeratorType.Immidiate:
                            result = Fetch(_queue._immidiate, _queue._immidiateHead, _queue._immidiateTail, _queue._immidiateSize);
                            if (!result && _type == EnumeratorType.All)
                            {
                                _state += 1;
                                _index = -1;
                                goto start;
                            }
                            return result;

                        case EnumeratorType.Queue:
                            if (_queue._backLogSize == _index)
                                _queue.Populate(1);
                            result = Fetch(_queue._queue, _queue._queueHead, _queue._queueTail, _queue._queueSize);
                            return result;
                    }
                }
                return false;
            }

            void IEnumerator.Reset()
            {
                if(_version != _queue._version)
                    throw new InvalidOperationException("Version has changed");

                _index = -1;
                _state = 0;
                _current = null;
            }

            bool Fetch(Track[] array, int head, int tail, int size)
            {
                if (_index != size)
                {
                    // Fetch
                    _current = array[(head + _index) % array.Length];

                    // Return
                    return true;
                }
                _index = -2;
                _current = null;
                return false;
            }
        }
    }
}
