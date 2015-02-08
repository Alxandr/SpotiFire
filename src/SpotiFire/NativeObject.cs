using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Serilog;

namespace SpotiFire
{
    public abstract class NativeObject : IDisposable
    {
        readonly IntPtr _nativePtr;
        readonly ILogger _logger;
        readonly List<IntPtr> _simplePointers = new List<IntPtr>();
        readonly object _lock = new object();
        bool _freed = false;

        internal IntPtr NativePtr
        {
            get
            {
                lock (_lock)
                {
                    if (_freed)
                        throw new ObjectDisposedException($"${nameof(NativeObject)} has been freed");
                }

                return _nativePtr;
            }
        }

        internal ILogger Log => _logger;

        public NativeObject(IntPtr nativePtr, ILogger logger)
        {
            _nativePtr = nativePtr;
            _logger = logger.ForContext(GetType());
            _logger.Debug("Native Object {ptr} created", _nativePtr);
        }

        ~NativeObject()
        {
            Dispose(false);
        }

        internal IntPtr RegisterDependent(IntPtr hGlobalPtr)
        {
            lock (_lock)
            {
                if (_freed)
                    throw new ObjectDisposedException($"${nameof(NativeObject)} has been freed");

                _simplePointers.Add(hGlobalPtr);
            }

            return hGlobalPtr;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            bool free = false;
            IntPtr[] toFree = null;
            lock (_lock)
            {
                if (!_freed)
                {
                    free = true;
                    _freed = true;
                    toFree = _simplePointers.ToArray();
                    _simplePointers.Clear();
                }
            }

            if (free)
            {
                try
                {
                    Free();

                    foreach (var ptr in toFree)
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                }
                catch (Exception e)
                {
                    // We do not want to throw here, as normally this executes in a using, or in a destructor.
                    _logger.Error(e, "Error occured during freeing of {ptr}", _nativePtr);
                }

                _logger.Debug("Native Object {ptr} freed", _nativePtr);
            }
        }

        protected abstract void Free();

        internal static IntPtr MakeString(string str)
        {
            if (str == null)
                return IntPtr.Zero;

            return Marshal.StringToHGlobalAnsi(str);
        }

        internal static string GetString(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            return Marshal.PtrToStringAnsi(ptr);
        }
    }
}