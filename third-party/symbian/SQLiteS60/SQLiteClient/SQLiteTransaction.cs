using System;
using System.Data;
using System.Collections;
using System.Text;

namespace System.Data.SQLiteClient
{
	sealed public class SQLiteTransaction : IDbTransaction, IDisposable
	{
		private SQLiteConnection _Connection;

        internal SQLiteTransaction(SQLiteConnection connection)
        {
            _Connection = connection;
            if (_Connection == null) throw new ArgumentNullException("Connection cannot be null.");

            Execute("BEGIN");
        }

        public void Dispose()
		{
            if (_Connection != null && _Connection.State != ConnectionState.Closed)
            {
                Rollback();
            }
		}

		IDbConnection IDbTransaction.Connection
		{
			get	{ return Connection; }
		}

		public SQLiteConnection Connection
		{
			get	{ return _Connection; }
		}

		public IsolationLevel IsolationLevel
		{
			get	{ return IsolationLevel.Unspecified; }
		}

		public void Commit()
		{
            Execute("COMMIT");
		}

		public void Rollback()
		{
            Execute("ROLLBACK");
		}

		private void Execute(string command)
		{
            if (_Connection == null || _Connection.State != ConnectionState.Open) throw new SQLiteException("Connection is not open.");

            SQLiteCommand cmd = _Connection.CreateCommand(command);
            cmd.ExecuteNonQuery();
		}
	}
}