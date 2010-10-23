#region Using directives

using System;
using System.Collections;

#endregion

namespace System.Data.SQLiteClient
{
    internal class SQLiteStatementCollection : CollectionBase, IDisposable
    {
        public SQLiteStatementCollection()
        {
        }

        public void Add(SQLiteStatement statement)
        {
            List.Add(statement);
        }

        public SQLiteStatement this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException();

                return List[index] as SQLiteStatement;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            for (int i = 0; i < Count; ++i)
            {
                SQLiteStatement statement = this[i];

                statement.Dispose();
            }
        }

        #endregion
    }
}
