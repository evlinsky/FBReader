using System;
using System.Globalization;
using System.Data;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.SQLiteClient.Native;

namespace System.Data.SQLiteClient
{
    sealed public class SQLiteDataReader : IDataReader, IEnumerable
    {
        private SQLiteCommand _Command;
        private bool _Done;
        private bool _FirstRead;
        private String[] _FieldNames;
        private Type[] _FieldTypes;
        private int[] _ColumnNameStartIndexes;
        private int _StatementIndex;
        private int _RecordsAffected;
        private SQLiteStatement _Statement;
        internal CommandBehavior _CommandBehavior;
        private bool _Initialized;
        private NativeMethods _NativeMethods;

        internal SQLiteDataReader(SQLiteCommand command, CommandBehavior commandBehavior)
        {
            _Done = true;
            _FirstRead = true;
            _StatementIndex = 0;
            _RecordsAffected = 0;
            _Initialized = false;

            if (command == null) throw new ArgumentNullException("Command cannot be null.");
            if (command.Statements.Count <= 0) throw new ArgumentException("The Command contains no statement.");
            _Command = command;
            _CommandBehavior = commandBehavior;
            _NativeMethods = _Command.Connection.NativeMethods;
        }

        private void EnsureInitialization()
        {
            if (!_Initialized)
            {
                _Initialized = true;
                _Command.AttachDataReader(this);
                ExecuteFirstStep();
            }
        }

        private void ExecuteFirstStep()
        {
            _Done = true;
            _FirstRead = true;

            // Exec first step and get column info.
            _Statement = _Command.Statements[_StatementIndex];
            _Statement.Compile();
            _NativeMethods.busy_timeout(_Command.CommandTimeout);

            SQLiteCode res = _Statement.Step();
            if (res == SQLiteCode.RowReady || res == SQLiteCode.Done)
            {
                _FieldNames = new String[_Statement.GetColumnCount()];
                _FieldTypes = new Type[_Statement.GetColumnCount()];
                _ColumnNameStartIndexes = new int[_Statement.GetColumnCount()];

                // Get column info.
                for (int i = 0; i < _Statement.GetColumnCount(); i++)
                {
                    _FieldNames[i] = _Statement.GetColumnName(i);
                    _FieldTypes[i] = SQLType2Type(_Statement.GetColumnType(i));
                    _ColumnNameStartIndexes[i] = _FieldNames[i].IndexOf('.') + 1;
                }

                _Done = res == SQLiteCode.Done;
            }
            else if (res == SQLiteCode.Error) _Statement.Dispose();
            else throw new SQLiteException(string.Format("Unknown SQLite error code {0}.", res));
        }

        private static Type SQLType2Type(String pTypeStr)
        {
            if (pTypeStr != null) pTypeStr = pTypeStr.ToUpper(CultureInfo.InvariantCulture);

            if (pTypeStr == null || pTypeStr.IndexOf("CHAR") != -1 || pTypeStr.IndexOf("TEXT") != -1) return typeof(String);
            else if (pTypeStr.IndexOf("SMALLINT") != -1) return typeof(Int16);
            else if (pTypeStr.IndexOf("BIGINT") != -1) return typeof(Int64);
            else if (pTypeStr.IndexOf("INT") != -1) return typeof(Int64);
            else if (pTypeStr.IndexOf("DOUBLE") != -1 || pTypeStr.IndexOf("REAL") != -1) return typeof(Double);
            else if (pTypeStr.IndexOf("FLOAT") != -1) return typeof(Single);
            else if (pTypeStr.IndexOf("BIT") != -1 || pTypeStr.IndexOf("BOOL") != -1) return typeof(Boolean);
            else if (pTypeStr.IndexOf("NUMERIC") != -1 || pTypeStr.IndexOf("DECIMAL") != -1 || pTypeStr.IndexOf("MONEY") != -1) return typeof(Decimal);
            else if (pTypeStr.IndexOf("DATE") != -1 || pTypeStr.IndexOf("TIME") != -1) return typeof(DateTime);
            else if (pTypeStr.IndexOf("BLOB") != -1 || pTypeStr.IndexOf("BINARY") != -1) return typeof(Byte[]);

            return typeof(String);
        }

        public void Dispose()
        {
            Close();
        }

        public int Depth
        {
            get { return 0; }
        }

        public bool IsClosed
        {
            get { return _Command == null; }
        }

        public int RecordsAffected
        {
            get { return _RecordsAffected; }
        }

        public void Close()
        {
            if (_Command != null)
            {
                CalculateRecordsAffected();
                _Command.DetachDataReader(this);
                _Command = null;
                _FieldNames = null;
                _FieldTypes = null;
            }
        }

