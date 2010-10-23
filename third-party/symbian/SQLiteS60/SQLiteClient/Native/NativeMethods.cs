#region Using directives

using System;
using System.Text;

#endregion

namespace System.Data.SQLiteClient.Native
{
    internal abstract class NativeMethods : IDisposable
    {
        private IntPtr _Handle;
        private Encoding _Encoding;

        public NativeMethods(Encoding encoding)
        {
            _Encoding = encoding;
        }

        public SQLiteCode Open(string filename)
        {
            SQLiteString nativeStr = new SQLiteString(this, _Encoding, filename);
            try
            {
                return open(nativeStr.ToSQLite(), out _Handle);
            }
            finally
            {
                nativeStr.Dispose();
            }
        }

        public SQLiteCode ErrorCode()
        {
            return errcode(_Handle);
        }

        public string ErrorMessage()
        {
            if (_Encoding == Encoding.UTF8)
            {
                return SQLiteString.FromSQLite(this, errmsg(_Handle), _Encoding);
            }
            else
            {
                return SQLiteString.FromSQLite(this, errmsg16(_Handle), _Encoding);
            }
        }

        public int Close()
        {
            return close(_Handle);
        }

        public int exec(string sql, IntPtr callback, IntPtr arg, out string errmsg)
        {
            SQLiteString nativeStr = new SQLiteString(this, _Encoding, sql);
            try
            {
                IntPtr errMsg = IntPtr.Zero;
                int rval = exec(_Handle, nativeStr.ToSQLite(), callback, arg, out errMsg);
                errmsg = SQLiteString.FromSQLite(this, errMsg, _Encoding);
                
                return rval;
            }
            finally
            {
                nativeStr.Dispose();
            }
        }

        public long last_insert_rowid()
        {
            return last_insert_rowid(_Handle);
        }

        public int changes()
        {
            return changes(_Handle);
        }

        public int total_changes()
        {
            return total_changes(_Handle);
        }

        public void busy_timeout(int ms)
        {
            busy_timeout(_Handle, ms);
        }

        public int prepare(string zSql, out IntPtr ppVm, out IntPtr pzTail)
        {
            SQLiteString nativeStr = new SQLiteString(this, _Encoding, zSql);
            try
            {
                return prepare(_Handle, nativeStr.ToSQLite(), nativeStr.Length, out ppVm, out pzTail);
            }
            finally
            {
                nativeStr.Dispose();
            }
        }

        public int bind_blob(IntPtr stmt, int idx, byte[] val, SQLiteDestructor destructor)
        {
            unsafe
            {
                fixed (byte* b = val)
                {
                    return bind_blob(stmt, idx, b, val.Length, destructor);
                }
            }
        }

        public int bind_text(IntPtr stmt, int idx, string val, SQLiteDestructor destructor)
        {
            SQLiteString nativeStr = new SQLiteString(this, _Encoding, val);
            if (_Encoding == Encoding.UTF8)
            {
                return bind_text(stmt, idx, nativeStr.ToSQLite(), nativeStr.Length, destructor);
            }
            else
            {
                return bind_text16(stmt, idx, nativeStr.ToSQLite(), nativeStr.Length, destructor);
            }
        }

        public string SQliteVersion()
        {
            return SQLiteString.FromSQLite(this, libversion(), _Encoding);
        }

        public string ColumnName(IntPtr pStmt, int iCol)
        {
            if (_Encoding == Encoding.UTF8)
            {
                return SQLiteString.FromSQLite(this, column_name(pStmt, iCol), _Encoding);
            }
            else
            {
                return SQLiteString.FromSQLite(this, column_name16(pStmt, iCol), _Encoding);
            }
        }

        public string ColumnText(IntPtr pStmt, int iCol)
        {
            if (_Encoding == Encoding.UTF8)
            {
                return SQLiteString.FromSQLite(this, column_text(pStmt, iCol), _Encoding);
            }
            else
            {
                return SQLiteString.FromSQLite(this, column_text16(pStmt, iCol), _Encoding);
            }
        }

