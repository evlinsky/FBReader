using System;
using System.Data;
using System.Collections;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Data.SQLiteClient.Native;
using System.Globalization;

namespace System.Data.SQLiteClient
{

	sealed public class SQLiteConnection : IDbConnection, IDisposable
	{
		private ConnectionState _State;
        private SQLiteCommandCollection _Commands;
		private String _ConnectionString;
		private int _DataReaderCount;
        private IFormatProvider _UniversalProvider;
        private int _LastChangesCount;
        private NativeMethods _NativeMethods;
        private SQLiteConnectionStringBuilder _ConnectionStringBuilder;
        private string _Version;

		public SQLiteConnection()
        {
            _Commands = new SQLiteCommandCollection();
            _ConnectionString = "";
            _DataReaderCount = 0;
            _UniversalProvider = CultureInfo.InvariantCulture;
            _LastChangesCount = 0;
            _State = ConnectionState.Closed;
            _NativeMethods = null;
            _Version = "";
        }

		public SQLiteConnection(String connectionString) : this()
		{
            _ConnectionStringBuilder = new SQLiteConnectionStringBuilder(connectionString);
			ConnectionString = connectionString;
		}

        public SQLiteConnection(SQLiteConnectionStringBuilder builder) : this()
        {
            _ConnectionStringBuilder = builder;
            _ConnectionString = builder.ToString();
        }

		public void Dispose()
		{
			Close();
        }

        internal NativeMethods NativeMethods
        {
            get { return _NativeMethods; }
        }

        internal DateTimeFormat DateTimeFormat
        {
            get { return _ConnectionStringBuilder.DateTimeFormat; }
        }

        internal IFormatProvider UniversalFormatProvider
        {
            get { return _UniversalProvider; }
        }

        internal int LastChangesCount
        {
            get { return _LastChangesCount; }
            set { _LastChangesCount = value; }
        }

        public string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {
                if (value == null || value == "") throw new ArgumentNullException("ConnectionString cannot be null or empty.");
                if (_State != ConnectionState.Closed) throw new SQLiteException("The connection is allready opened.");

                _ConnectionString = value;
                _ConnectionStringBuilder = new SQLiteConnectionStringBuilder(value);
            }
        }

        public int ConnectionTimeout
        {
            get { return 0; }
        }

        public String Database
        {
            get { return _ConnectionStringBuilder.DataSource; }
        }

        public ConnectionState State
        {
            get { return _State; }
        }

        public string SQLiteVersion
        {
            get { return _Version; }
        }

        internal void AttachCommand(SQLiteCommand command)
		{
			_Commands.Add(command);
		}

        internal void DetachCommand(SQLiteCommand command)
		{
            _Commands.Remove(command);
		}

		internal void AttachDataReader(SQLiteDataReader reader)
		{
            _DataReaderCount ++;
			_State = ConnectionState.Executing;
		}

		internal void DetachDataReader(SQLiteDataReader reader)
		{
            _DataReaderCount--;
            if (_DataReaderCount == 0 && _State == ConnectionState.Executing) _State = ConnectionState.Open;
			if((reader._CommandBehavior & CommandBehavior.CloseConnection) != 0)
			{
                if (_DataReaderCount == 0)
                {
                    Close();
                }
                else
                {
                    throw new SQLiteException("There is allready another DataReader opened. Connection cannot be closed by closing DataReader.");
                }
			}
		}


		IDbTransaction IDbConnection.BeginTransaction()
		{
			return BeginTransaction();
		}

		IDbTransaction IDbConnection.BeginTransaction(IsolationLevel level)
		{
			return BeginTransaction(level);
		}

		public SQLiteTransaction BeginTransaction()
		{
			if (_State != ConnectionState.Open)
			{
				throw new SQLiteException("The connection is not opened.");
			}
			
            return new SQLiteTransaction(this);
		}

		public SQLiteTransaction BeginTransaction(IsolationLevel level)
		{
            return BeginTransaction();
		}

		public void ChangeDatabase(String newDatabase)
		{
			throw new NotSupportedException();
		}

		public void Open()
		{
            if (_State != ConnectionState.Closed) throw new SQLiteException("The connection is not closed.");

			if (File.Exists(_ConnectionStringBuilder.DataSource))
			{
				if (_ConnectionStringBuilder.NewDatabase)
				{
					// Try to delete existing file
					try
					{
						File.Delete(_ConnectionStringBuilder.DataSource);
                        File.Delete(String.Concat(_ConnectionStringBuilder.DataSource, "-journal"));
					}
					catch (IOException)
					{
						throw new SQLiteException ("Cannot create new file, the existing file is in use.");
					}
				}
			}
			else
			{
				if (!_ConnectionStringBuilder.NewDatabase)
				{
                    throw new SQLiteException(String.Format("File '{0}' does not exist. Use ConnectionString parameter NewDatabase=True to create new file or enter the full path name.", _ConnectionStringBuilder.DataSource));
				}
			}

            if (_ConnectionStringBuilder.Emulator)
            {
                _NativeMethods = new WindowsMethods(_ConnectionStringBuilder.Encoding);
            }
            else
            {
                _NativeMethods = new SymbianMethods(_ConnectionStringBuilder.Encoding);
            }

            SQLiteCode res = _NativeMethods.Open(_ConnectionStringBuilder.DataSource);
            if (res != SQLiteCode.Ok)
            {
                try
                {
                    if (_NativeMethods.ErrorCode() != SQLiteCode.Ok)
                    {
                        throw new SQLiteException(string.Format("Error opening database {0}.\r\n {1}", _ConnectionStringBuilder.DataSource, _NativeMethods.ErrorMessage()));
                    }
                }
                finally
                {
                    _NativeMethods.Dispose();
                    _NativeMethods = null;
                }
            }

            _Version = _NativeMethods.SQliteVersion();
			_State = ConnectionState.Open;

            SQLiteCommand cmd = CreateCommand(string.Format("pragma synchronous = {0}", _ConnectionStringBuilder.SynchronousMode));
            cmd.ExecuteNonQuery();

            cmd = CreateCommand(string.Format("pragma cache_size = {0}", _ConnectionStringBuilder.CacheSize));
            cmd.ExecuteNonQuery();
		}

		public void Close ()
		{
			if (_State != ConnectionState.Closed)
			{
                _Commands.Dispose();
				_State = ConnectionState.Closed;

                _NativeMethods.Dispose();
            }
		}

		IDbCommand IDbConnection.CreateCommand()
		{
			return new SQLiteCommand ("", this);
		}

		public SQLiteCommand CreateCommand()
		{
			return new SQLiteCommand ("", this);
		}

        public SQLiteCommand CreateCommand(string commandText)
        {
            return new SQLiteCommand(commandText, this);
        }
    }
}