        private void CalculateRecordsAffected()
        {
            EnsureInitialization();
            if (_Statement != null)
            {
                _Statement.Reset();
            }
            if (_Command != null && _Command.Connection != null) _RecordsAffected += _Command.Connection.LastChangesCount;
        }

        public DataTable GetSchemaTable()
        {
            EnsureInitialization();
            DataTable pSchemaTable = new DataTable("Schema");

            pSchemaTable.Columns.Add("ColumnName", typeof(String));
            pSchemaTable.Columns.Add("ColumnOrdinal", typeof(Int32));
            pSchemaTable.Columns.Add("ColumnSize", typeof(Int32));
            pSchemaTable.Columns.Add("NumericPrecision", typeof(Int32));
            pSchemaTable.Columns.Add("NumericScale", typeof(Int32));
            pSchemaTable.Columns.Add("DataType", typeof(Type));
            pSchemaTable.Columns.Add("ProviderType", typeof(Int32));
            pSchemaTable.Columns.Add("IsLong", typeof(Boolean));
            pSchemaTable.Columns.Add("AllowDBNull", typeof(Boolean));
            pSchemaTable.Columns.Add("IsReadOnly", typeof(Boolean));
            pSchemaTable.Columns.Add("IsRowVersion", typeof(Boolean));
            pSchemaTable.Columns.Add("IsUnique", typeof(Boolean));
            pSchemaTable.Columns.Add("IsKey", typeof(Boolean));
            pSchemaTable.Columns.Add("IsAutoIncrement", typeof(Boolean));
            pSchemaTable.Columns.Add("BaseSchemaName", typeof(String));
            pSchemaTable.Columns.Add("BaseCatalogName", typeof(String));
            pSchemaTable.Columns.Add("BaseTableName", typeof(String));
            pSchemaTable.Columns.Add("BaseColumnName", typeof(String));

            pSchemaTable.BeginLoadData();
            for (Int32 i = 0; i < _FieldNames.Length; ++i)
            {
                DataRow pSchemaRow = pSchemaTable.NewRow();

                // This may be as good as we can do.
                string columnName = GetName(i);
                pSchemaRow["ColumnName"] = columnName;
                pSchemaRow["ColumnOrdinal"] = i;
                pSchemaRow["ColumnSize"] = 0;
                pSchemaRow["NumericPrecision"] = 0;
                pSchemaRow["NumericScale"] = 0;
                pSchemaRow["DataType"] = _FieldTypes[i];
                pSchemaRow["ProviderType"] = GetProviderType(_FieldTypes[i]);
                pSchemaRow["IsLong"] = (false);
                pSchemaRow["AllowDBNull"] = (true);
                pSchemaRow["IsReadOnly"] = (false);
                pSchemaRow["IsRowVersion"] = (false);
                pSchemaRow["IsUnique"] = (false);
                pSchemaRow["IsKey"] = (false);
                pSchemaRow["IsAutoIncrement"] = (false);
                pSchemaRow["BaseSchemaName"] = "";
                pSchemaRow["BaseCatalogName"] = "";
                pSchemaRow["BaseTableName"] = "";
                pSchemaRow["BaseColumnName"] = columnName;

                pSchemaTable.Rows.Add(pSchemaRow);
            }
            // Enhance schema support for SELECT command.
            // It's necessary to build SQLiteCommandBuilder
            EnhanceSchemaTable(pSchemaTable);
            pSchemaTable.EndLoadData();

            return pSchemaTable;
        }

        public bool NextResult()
        {
            if (_Command == null) throw new NullReferenceException("The Command is null.");

            EnsureInitialization();
            _StatementIndex ++;
            
            bool success = _StatementIndex < _Command.Statements.Count;
            
            if (success)
            {
                CalculateRecordsAffected();
                ExecuteFirstStep();
            }
            
            return success;
        }

        public bool Read()
        {
            if (_Command == null) throw new NullReferenceException("The Command is null.");

            EnsureInitialization();
            if (_Done) return false;
            
            if (_FirstRead)
            {
                _FirstRead = false;
                return true;
            }

            SQLiteCode res = _Statement.Step();
            if (res == SQLiteCode.RowReady) return true;
            else if (res == SQLiteCode.Done) _Done = true;
            else if (res == SQLiteCode.Error) _Statement.Dispose();
            else throw new SQLiteException(string.Format("Unknown SQLite error code {0}.", res));
            
            return false;
        }

        public int FieldCount
        {
            get
            {
                EnsureInitialization();
                return _FieldNames.Length;
            }
        }

        public Object this[String name]
        {
            get
            {
                return this[GetOrdinal(name)];
            }
        }

        public Object this[int i]
        {
            get
            {
                return GetValue(i);
            }
        }

