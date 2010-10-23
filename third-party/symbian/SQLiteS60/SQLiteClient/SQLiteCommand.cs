using System;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data.SQLiteClient.Native;
using System.Windows.Forms;

namespace System.Data.SQLiteClient
{
	sealed public class SQLiteCommand : IDbCommand
	{
        private const char Quote = '\'';
        private const char DoubleQuote = '"';
        private const char StatementTerminator = ';';
        private const char NamedParameterBeginChar = '@';
	    private const char CreateTriggerBeginChar1 = 'C';
	    private const char CreateTriggerBeginChar2 = 'c';
        //private const char UnnamedParameterBeginChar = '?';

        private String _CommandText;
		private UpdateRowSource _UpdatedRowSource;
		private SQLiteParameterCollection _Parameters;
        private SQLiteStatementCollection _Statements;
		private int _Timeout;
		private SQLiteConnection _Connection;
		private bool _ServingDataReader;
        private SQLiteTransaction _Transaction;
		private static Regex _CreateTriggerRegEx = new Regex(@"(?i)CREATE\s+(?:TEMP\w*\s+)*TRIGGER", RegexOptions.Compiled);

		public SQLiteCommand()
        {
            _CommandText = String.Empty;
            _UpdatedRowSource = UpdateRowSource.Both;
            _Timeout = 30;
            _ServingDataReader = false;
            _Transaction = null;

            _Parameters = new SQLiteParameterCollection();
            _Statements = new SQLiteStatementCollection();
        }

		public SQLiteCommand(String commandText) : this()
		{
			CommandText = commandText;
		}

		public SQLiteCommand(String commandText, SQLiteConnection connection) : this(commandText)
		{
			if (connection == null)	throw new ArgumentNullException("Connection is null.");
			
            Connection = connection;
		}

		#region IDisposable members
		public void Dispose()
		{
            _Statements.Dispose();

            if (_Connection != null) _Connection.DetachCommand(this);
		}
		#endregion

		#region IDbCommand members
		public String CommandText
		{
			get	{ return _CommandText; }
			set
			{
                if (value == null) throw new ArgumentNullException("CommandText cannot be null.");
                
                if (_Connection != null && _ServingDataReader) throw new InvalidOperationException("Can not set CommandText when the connection is busy serving data readers");
				
                _Statements.Dispose();

				_CommandText = value;

				ParseCommand(_CommandText);
			}
		}

		public int CommandTimeout
		{
			get	{ return _Timeout; }
			set
			{
				if (value < 0) throw new SQLiteException("CommandTimeout must be greater 0.");
				
                _Timeout = value;
			}
		}

		public CommandType CommandType
		{
			get
			{
				return CommandType.Text;
			}
			set
			{
                if (value != CommandType.Text) throw new SQLiteException("Only CommandType.Text is supported.");
			}
		}

		IDbConnection IDbCommand.Connection
		{
			get	{ return Connection; }
			set 
            {
                SQLiteConnection connection = value as SQLiteConnection;
                if (connection == null) throw new ArgumentNullException("Connection is null.");
                
                Connection = value as SQLiteConnection;
			}
		}

		public SQLiteConnection Connection
		{
			get	{ return _Connection; }
			set
			{
				if (_Connection != null)
				{
                    if (_Connection.State == ConnectionState.Executing)
                    {
                        throw new InvalidOperationException("Connection is currently executing an operation.");
                    }

                    _Statements.Dispose();
                    _Connection.DetachCommand(this);
				}

				if (value == null) _Connection = null;
				else
				{
					_Connection = value;
					_Connection.AttachCommand(this);
				}
			}
		}

		IDataParameterCollection IDbCommand.Parameters
		{
            get { return _Parameters; }
		}

		public SQLiteParameterCollection Parameters
		{
			get	{ return _Parameters; }
		}

		IDbTransaction IDbCommand.Transaction
		{
			get	{ return Transaction; }
            set { Transaction = value as SQLiteTransaction; }
		}

		public SQLiteTransaction Transaction
		{
            get { return _Transaction; }
            set { _Transaction = value;	}
		}

