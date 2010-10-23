using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Data.SQLiteClient.Native;

namespace System.Data.SQLiteClient
{
	sealed public class SQLiteParameter : IDbDataParameter
	{
		private DbType _DbType;
		private String _ParameterName;
		private String _SourceColumn;
		private DataRowVersion _SourceVersion;
		private Object _Value;
		private int _Size;

		public SQLiteParameter() : this(string.Empty, DbType.Object, null)
		{
		}

		public SQLiteParameter (String name, DbType type) : this(name, type, null)
		{
		}

        public SQLiteParameter(String name, DbType type, object value)
        {
            _DbType = type;
            _ParameterName = name;
            _SourceColumn = "";
            _SourceVersion = DataRowVersion.Default;
            _Value = value;
            _Size = 0;
        }

        public DbType DbType
		{
			get { return _DbType; }
			set	{ _DbType = value; }
		}

		public ParameterDirection Direction
		{
			get	{ return ParameterDirection.Input; }
			set
			{
				if (value != ParameterDirection.Input) throw new SQLiteException("Only ParameterDirection=Input is allowed.");
			}
		}

		public bool IsNullable
		{
			get { return false;	}
		}

		public String ParameterName
		{
			get	{ return _ParameterName; }
			set	{ _ParameterName = value; }
		}

		public String SourceColumn
		{
			get	{ return _SourceColumn;	}
			set	
            { 
                if (value == null) throw new ArgumentNullException("SourceColumn cannot be null.");
				_SourceColumn = value;
			}
		}

		public DataRowVersion SourceVersion
		{
			get	{ return _SourceVersion; }
			set { _SourceVersion = value; }
		}

		public Object Value
		{
			get	{ return _Value; }
			set	{ _Value = value; }
		}

		public Byte Precision
		{
			get { throw new NotSupportedException(); }
			set	{ throw new NotSupportedException(); }
		}

		public Byte Scale
		{
			get	{ throw new NotSupportedException(); }
			set	{ throw new NotSupportedException(); }
		}

		public int Size
		{
			get { return _Size;	}
			set	{ _Size = value; }
		}
	}
}
