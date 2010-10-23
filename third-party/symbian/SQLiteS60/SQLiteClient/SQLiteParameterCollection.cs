using System;
using System.Data;
using System.Collections;
using System.Text;

namespace System.Data.SQLiteClient
{
	sealed public class SQLiteParameterCollection : CollectionBase, IDataParameterCollection
	{
		internal SQLiteParameterCollection ()
		{
		}

		public SQLiteParameter this[String pParamName]
		{
			get
			{
				int index = IndexOf(pParamName);
				if( index < 0 )
					return null;
				else
					return InnerList[index] as SQLiteParameter;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		Object IDataParameterCollection.this[String pParamName]
		{
			get
			{
				return this[pParamName];
			}
			set
			{
				this[pParamName] = value as SQLiteParameter;
			}
		}

		public SQLiteParameter this[int index]
		{
			get
			{
				return (SQLiteParameter)(InnerList[index]);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int Add(Object param)
		{
			if( param == null )
				throw new ArgumentNullException();

			if( InnerList.Contains(param) )
				throw new ArgumentException("Parameter is already added into the collection");

			return InnerList.Add((SQLiteParameter)param);
		}

		public SQLiteParameter Add(SQLiteParameter param)
		{
			Add((Object)param);
			return param;
		}

		public SQLiteParameter Add(String parameterName, Object val)
		{
			SQLiteParameter param = Add(parameterName,DbType.String);
			param.Value = val;
			return param;
		}
        
        public SQLiteParameter Add(String parameterName, DbType dbType)
		{
			return Add(parameterName,dbType,0);
		}

		public SQLiteParameter Add(String parameterName, DbType dbType, int size)
		{
			return Add(parameterName,dbType,size,null);
		}

		public SQLiteParameter Add(String parameterName, DbType dbType, int size, String sourceColumn)
		{
			SQLiteParameter param = new SQLiteParameter(parameterName,dbType);
			param.Size = size;
			if( sourceColumn != null )
				param.SourceColumn = sourceColumn;
			return Add(param);
		}

		public bool Contains (String parameterName)
		{
			return IndexOf(parameterName) >= 0;
		}

		public int IndexOf (String parameterName)
		{
			if( parameterName == null )
				return -1;

			int count = InnerList.Count;
			for( int i=0 ; i < count ; ++i )
			{
				String name = this[i].ParameterName;
				if( name != null && String.Compare(name,parameterName,true,System.Globalization.CultureInfo.InvariantCulture) == 0 )
					return i;
			}
			return -1;
		}

		public void RemoveAt (String parameterName)
		{
			throw new NotSupportedException();
		}
	}
}