        public bool GetBoolean(int i)
        {
            int val = 0;
            try { val = GetInt32(i); }
            catch (FormatException) { }

            // if val is zero, it could be the format exception or the invalid data. 
            if (val != 0) return true;

            String s = GetString(i);
            try { return Boolean.Parse(s); }
            catch (FormatException) { }

            s = s.TrimStart();

            // now find the first non-digit and parse the string up to this character
            const int MaxLen = 21;
            int lengthToSearch = s.Length > MaxLen ? MaxLen : s.Length;
            int len = 0;
            while (len < lengthToSearch)
            {
                char c = s[len];
                if (c < '0' || c > '9')
                {
                    if (len > 0 || (c != '-' && c != '+'))
                        break;
                }
                ++len;
            }

            try { return Int32.Parse(s.Substring(0, len)) != 0; }
            catch (FormatException) { }

            return false;
        }

        public Byte GetByte(int i)
        {
            return (Byte)GetInt32(i);
        }

        public long GetBytes(
            int i,
            long fieldOffset,
            Byte[] buffer,
            int bufferoffset,
            int length
            )
        {
            return _Statement.GetColumnBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public Char GetChar(int i)
        {
            string s = GetString(i);
            if (s == null || s == "") throw new SQLiteException("Value is DBNull.");
            return s[0];
        }

        public long GetChars(
            int i,
            long fieldoffset,
            Char[] buffer,
            int bufferoffset,
            int length
            )
        {
            String pStr = GetString(i);
            if (buffer == null) return pStr.Length;
            
            if ((fieldoffset + length) > pStr.Length)
                length = pStr.Length - (int)fieldoffset;
            if ((bufferoffset + length) > buffer.Length)
                length = buffer.Length - bufferoffset;
            
            pStr.CopyTo((int)fieldoffset, buffer, bufferoffset, length);
            return length;
        }

        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        public String GetDataTypeName(int i)
        {
            EnsureInitialization();
            return _FieldTypes[i].Name;
        }

        public DateTime GetDateTime(int i)
        {
            return _Statement.GetColumnDateTime(i);
        }

        public Decimal GetDecimal(int i)
        {
            return _Statement.GetColumnDecimal(i);
        }

        public double GetDouble(int i)
        {
            return _Statement.GetColumnDouble(i);
        }

        public Type GetFieldType(int i)
        {
            EnsureInitialization();
            return _FieldTypes[i];
        }

        public float GetFloat(int i)
        {
            return (float)GetDouble(i);
        }

        public Guid GetGuid(int i)
        {
            return new Guid(GetString(i));
        }

        public short GetInt16(int i)
        {
            return (short)GetInt32(i);
        }

        public int GetInt32(int i)
        {
            return _Statement.GetColumnInt(i);
        }

        public long GetInt64(int i)
        {
            return _Statement.GetColumnLong(i);
        }

        public String GetName(int i)
        {
            EnsureInitialization();
            return _FieldNames[i].Substring(_ColumnNameStartIndexes[i]);
        }

        public int GetOrdinal(String name)
        {
            int i;
            for (i = 0; i < _FieldNames.Length; ++i)
                if (name.Equals(_FieldNames[i]))
                    return i;

            for (i = 0; i < _FieldNames.Length; ++i)
                if (String.Compare(name, 0, _FieldNames[i], _ColumnNameStartIndexes[i], System.Int32.MaxValue, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
                    return i;
            
            return -1;
        }

        public String GetString(int i)
        {
            string s = _Statement.GetColumnValue(i);
            if (s == null) return "";
            
            return s;
        }

        public Object GetValue(int i)
        {
            if (!IsDBNull(i))
            {
                Type type = _FieldTypes[i];
                if (type == typeof(String)) return GetString(i);
                
                if (!_Statement.IsColumnEmptyString(i))
                {
                    if (type == typeof(Double))
                        return GetDouble(i);
                    if (type == typeof(Int16))
                        return GetInt16(i);
                    if (type == typeof(Int32))
                        return GetInt32(i);
                    if (type == typeof(Int64))
                        return GetInt64(i);
                    if (type == typeof(Single))
                        return GetFloat(i);
                    if (type == typeof(Boolean))
                        return GetBoolean(i);
                    if (type == typeof(Decimal))
                        return GetDecimal(i);
                    if (type == typeof(DateTime))
                        return GetDateTime(i);
                    if (type == typeof(Byte[]))
                    {
                        int len = (int)GetBytes(i, 0, null, 0, 0);
                        if (len == 0) return DBNull.Value;
                        else
                        {
                            byte[] buffer = new byte[len];
                            GetBytes(i, 0, buffer, 0, len);
                            return buffer;
                        }
                    }
                    throw new SQLiteException(string.Format("Invalid data type {0}", type.Name));
                }
            }
            return DBNull.Value;
        }

        public int GetValues(Object[] values)
        {
            int count = values.Length;
            if (_FieldNames.Length < count) count = _FieldNames.Length;
            
            for (int i = 0; i < count; ++i)
                values[i] = GetValue(i);
            
            return count;
        }

        public bool IsDBNull(int i)
        {
            return _Statement.IsColumnNull(i);
        }

        public IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this, true);
        }

        private void EnhanceSchemaTable(DataTable schemaTable)
        {
            const string SELECTText = "SELECT";

            SQLiteStatement stmt = _Statement;
            string commandText = stmt.CommandText.Trim();
            if (string.Compare(commandText, 0, SELECTText, 0, SELECTText.Length, true) != 0) return;

            // The first step is to determine the table name.
            string baseTableName = FindBaseTableName(commandText);

            // BaseTableName was not found -> nothing to enhance
            if (baseTableName == null) return;

            // update BaseTableName column in the schema table
            foreach (DataRow row in schemaTable.Rows)
                row["BaseTableName"] = baseTableName;

            // the second step is to get the information about the table
            IDbCommand cmd = _Command.Connection.CreateCommand();
            cmd.CommandText = "PRAGMA table_info(" + baseTableName + ")";
            IDataReader tableInfoReader = cmd.ExecuteReader();
            
            try
            {
                while (tableInfoReader.Read())
                {
                    string columnName = Convert.ToString(tableInfoReader["name"]);
                    DataRow row = FindRow(schemaTable, columnName);
                    if (row != null)
                    {
                        string type = Convert.ToString(tableInfoReader["type"]);
                        bool notNull = Convert.ToInt32(tableInfoReader["notnull"]) != 0;
                        bool primaryKey = Convert.ToInt32(tableInfoReader["pk"]) != 0;

                        row["AllowDBNull"] = !(primaryKey || notNull);
                        row["IsUnique"] = primaryKey;
                        row["IsKey"] = primaryKey;
                        row["IsAutoIncrement"] = primaryKey && string.Compare(type, "INTEGER", true) == 0;
                    }
                }
            }
            finally
            {
                tableInfoReader.Dispose();
            }
        }

        private static string FindBaseTableName(string commandText)
        {
            const string FROMText = "FROM";

            int parenthesisLevel = 0;
            bool insideQuote = false;

            for (int i = 0; i < commandText.Length - FROMText.Length - 1; i++)
            {
                char c = commandText[i];
                if (c == '(')
                {
                    if (!insideQuote) parenthesisLevel++;
                }
                else if (c == ')')
                {
                    if (!insideQuote)
                    {
                        if (--parenthesisLevel < 0) throw new SQLiteException("Unmatched parenthesis at the position #" + i);
                    }
                }
                else if (c == '\'') insideQuote = !insideQuote;
                else if (c == 'F' || c == 'f')
                {
                    if (!insideQuote && parenthesisLevel == 0 &&
                        string.Compare(commandText.Substring(i, FROMText.Length), FROMText, true) == 0)
                    {
                        string[] tokens = commandText.Substring(i + FROMText.Length).Trim().Split(null);
                        if (tokens.Length > 0)
                            return tokens[0];
                    }
                }
            }

            return null;
        }

        private static DataRow FindRow(DataTable table, string columnName)
        {
            foreach (DataRow row in table.Rows)
            {
                if (string.Compare(Convert.ToString(row["ColumnName"]), columnName, true) == 0) return row;
            }
            return null;
        }

        private static DbType GetProviderType(Type type)
        {
            if (type == typeof(String))
                return DbType.String;
            if (type == typeof(Double))
                return DbType.Double;
            if (type == typeof(Int16))
                return DbType.Int16;
            if (type == typeof(Int32))
                return DbType.Int32;
            if (type == typeof(Int64))
                return DbType.Int64;
            if (type == typeof(Single))
                return DbType.Single;
            if (type == typeof(Boolean))
                return DbType.Boolean;
            if (type == typeof(Decimal))
                return DbType.Decimal;
            if (type == typeof(DateTime))
                return DbType.DateTime;
            if (type == typeof(Byte[]))
                return DbType.Binary;
            
            return DbType.String;
        }

        #region internal DbEnumerator
        class DbEnumerator : IEnumerator, IDisposable
        {
            private IDataReader _Reader;
            private bool _CloseReader;

            public DbEnumerator(IDataReader reader, bool closeReader)
            {
                _Reader = reader;
                _CloseReader = closeReader;
            }

            #region IEnumerator Members

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public object Current
            {
                get
                {
                    return _Reader;
                }
            }

            public bool MoveNext()
            {
                bool success = _Reader.Read();
                
                if (!success && _CloseReader) _Reader.Close();
                
                return success;
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                if (_CloseReader && _Reader != null)
                {
                    _Reader.Dispose();
                    _Reader = null;
                }
            }

            #endregion
        }
        #endregion
    }
}
