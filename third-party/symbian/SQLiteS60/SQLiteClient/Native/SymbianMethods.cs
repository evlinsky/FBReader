using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.SQLiteClient.Native
{
    internal class SymbianMethods : NativeMethods
    {
        public SymbianMethods(Encoding encoding) : base(encoding)
        {
        }

        #region INativeMethods Members

        protected override IntPtr libversion()
        {
            return sqlite3_libversion();
        }

        public override int strlen(IntPtr p)
        {
            return sqlite3_strlen(p);
        }

        public override IntPtr malloc(int size)
        {
            return sqlite3_malloc(size);
        }

        public override void free(IntPtr p)
        {
            sqlite3_free(p);
        }

        protected override SQLiteCode open(IntPtr filename, out IntPtr db)
        {
            return sqlite3_open(filename, out db);
        }

        protected override SQLiteCode errcode(IntPtr db)
        {
            return sqlite3_errcode(db);
        }

        protected override IntPtr errmsg(IntPtr db)
        {
            return sqlite3_errmsg(db);
        }

        protected override IntPtr errmsg16(IntPtr db)
        {
            return sqlite3_errmsg16(db);
        }

        protected override int close(IntPtr h)
        {
            return sqlite3_close(h);
        }

        protected override int exec(IntPtr h, IntPtr sql, IntPtr callback, IntPtr arg, out IntPtr errmsg)
        {
            return sqlite3_exec(h, sql, callback, arg, out errmsg);
        }

        protected override long last_insert_rowid(IntPtr h)
        {
            return sqlite3_last_insert_rowid(h);
        }

        protected override int changes(IntPtr h)
        {
            return sqlite3_changes(h);
        }

        protected override int total_changes(IntPtr h)
        {
            return sqlite3_total_changes(h);
        }

        protected override void busy_timeout(IntPtr h, int ms)
        {
            sqlite3_busy_timeout(h, ms);
        }

        protected override int prepare(IntPtr db, IntPtr zSql, int nBytes, out IntPtr ppVm, out IntPtr pzTail)
        {
            return sqlite3_prepare(db, zSql, nBytes, out ppVm, out pzTail);
        }

        public override int column_count(IntPtr pStmt)
        {
            return sqlite3_column_count(pStmt);
        }

        protected override IntPtr column_name(IntPtr pStmt, int iCol)
        {
            return sqlite3_column_name(pStmt, iCol);
        }

        protected override IntPtr column_name16(IntPtr pStmt, int iCol)
        {
            return sqlite3_column_name16(pStmt, iCol);
        }

        protected override IntPtr column_decltype(IntPtr pStmt, int i)
        {
            return sqlite3_column_decltype(pStmt, i);
        }

        protected override IntPtr column_decltype16(IntPtr pStmt, int iCol)
        {
            return sqlite3_column_decltype16(pStmt, iCol);
        }

        public override SQLiteCode step(IntPtr pStmt)
        {
            return sqlite3_step(pStmt);
        }

        public override int data_count(IntPtr pStmt)
        {
            return sqlite3_data_count(pStmt);
        }

        public override IntPtr column_blob(IntPtr pStmt, int iCol)
        {
            return sqlite3_column_blob(pStmt, iCol);
        }

        protected override int column_bytes(IntPtr pStmt, int iCol)
        {
            return sqlite3_column_bytes(pStmt, iCol);
        }

        protected override int column_bytes16(IntPtr pStmt, int iCol)
        {
            return sqlite3_column_bytes16(pStmt, iCol);
        }

        public override double column_double(IntPtr pStmt, int iCol)
        {
            return sqlite3_column_double(pStmt, iCol);
        }

        public override int column_int(IntPtr pStmt, int iCol)
        {
            return sqlite3_column_int(pStmt, iCol);
        }

        public override long column_int64(IntPtr pStmt, int iCol)
        {
            return sqlite3_column_int64(pStmt, iCol);
        }

        protected override IntPtr column_text(IntPtr pStmt, int iCol)
        {
            return sqlite3_column_text(pStmt, iCol);
        }

        protected override IntPtr column_text16(IntPtr pStmt, int iCol)
        {
            return sqlite3_column_text16(pStmt, iCol);
        }

        public override SQLiteType column_type(IntPtr pStmt, int iCol)
        {
            return sqlite3_column_type(pStmt, iCol);
        }

        public override SQLiteCode finalize(IntPtr h)
        {
            return sqlite3_finalize(h);
        }

        public override int reset(IntPtr h)
        {
            return sqlite3_reset(h);
        }

        protected override unsafe int bind_blob(IntPtr stmt, int idx, byte* val, int n, SQLiteDestructor destructor)
        {
            return sqlite3_bind_blob(stmt, idx, val, n, destructor);
        }

        public override int bind_double(IntPtr stmt, int idx, double val)
        {
            return sqlite3_bind_double(stmt, idx, val);
        }

        public override int bind_int(IntPtr stmt, int idx, int val)
        {
            return sqlite3_bind_int(stmt, idx, val);
        }

        public override int bind_int64(IntPtr stmt, int idx, long val)
        {
            return sqlite3_bind_int64(stmt, idx, val);
        }

        public override int bind_null(IntPtr stmt, int idx)
        {
            return sqlite3_bind_null(stmt, idx);
        }

        protected override int bind_text(IntPtr stmt, int idx, IntPtr val, int n, SQLiteDestructor destructor)
        {
            return sqlite3_bind_text(stmt, idx, val, n, destructor);
        }

        protected override int bind_text16(IntPtr stmt, int idx, IntPtr val, int n, SQLiteDestructor destructor)
        {
            return sqlite3_bind_text16(stmt, idx, val, n, destructor);
        }

        #endregion
        
        private const CallingConvention _Convention = (CallingConvention)4;

        [DllImport("sqlite.dll", EntryPoint = "#78", CallingConvention = _Convention)]
        private static extern IntPtr sqlite3_libversion();
        
        [DllImport("sqlite.dll", EntryPoint = "#123", CallingConvention = _Convention)]
        private static extern int sqlite3_strlen(IntPtr p);

        [DllImport("sqlite.dll", EntryPoint = "#81", CallingConvention = _Convention)]
        private static extern IntPtr sqlite3_malloc(int size);

        [DllImport("sqlite.dll", EntryPoint = "#70", CallingConvention = _Convention)]
        private static extern void sqlite3_free(IntPtr p);

        [DllImport("sqlite.dll", EntryPoint = "#86", CallingConvention = _Convention)]
        private static extern SQLiteCode sqlite3_open(IntPtr filename, out IntPtr db);

        [DllImport("sqlite.dll", EntryPoint = "#62", CallingConvention = _Convention)]
        private static extern SQLiteCode sqlite3_errcode(IntPtr db);

        [DllImport("sqlite.dll", EntryPoint = "#63", CallingConvention = _Convention)]
        private static extern IntPtr sqlite3_errmsg(IntPtr db);

        [DllImport("sqlite.dll", EntryPoint = "#64", CallingConvention = _Convention)]
        private static extern IntPtr sqlite3_errmsg16(IntPtr db);

        [DllImport("sqlite.dll", EntryPoint = "#27", CallingConvention = _Convention)]
        private static extern int sqlite3_close(IntPtr h);

        [DllImport("sqlite.dll", EntryPoint = "#65", CallingConvention = _Convention)]
        private static extern int sqlite3_exec(IntPtr h, IntPtr sql, IntPtr callback, IntPtr arg, out IntPtr errmsg);

        [DllImport("sqlite.dll", EntryPoint = "#77", CallingConvention = _Convention)]
        private static extern long sqlite3_last_insert_rowid(IntPtr h);

        [DllImport("sqlite.dll", EntryPoint = "#25", CallingConvention = _Convention)]
        private static extern int sqlite3_changes(IntPtr h);

        [DllImport("sqlite.dll", EntryPoint = "#126", CallingConvention = _Convention)]
        private static extern int sqlite3_total_changes(IntPtr h);

        [DllImport("sqlite.dll", EntryPoint = "#24", CallingConvention = _Convention)]
        private static extern void sqlite3_busy_timeout(IntPtr h, int ms);

        [DllImport("sqlite.dll", EntryPoint = "#90", CallingConvention = _Convention)]
        private static extern int sqlite3_prepare(IntPtr db, IntPtr zSql, int nBytes, out IntPtr ppVm, out IntPtr pzTail);

        [DllImport("sqlite.dll", EntryPoint = "#33", CallingConvention = _Convention)]
        private static extern int sqlite3_column_count(IntPtr pStmt);

        [DllImport("sqlite.dll", EntryPoint = "#41", CallingConvention = _Convention)]
        private static extern IntPtr sqlite3_column_name(IntPtr pStmt, int iCol);

        [DllImport("sqlite.dll", EntryPoint = "#42", CallingConvention = _Convention)]
        private static extern IntPtr sqlite3_column_name16(IntPtr pStmt, int iCol);

        [DllImport("sqlite.dll", EntryPoint = "#34", CallingConvention = _Convention)]
        private static extern IntPtr sqlite3_column_decltype(IntPtr pStmt, int i);

        [DllImport("sqlite.dll", EntryPoint = "#35", CallingConvention = _Convention)]
        private static extern IntPtr sqlite3_column_decltype16(IntPtr pStmt, int iCol);

        [DllImport("sqlite.dll", EntryPoint = "#122", CallingConvention = _Convention)]
        private static extern SQLiteCode sqlite3_step(IntPtr pStmt);

        [DllImport("sqlite.dll", EntryPoint = "#57", CallingConvention = _Convention)]
        private static extern int sqlite3_data_count(IntPtr pStmt);

        [DllImport("sqlite.dll", EntryPoint = "#30", CallingConvention = _Convention)]
        private static extern IntPtr sqlite3_column_blob(IntPtr pStmt, int iCol);

        [DllImport("sqlite.dll", EntryPoint = "#31", CallingConvention = _Convention)]
        private static extern int sqlite3_column_bytes(IntPtr pStmt, int iCol);

        [DllImport("sqlite.dll", EntryPoint = "#32", CallingConvention = _Convention)]
        private static extern int sqlite3_column_bytes16(IntPtr pStmt, int iCol);

        [DllImport("sqlite.dll", EntryPoint = "#36", CallingConvention = _Convention)]
        private static extern double sqlite3_column_double(IntPtr pStmt, int iCol);

        [DllImport("sqlite.dll", EntryPoint = "#38", CallingConvention = _Convention)]
        private static extern int sqlite3_column_int(IntPtr pStmt, int iCol);

        [DllImport("sqlite.dll", EntryPoint = "#39", CallingConvention = _Convention)]
        private static extern long sqlite3_column_int64(IntPtr pStmt, int iCol);

        [DllImport("sqlite.dll", EntryPoint = "#43", CallingConvention = _Convention)]
        private static extern IntPtr sqlite3_column_text(IntPtr pStmt, int iCol);

        [DllImport("sqlite.dll", EntryPoint = "#44", CallingConvention = _Convention)]
        private static extern IntPtr sqlite3_column_text16(IntPtr pStmt, int iCol);

        [DllImport("sqlite.dll", EntryPoint = "#45", CallingConvention = _Convention)]
        private static extern SQLiteType sqlite3_column_type(IntPtr pStmt, int iCol);

        [DllImport("sqlite.dll", EntryPoint = "#69", CallingConvention = _Convention)]
        private static extern SQLiteCode sqlite3_finalize(IntPtr h);

        [DllImport("sqlite.dll", EntryPoint = "#98", CallingConvention = _Convention)]
        private static extern int sqlite3_reset(IntPtr h);

        [DllImport("sqlite.dll", EntryPoint = "#4", CallingConvention = _Convention)]
        private unsafe static extern int sqlite3_bind_blob(IntPtr stmt, int idx, byte* val, int n, SQLiteDestructor destructor);

        [DllImport("sqlite.dll", EntryPoint = "#5", CallingConvention = _Convention)]
        private static extern int sqlite3_bind_double(IntPtr stmt, int idx, double val);

        [DllImport("sqlite.dll", EntryPoint = "#7", CallingConvention = _Convention)]
        private static extern int sqlite3_bind_int(IntPtr stmt, int idx, int val);

        [DllImport("sqlite.dll", EntryPoint = "#8", CallingConvention = _Convention)]
        private static extern int sqlite3_bind_int64(IntPtr stmt, int idx, long val);

        [DllImport("sqlite.dll", EntryPoint = "#10", CallingConvention = _Convention)]
        private static extern int sqlite3_bind_null(IntPtr stmt, int idx);

        [DllImport("sqlite.dll", EntryPoint = "#14", CallingConvention = _Convention)]
        private static extern int sqlite3_bind_text(IntPtr stmt, int idx, IntPtr val, int n, SQLiteDestructor destructor);

        [DllImport("sqlite.dll", EntryPoint = "#15", CallingConvention = _Convention)]
        private static extern int sqlite3_bind_text16(IntPtr stmt, int idx, IntPtr val, int n, SQLiteDestructor destructor);
    }
}