		public UpdateRowSource UpdatedRowSource
		{
			get
			{
				return _UpdatedRowSource;
			}
			set
			{
                if (_Connection != null && _Connection.State == ConnectionState.Executing)
                {
                    throw new InvalidOperationException("Connection is currently executing an operation.");
                }

                _UpdatedRowSource = value;
			}
		}

		public void Cancel()
		{
			throw new NotSupportedException();
		}

		IDbDataParameter IDbCommand.CreateParameter()
		{
			return CreateParameter();
		}

		public SQLiteParameter CreateParameter()
		{
			return new SQLiteParameter(null, DbType.AnsiString);
		}

		public int ExecuteNonQuery ()
		{
			int result = 0;
			for(int i=0 ; i < _Statements.Count; ++i)
			{
				SQLiteStatement statement = _Statements[i];

                statement.Compile();

                _Connection.NativeMethods.busy_timeout(_Timeout);

                SQLiteCode res = statement.Step();

                if (res == SQLiteCode.Done || res == SQLiteCode.RowReady)
				{
					statement.Reset();

                    result += _Connection.LastChangesCount;
				}
                else if (res == SQLiteCode.Error)
                {
                    statement.Dispose(); // UnCompile will throw exception with appropriate msg.
                }
                else
                {
                    throw new SQLiteException("Unknown SQLite error");
                }
			}

			return result;
		}

		IDataReader IDbCommand.ExecuteReader()
		{
			return ExecuteReader();
		}

		IDataReader IDbCommand.ExecuteReader(CommandBehavior cmdBehavior)
		{
			return ExecuteReader(cmdBehavior);
		}

		public SQLiteDataReader ExecuteReader()
		{
			return ExecuteReader(CommandBehavior.Default);
		}

		public SQLiteDataReader ExecuteReader(CommandBehavior cmdBehavior)
		{
			if (_Connection == null || !(_Connection.State == ConnectionState.Open || _Connection.State == ConnectionState.Executing))
				throw new InvalidOperationException("The connection must be open to call ExecuteReader");
			
            return new SQLiteDataReader(this, cmdBehavior);
		}

		public Object ExecuteScalar()
		{
			// ExecuteReader and get value of first column of first row
			IDataReader reader = ExecuteReader();
			Object obj = null;

            try
            {
                if (reader.Read() && reader.FieldCount > 0)
                {
                    obj = reader.GetValue(0);
                }
            }
            finally
            {
                reader.Close();
            }
			
            return obj;
		}

		public void Prepare()
		{
		}
		#endregion

		private static void AppendUpToFoundIndex(StringBuilder sb, string cmd, int foundIndex, ref int index)
		{
			if( foundIndex < 0 ) foundIndex = cmd.Length;
			sb.Append( cmd, index, foundIndex-index );
			index = foundIndex;
		}

		private static void AppendUpToSameChar(StringBuilder sb, string cmd, int foundIndex, out int nextIndex)
		{
			int i = cmd.IndexOf(cmd[foundIndex], foundIndex+1);
			if( i < 0 )
			{
				sb.Append(cmd,foundIndex,cmd.Length-foundIndex);
				nextIndex = cmd.Length;
			}
			else
			{
				sb.Append(cmd,foundIndex,i-foundIndex+1);
				nextIndex = i+1;
			}
		}

		private static void AppendNamedParameter(StringBuilder sb, string cmd, int foundIndex, ArrayList paramNames, out int index)
		{
			index = foundIndex+1; 
			do
			{
				char ch = cmd[index];
				
                if( !(Char.IsLetterOrDigit(ch) || ch == '_') )
					break;
				
                index++;
			}
			while( index < cmd.Length );
			
            sb.Append('?');
			paramNames.Add(cmd.Substring(foundIndex,index-foundIndex));
		}

		private void TerminateStatement(StringBuilder sb, ref ArrayList paramNames)
		{
			// One SQL statement is terminated.
			// Add this statement to the collection and
			// reset all string builders
			string cmd = sb.ToString().Trim();
			if( cmd.Length > 0 ) _Statements.Add(new SQLiteStatement(this, cmd, paramNames));

            sb.Length = 0;
			
            paramNames = new ArrayList();
		}

