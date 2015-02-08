using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpotiFire
{
    internal class NativeContext : IDisposable
    {
        readonly List<IntPtr> _ptrs = new List<IntPtr>();

        public IntPtr String(string str)
        {
            var ptr = NativeObject.MakeString(str);
            _ptrs.Add(ptr);
            return ptr;
        }

        public void Dispose()
        {
            foreach(var ptr in _ptrs)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}