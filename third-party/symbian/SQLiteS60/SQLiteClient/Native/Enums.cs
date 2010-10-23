#region Using directives

using System;

#endregion

namespace System.Data.SQLiteClient.Native
{
    internal enum SQLiteCode
    {
        Ok = 0,                         /* Successful result */
        Error = 1,                      /* SQL error or missing database */
        InternalError = 2,              /* An internal logic error in SQLite */
        PermissionDenied = 3,            /* Access permission denied */
        CallbackAbort = 4,              /* Callback routine requested an abort */
        Busy = 5,                       /* The database file is locked */
        TableLocked = 6,                /* A table in the database is locked */
        NoMemory = 7,                   /* A malloc() failed */
        ReadOnly = 8,                   /* Attempt to write a readonly database */
        Interupt = 9,                   /* Operation terminated by public const int interrupt() */
        IOError = 10,                   /* Some kind of disk I/O error occurred */
        DatabaseCorrupt = 11,           /* The database disk image is malformed */
        DatabaseNotFound = 12,          /* (Internal Only) Table or record not found */
        DatabaseFull = 13,              /* Insertion failed because database is full */
        DatabaseCannotOpened = 14,      /* Unable to open the database file */
        DatabaseLockProtocolError = 15, /* Database lock protocol error */
        TableEmpty = 16,                /* (Internal Only) Database table is empty */
        DatabaseSchemaChanged = 17,     /* The database schema changed */
        RowToBig = 18,                  /* Too much data for one row of a table */
        ContraintViolation = 19,        /* Abort due to contraint violation */
        DataTypeMismatch = 20,          /* Data type mismatch */
        LibraryUsedIncorrectly = 21,    /* Library used incorrectly */
        FeatureNotSupported = 22,       /* Uses OS features not supported on host */
        AuthorizationDenied = 23,       /* Authorization denied */
        DatabaseFormatError = 24,       /* Auxiliary database format error */
        OutOfRange = 25,                /* 2nd parameter to sqlite_bind out of range */
        NoDatabaseFile = 26,            /* File opened that is not a database file */
        RowReady = 100,                 /* sqlite_step() has another row ready */
        Done = 101,                     /* sqlite_step() has finished executing */
    }

    internal enum SQLiteDestructor
    {
        Static = 0,
        Transient = -1
    }

    internal enum SQLiteType
    {
        Integer = 1,
        Float = 2,
        Text = 3,
        Blob = 4,
        Null = 5,
    }
}