		private static void AppendTrigger(StringBuilder sb, string cmd, int foundIndex, out int index)
		{
			if( !_CreateTriggerRegEx.IsMatch(cmd,foundIndex) )
			{
				sb.Append(cmd[foundIndex]);
				index = foundIndex+1;
				return;
			}

			const string END = "END";

			// search the keyword END
			char[] Terminators = 
				{
					Quote,
					DoubleQuote,
					'e',
					'E'
				};

			index = foundIndex;
			while( index < cmd.Length )
			{
				foundIndex = cmd.IndexOfAny(Terminators,index);
				AppendUpToFoundIndex(sb,cmd,foundIndex,ref index);
				if( foundIndex < 0 )
					break;

				switch( cmd[foundIndex] )
				{
                    case Quote:
                    case DoubleQuote:
						AppendUpToSameChar(sb,cmd,foundIndex,out index);
						break;

					case 'e':
					case 'E':
						if(string.Compare(cmd,foundIndex,END,0,END.Length,true,CultureInfo.InvariantCulture) == 0)
						{
							sb.Append(cmd,foundIndex,END.Length);
							index = foundIndex+END.Length+1;
							return;
						}
						else
						{
							sb.Append(cmd[foundIndex]);
							index = foundIndex+1;
						}
						break;
				}
			}
		}

		/// <summary>
		/// parse the command text and split it into separate SQL commands
		/// </summary>
 		private void ParseCommand(string cmd)
		{
			_Statements.Clear();

			if(cmd.Length == 0)	return;

			char[] Terminators = 
				{	StatementTerminator,
					NamedParameterBeginChar,
					Quote,
					DoubleQuote,
//					UnnamedParameterBeginChar,
					CreateTriggerBeginChar1,
					CreateTriggerBeginChar2
				};

			int index = 0;
			ArrayList paramNames = new ArrayList();
			StringBuilder sb = new StringBuilder(cmd.Length);

			while( index < cmd.Length )
			{
				int foundIndex = cmd.IndexOfAny(Terminators,index);

				AppendUpToFoundIndex(sb,cmd,foundIndex,ref index);
				
                if( foundIndex < 0 ) break;

				switch( cmd[foundIndex] )
				{
                    case Quote:
                    case DoubleQuote:
						AppendUpToSameChar(sb,cmd,foundIndex,out index);
						break;

                    case NamedParameterBeginChar:
						AppendNamedParameter(sb,cmd,foundIndex,paramNames,out index);
						break;

                    //case UnnamedParameterBeginChar:
                    //    paramNames.Add(null);
                    //    sb.Append(UnnamedParameterBeginChar);
                    //    index = foundIndex+1;
                    //    break;

                    case StatementTerminator:
						TerminateStatement(sb,ref paramNames);
						index = foundIndex+1;
						break;

                    case CreateTriggerBeginChar1:
                    case CreateTriggerBeginChar2:
						AppendTrigger(sb,cmd,foundIndex,out index);
						break;

					default:
						throw new SQLiteException(string.Format("Found the unexpected terminator '{0}' at the position {1}.", cmd[foundIndex], foundIndex));
				}
			}
			TerminateStatement(sb,ref paramNames);

			// now iterate all SQL statements and assign 
			// the starting index of unnamed parameters inside Parameters collection
			int unnamedParameterCount = 0;
			for( int i=0 ; i < _Statements.Count ; ++i )
			{
				SQLiteStatement statement = _Statements[i];

				statement.SetUnnamedParametersStartIndex(unnamedParameterCount);

                unnamedParameterCount += statement.GetUnnamedParameterCount();
			}
		}

		internal void AttachDataReader(SQLiteDataReader reader)
		{
			_ServingDataReader = true;

			if(_Connection != null) _Connection.AttachDataReader(reader);
		}

		internal void DetachDataReader(SQLiteDataReader reader)
		{
			if(_Connection != null) _Connection.DetachDataReader(reader);

            _Statements.Dispose();
			_ServingDataReader = false;
		}

        internal SQLiteStatementCollection Statements
        {
            get { return _Statements; }
        }
	}
}
