using System;
using System.Collections;

namespace System.Data.SQLiteClient
{
    public class SQLiteCommandCollection : CollectionBase, IDisposable
    {
        public SQLiteCommandCollection()
        {
        }

        public void Add(SQLiteCommand command)
        {
            List.Add(command);
        }

        public void Remove(SQLiteCommand command)
        {
            List.Remove(command);
        }

        public SQLiteCommand this[int index]
        {
            get
            {
                if (index < 0 || index >= Count) throw new IndexOutOfRangeException();

                return List[index] as SQLiteCommand;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            for (int i = 0; i < Count; ++i)
            {
                SQLiteCommand command = this[i];

                command.Dispose();
            }
        }

        #endregion
    }
}