        public string ColumnDeclarationType(IntPtr pStmt, int i)
        {
            if (_Encoding == Encoding.UTF8)
            {
                return SQLiteString.FromSQLite(this, column_decltype(pStmt, i), _Encoding);
            }
            else
            {
                return SQLiteString.FromSQLite(this, column_decltype16(pStmt, i), _Encoding);
            }
        }

        public int ColumnBytes(IntPtr pStmt, int iCol)
        {
            if (_Encoding == Encoding.UTF8)
            {
                return column_bytes(pStmt, iCol);
            }
            else
            {
                return column_bytes16(pStmt, iCol);
            }
        }

        #region abstract Methods
        public abstract int strlen(IntPtr p);

        public abstract IntPtr malloc(int size);

        public abstract void free(IntPtr p);

        protected abstract IntPtr libversion();

        protected abstract SQLiteCode open(IntPtr filename, out IntPtr db);

        protected abstract SQLiteCode errcode(IntPtr db);

        protected abstract IntPtr errmsg(IntPtr db);

        protected abstract IntPtr errmsg16(IntPtr db);

        protected abstract int close(IntPtr h);

        protected abstract int exec(IntPtr h, IntPtr sql, IntPtr callback, IntPtr arg, out IntPtr errmsg);

        protected abstract long last_insert_rowid(IntPtr h);

        protected abstract int changes(IntPtr h);

        protected abstract int total_changes(IntPtr h);

        protected abstract void busy_timeout(IntPtr h, int ms);

        protected abstract int prepare(IntPtr db, IntPtr zSql, int nBytes, out IntPtr ppVm, out IntPtr pzTail);

        public abstract int column_count(IntPtr pStmt);

        protected abstract IntPtr column_name(IntPtr pStmt, int iCol);

        protected abstract IntPtr column_name16(IntPtr pStmt, int iCol);

        protected abstract IntPtr column_decltype(IntPtr pStmt, int i);

        protected abstract IntPtr column_decltype16(IntPtr pStmt, int iCol);

        public abstract SQLiteCode step(IntPtr pStmt);

        public abstract int data_count(IntPtr pStmt);

        public abstract IntPtr column_blob(IntPtr pStmt, int iCol);

        protected abstract int column_bytes(IntPtr pStmt, int iCol);

        protected abstract int column_bytes16(IntPtr pStmt, int iCol);

        public abstract double column_double(IntPtr pStmt, int iCol);

        public abstract int column_int(IntPtr pStmt, int iCol);

        public abstract long column_int64(IntPtr pStmt, int iCol);

        protected abstract IntPtr column_text(IntPtr pStmt, int iCol);

        protected abstract IntPtr column_text16(IntPtr pStmt, int iCol);

        public abstract SQLiteType column_type(IntPtr pStmt, int iCol);

        public abstract SQLiteCode finalize(IntPtr pStmt);

        public abstract int reset(IntPtr pStmt);

        protected abstract unsafe int bind_blob(IntPtr stmt, int idx, byte * val, int len, SQLiteDestructor destructor);

        public abstract int bind_double(IntPtr stmt, int idx, double val);

        public abstract int bind_int(IntPtr stmt, int idx, int val);

        public abstract int bind_int64(IntPtr stmt, int idx, long val);

        public abstract int bind_null(IntPtr stmt, int idx);

        protected abstract int bind_text(IntPtr stmt, int idx, IntPtr val, int n, SQLiteDestructor destructor);

        protected abstract int bind_text16(IntPtr stmt, int idx, IntPtr val, int n, SQLiteDestructor destructor);
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_Handle != IntPtr.Zero)
            {
                Close();
                _Handle = IntPtr.Zero;
            }
        }

        #endregion
    }
}
