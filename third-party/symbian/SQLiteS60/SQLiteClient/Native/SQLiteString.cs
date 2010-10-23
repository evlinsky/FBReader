#region Using directives

using System;
using System.Text;
using System.Runtime.InteropServices;

#endregion

namespace System.Data.SQLiteClient.Native
{
    internal class SQLiteString
    {
		private String _StringValue;
		private IntPtr _NativePtr;
		private Int32  _ByteLength;
		private	Encoding _Encoding;
        private NativeMethods _NativeMethods;

        public SQLiteString(NativeMethods nativeMethods, Encoding encoding)
            : this(nativeMethods, encoding, null)
		{
		}

        public SQLiteString(NativeMethods nativeMethods, String str)
            : this(nativeMethods, Encoding.ASCII, str)
		{
		}

        public SQLiteString(NativeMethods nativeMethods, Encoding encoding, String str)
		{
			_NativePtr = IntPtr.Zero;
			_ByteLength = 0;
			_Encoding = encoding;
			_StringValue  = str;
            _NativeMethods = nativeMethods;
		}

		public static String FromSQLite (NativeMethods nativeMethods, IntPtr sqliteStr, Encoding encoding)
		{
			if (sqliteStr == IntPtr.Zero) return null;

            Int32 slen = nativeMethods.strlen(sqliteStr);
			Byte[] bytes = new Byte[slen];
			Marshal.Copy (sqliteStr, bytes, 0, slen);
			
            return encoding.GetString (bytes,0,slen);
		}

		public String StringValue
		{
			get	{ return _StringValue;	}
			set
			{
				_StringValue = value;
				FreeMemory();
			}
		}

        public IntPtr ToSQLite()
		{
			if( _StringValue == null )
				return IntPtr.Zero;

			if (_NativePtr == IntPtr.Zero)
			{
				Byte[] bytes = _Encoding.GetBytes (_StringValue);
				_ByteLength = bytes.Length + 1;
                _NativePtr = _NativeMethods.malloc(_ByteLength);
				Marshal.Copy (bytes, 0, _NativePtr, bytes.Length);
				Marshal.WriteByte(_NativePtr,bytes.Length,0);
			}

			return _NativePtr;
		}

		public int Length
		{
            get 
            {
                ToSQLite();
                return _ByteLength;
            }
		}

		private void FreeMemory()
		{
			if (_NativePtr != IntPtr.Zero)
			{
                _NativeMethods.free(_NativePtr);
				_NativePtr = IntPtr.Zero;
			}
			_ByteLength = 0;
		}

		#region IDisposable Members

		public void Dispose()
		{
			FreeMemory();
		}

		#endregion
    }
}
