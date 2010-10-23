using System;

namespace System.Data.SQLiteClient
{
	public class SQLiteException : Exception
	{
		internal SQLiteException (String message) : base(message) 
        {
        }
	}
}
