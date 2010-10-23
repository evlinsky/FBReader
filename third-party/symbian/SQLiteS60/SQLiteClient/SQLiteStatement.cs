using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Data.SQLiteClient.Native;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Data.SQLiteClient
{
	sealed internal class SQLiteStatement : IDisposable
	{
        /**
        * Using Universable Sortable format "s"
        *  - see Help documentation on DateTimeFormatInfo Class for more info.
        * In actual fact we may want to use a more compact pattern like
        * yyyyMMdd'T'HHmmss.fffffff
        * See also http://www.cl.cam.ac.uk/~mgk25/iso-time.html
        */
        private const string _ISO8601SQLiteDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        private static string[] _ISO8601DateFormats = new string[] 
                                {   _ISO8601SQLiteDateTimeFormat,
									"yyyyMMddHHmmss",
									"yyyyMMddTHHmmssfffffff",
									"yyyy-MM-dd",
									"yy-MM-dd",
									"yyyyMMdd",
									"HH:mm:ss",
									"THHmmss"
								};

        private SQLiteCommand _Command;
		private String _CommandText;
		private ArrayList _ParameterNames;
		private int _UnnamedParametersStartIndex;
        private bool _FirstStep;
        private int _PreviousTotalChangesCount;
        private IntPtr _StatementHandle;
        private NativeMethods _NativeMethods;

		public SQLiteStatement(SQLiteCommand command, String commandText, ArrayList paramNames)
		{
            if (command == null) throw new ArgumentNullException("Command is null.");
            
            if (commandText == null) throw new ArgumentNullException("Command text is null.");

            if (commandText.Length == 0) throw new ArgumentException("The command text cannot be empty.");

            _Command = command;
            _UnnamedParametersStartIndex = 0;
            _CommandText = commandText;
			_ParameterNames = paramNames;
            _FirstStep = true;
		}

		public string CommandText
		{
			get { return _CommandText; }
		}

		public void Compile()
		{
            if (_Command.Connection == null || _Command.Connection.State == ConnectionState.Closed || _Command.Connection.State == ConnectionState.Broken)
            {
                throw new InvalidOperationException("Connection is currently executing an operation.");
            }

            if (_NativeMethods == null) _NativeMethods = _Command.Connection.NativeMethods;

			if (!_FirstStep)
			{
				Reset();
			}
			else
			{
				if (_CommandText.Length == 0) throw new InvalidOperationException("The command text cannot be empty.");

                IntPtr pTail;
                IntPtr pVM;
                _NativeMethods.prepare(_CommandText, out pVM, out pTail);

                if (_NativeMethods.ErrorCode() != SQLiteCode.Ok)
                {
                    throw new SQLiteException(string.Format("Error while prepare statement {0}.\r\n {1}", _CommandText, _NativeMethods.ErrorMessage()));
                }
                    
                _StatementHandle = pVM;
            }

			BindParameters();
		}

		public int GetUnnamedParameterCount()
		{
			int count = 0;
			for(int i=0 ; i < _ParameterNames.Count ; ++i)
			{
				if(_ParameterNames[i] == null) ++count;
			}
			return count;
		}

        private SQLiteParameter FindUnnamedParameter(int index)
        {
            if (index >= 0)
            {
                for (int i = 0; i < _Command.Parameters.Count; ++i)
                {
                    SQLiteParameter param = _Command.Parameters[i];
                    if (param.ParameterName == null)
                    {
                        if (index-- == 0) return param;
                    }
                }
            }

            throw new SQLiteException(String.Format("Can not find unnamed parameter with index = {0}.", index));
        }

        private SQLiteParameter FindNamedParameter(String parameterName)
        {
            int index = _Command.Parameters.IndexOf(parameterName);

            if (index < 0) throw new SQLiteException(String.Format("Can not find the parameter with name '{0}'.", parameterName));

            return _Command.Parameters[index];
        }
        
        private void BindParameters()
		{
			int count = _ParameterNames.Count;
			int unnamedParamCount = 0;
			for(int i=0 ; i < count ; ++i)
			{
				SQLiteParameter param = null;
				string paramName = (string)_ParameterNames[i];

                if (paramName == null)
                {
                    param = FindUnnamedParameter(unnamedParamCount + _UnnamedParametersStartIndex);
                    ++unnamedParamCount;
                }
                else
                {
                    param = FindNamedParameter(paramName);
                }

				bind(i+1, param);
			}
		}

		public void SetUnnamedParametersStartIndex(int index)
		{
			_UnnamedParametersStartIndex = index;
		}

        public SQLiteCode Step()
        {
            if (_FirstStep)
            {
                _FirstStep = false;
                _PreviousTotalChangesCount = _NativeMethods.total_changes();
            }
            SQLiteCode res = _NativeMethods.step(_StatementHandle);
            
            if (res == SQLiteCode.LibraryUsedIncorrectly || res == SQLiteCode.Busy)
            {
                throw new SQLiteException(string.Format("Step failed for {0}, ErrorCode {1}. ", _CommandText, _NativeMethods.ErrorCode()));
            }

            return res;
        }

        public void Reset()
        {
            _NativeMethods.reset(_StatementHandle);

            if (_NativeMethods.ErrorCode() != SQLiteCode.Ok)
            {
                throw new SQLiteException(string.Format("Error while reset statement {0}.\r\n {1}", _CommandText, _NativeMethods.ErrorMessage()));
            }
            
            _FirstStep = true;
            _Command.Connection.LastChangesCount = _NativeMethods.total_changes() - _PreviousTotalChangesCount;
        }

        private void bind(int index, IDbDataParameter parameter)
        {
            object value = parameter.Value;

            if (value == null || value == DBNull.Value)
            {
                _NativeMethods.bind_null(_StatementHandle, index);
            }
            else
            {
                switch (parameter.DbType)
                {
                    case DbType.Boolean:
                        _NativeMethods.bind_int(_StatementHandle, index, Convert.ToBoolean(value) ? 1 : 0);
                        break;

                    case DbType.Byte:
                    case DbType.Int16:
                    case DbType.Int32:
                    case DbType.SByte:
                    case DbType.UInt16:
                    case DbType.UInt32:
                        _NativeMethods.bind_int(_StatementHandle, index, Convert.ToInt32(value));
                        break;

                    case DbType.Double:
                    case DbType.Single:
                        _NativeMethods.bind_double(_StatementHandle, index, Convert.ToDouble(value));
                        break;

                    case DbType.Date:
                    case DbType.DateTime:
                    case DbType.Time:
                        if (_Command.Connection.DateTimeFormat.Equals(DateTimeFormat.CurrentCulture))
                        {
                            value = Convert.ToDateTime(value).ToString(DateTimeFormatInfo.CurrentInfo);
                            goto default;
                        }
                        else if (_Command.Connection.DateTimeFormat.Equals(DateTimeFormat.Ticks))
                        {
                            _NativeMethods.bind_int64(_StatementHandle, index, Convert.ToDateTime(value).Ticks);
                        }
                        else
                        {
                            value = Convert.ToDateTime(value).ToString(_ISO8601SQLiteDateTimeFormat);
                            goto default;
                        }

                        break;

                    case DbType.Int64:
                    case DbType.UInt64:
                        _NativeMethods.bind_int64(_StatementHandle, index, Convert.ToInt64(value));
                        break;

                    case DbType.Binary:
                        byte[] bytes = (byte[])value;
                        _NativeMethods.bind_blob(_StatementHandle, index, bytes, SQLiteDestructor.Transient);
                        break;

                    default:
                        // cache was purged, fill it again
                        string str = null;
                        switch (parameter.DbType)
                        {
                            case DbType.Currency:
                            case DbType.Decimal:
                                str = Convert.ToDecimal(value).ToString(_Command.Connection.UniversalFormatProvider);
                                break;

                            default:
                                str = Convert.ToString(value);
                                break;
                        }

                        _NativeMethods.bind_text(_StatementHandle, index, str, SQLiteDestructor.Static);
                        break;
                }
            }

            if (_NativeMethods.ErrorCode() != SQLiteCode.Ok)
            {
                throw new SQLiteException(string.Format("Bind parameter {0} (data type {1}) failed.\r\nSQLite message: {2}", 
                    parameter.ParameterName, parameter.DbType, _NativeMethods.ErrorMessage()));
            }

        }

        public int GetColumnCount()
        {
            return _NativeMethods.column_count(_StatementHandle);
        }

        public string GetColumnName(int iCol)
        {
            return _NativeMethods.ColumnName(_StatementHandle, iCol);
        }

        public string GetColumnType(int iCol)
        {
            string type = _NativeMethods.ColumnDeclarationType(_StatementHandle, iCol);
            
            if (type == null)
            {
                // Try a different method for determining the column type.
                switch (_NativeMethods.column_type(_StatementHandle, iCol))
                {
                    case SQLiteType.Integer:
                        type = "INT64";
                        break;
                    case SQLiteType.Float:
                        type = "FLOAT";
                        break;
                    case SQLiteType.Text:
                        type = "TEXT";
                        break;
                    case SQLiteType.Blob:
                        type = "BLOB";
                        break;
                }
            }

            return type;
        }

        public bool IsColumnNull(int iCol)
        {
            return _NativeMethods.column_type(_StatementHandle, iCol) == SQLiteType.Null;
        }

        public bool IsColumnEmptyString(int iCol)
        {
            if (_NativeMethods.column_type(_StatementHandle, iCol) == SQLiteType.Text)
            {
                int bytes = _NativeMethods.ColumnBytes(_StatementHandle, iCol);
                return bytes <= 1;
            }
            return false;
        }

        public string GetColumnValue(int iCol)
        {
            return _NativeMethods.ColumnText(_StatementHandle, iCol);
        }

        public int GetColumnInt(int iCol)
        {
            return _NativeMethods.column_int(_StatementHandle, iCol);
        }

        public long GetColumnLong(int iCol)
        {
            return _NativeMethods.column_int64(_StatementHandle, iCol);
        }

        public double GetColumnDouble(int iCol)
        {
            return _NativeMethods.column_double(_StatementHandle, iCol);
        }

        public decimal GetColumnDecimal(int iCol)
        {
            return decimal.Parse(GetColumnValue(iCol), _Command.Connection.UniversalFormatProvider);
        }

        public DateTime GetColumnDateTime(int iCol)
        {
            if (_Command.Connection.DateTimeFormat.Equals(DateTimeFormat.CurrentCulture))
            {
                return DateTime.Parse(GetColumnValue(iCol));
            }
            else if (_Command.Connection.DateTimeFormat.Equals(DateTimeFormat.Ticks))
            {
                return new DateTime(GetColumnLong(iCol));
            }
            else
            {
                string val = GetColumnValue(iCol);
                object retVal = null;
                try
                {
                    retVal = DateTime.ParseExact(val, _ISO8601DateFormats,
                                                    DateTimeFormatInfo.InvariantInfo,
                                                    DateTimeStyles.None);
                }
                catch { /* nothing to do */ }
                
                if (retVal == null)
                {
                    try
                    {
                        long iVal = Int64.Parse(val);

                        // Try the old Ticks format.
                        retVal = new DateTime(iVal);
                    }
                    catch
                    {
                        throw new SQLiteException(string.Format("Invalid DateTime Field Format: {0}", val));
                    }
                }

                return (DateTime)retVal;
            }
        }

        public long GetColumnBytes(int iCol, long fieldOffset, Byte[] buffer, int bufferoffset, int length)
        {
            int fieldLen = _NativeMethods.ColumnBytes(_StatementHandle, iCol);
            
            if (buffer == null) return fieldLen;

            IntPtr blob = _NativeMethods.column_blob(_StatementHandle, iCol);
            
            if (blob == IntPtr.Zero) return 0;

            IntPtr src = (IntPtr)(blob.ToInt32() + fieldOffset);

            if ((fieldOffset + length) > fieldLen) length = fieldLen - (int)fieldOffset;
            if ((bufferoffset + length) > buffer.Length) length = buffer.Length - bufferoffset;
            
            Marshal.Copy(src, buffer, bufferoffset, length);
            
            return length;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_StatementHandle != IntPtr.Zero)
            {
                SQLiteCode res = _NativeMethods.finalize(_StatementHandle);
                _StatementHandle = IntPtr.Zero;

                if (res != SQLiteCode.Ok && res != SQLiteCode.CallbackAbort)
                {
                    throw new SQLiteException(string.Format("Error while finalize statement {0}.\r\n {1}", _CommandText, _NativeMethods.ErrorMessage()));
                }

                _Command.Connection.LastChangesCount = _NativeMethods.total_changes() - _PreviousTotalChangesCount;
            }
        }

        #endregion
    }
}
