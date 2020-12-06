using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class PointerEx
    {
        public IntPtr IntPtr { get; set; }
        public PointerEx(IntPtr value)
        {
            this.IntPtr = value;
        }

        #region overrides
        public static implicit operator IntPtr(PointerEx px)
        {
            return px.IntPtr;
        }

        public static implicit operator PointerEx(IntPtr ip)
        {
            return new PointerEx(ip);
        }

        public static PointerEx operator +(PointerEx px, PointerEx pxo)
        {
            return px.Add(pxo);
        }

        public static implicit operator bool(PointerEx px)
        {
            return px != IntPtr.Zero;
        }

        public static implicit operator int(PointerEx px)
        {
            return px.IntPtr.ToInt32();
        }

        public static implicit operator uint(PointerEx px)
        {
            return (uint)px.IntPtr.ToInt32();
        }

        public static implicit operator long(PointerEx px)
        {
            return px.IntPtr.ToInt64();
        }

        public static implicit operator ulong(PointerEx px)
        {
            return (ulong)px.IntPtr.ToInt64();
        }

        public static implicit operator PointerEx(int i)
        {
            return new IntPtr(i);
        }

        public static implicit operator PointerEx(uint ui)
        {
            return new IntPtr(ui);
        }

        public static implicit operator PointerEx(long l)
        {
            return new IntPtr(l);
        }

        public static implicit operator PointerEx(ulong ul)
        {
            return new IntPtr((long)ul);
        }

        public override string ToString()
        {
            return this.IntPtr.ToInt64().ToString($"X{IntPtr.Size * 2}");
        }
        #endregion
    }
}
