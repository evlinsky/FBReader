using System;
using System.Text;
using System.Globalization;

namespace System.Data.SQLiteClient
{
    public enum SynchronousMode
    {
        Normal = 0,
        Full,
        Off
    }

    public enum DateTimeFormat
    {
        ISO8601 = 0,
        Ticks,
        CurrentCulture
    }

    public class SQLiteConnectionStringBuilder
    {
        private string _DataSource;
        private bool _NewDatabase;
        private bool _Emulator;
        private Encoding _Encoding;
        private int _CacheSize;
        private SynchronousMode _Synchronous;
        private DateTimeFormat _DateTimeFormat;


        public SQLiteConnectionStringBuilder()
        {
            _DataSource = String.Empty;
            _NewDatabase = false;
            _Emulator = false;
            _Encoding = Encoding.UTF8;
            _CacheSize = 2000;
            _Synchronous = SynchronousMode.Normal;
            _DateTimeFormat = DateTimeFormat.ISO8601;
        }

        public SQLiteConnectionStringBuilder(string connectionString)
        {
            Parse(connectionString);
        }

        public string DataSource
        {
            get { return _DataSource; }
            set { _DataSource = value; }
        }

        public bool NewDatabase
        {
            get { return _NewDatabase; }
            set { _NewDatabase = value; }
        }

        public bool Emulator
        {
            get { return _Emulator; }
            set { _Emulator = value; }
        }

        public Encoding Encoding
        {
            get { return _Encoding; }
            set { _Encoding = value; }
        }

        public int CacheSize
        {
            get { return _CacheSize; }
            set { _CacheSize = value; }
        }

        public SynchronousMode SynchronousMode
        {
            get { return _Synchronous; }
            set { _Synchronous = value; }
        }

        public DateTimeFormat DateTimeFormat
        {
            get { return _DateTimeFormat; }
            set { _DateTimeFormat = value; }
        }

        private void Parse(string connectionString)
        {
            String helpMessage =
                "Valid parameters:\n" +
                "Data Source=<database file>  (required)\n" +
                "NewDatabase=True|False  (default: False)\n" +
                "Emulator=True|False  (default: False)\n" +
                "Encoding=UTF8|UTF16  (default: UTF8)\n" +
                "Cache Size=<N>  (default: 2000)\n" +
                "Synchronous=Full|Normal|Off  (default: Normal)\n" +
                "DateTimeFormat=ISO8601|Ticks|CurrentCulture  (default: ISO8601)\n";

            // Parse connection string
            String[] parameters = connectionString.Split(";".ToCharArray());
            
            if (parameters.Length == 0) throw new SQLiteException(helpMessage);
            
            for (int i = 0; i < parameters.Length; i++)
            {
                String parameter = parameters[i].Trim();
                if (parameter.Length == 0) continue;

                int index = parameter.IndexOf('=');
                if (index < 0) throw new SQLiteException(helpMessage);

                String paramName = parameter.Substring(0, index).Trim();
                String paramValue = parameter.Substring(index + 1).Trim();

                if (paramName.Equals("Data Source"))_DataSource = paramValue;
                else if (paramName.Equals("NewDatabase")) _NewDatabase = paramValue.ToUpper().Equals("TRUE");
                else if (paramName.Equals("Emulator")) _Emulator = paramValue.ToUpper().Equals("TRUE");
                else if (paramName.Equals("Encoding"))
                {
                    switch (paramValue.ToUpper())
                    {
                        case "UTF8":
                            _Encoding = Encoding.UTF8;
                            break;
                        case "UTF16":
                            _Encoding = Encoding.Unicode;
                            break;
                        default:
                            throw new SQLiteException(string.Format("Unknown encoding specified: {0}", paramValue));
                    }
                }
                else if (paramName.Equals("Synchronous"))
                {
                    switch (paramValue.ToUpper())
                    {
                        case "FULL":
                            _Synchronous = SynchronousMode.Full;
                            break;
                        case "NORMAL":
                            _Synchronous = SynchronousMode.Normal;
                            break;
                        case "OFF":
                            _Synchronous = SynchronousMode.Off;
                            break;
                        default:
                            throw new SQLiteException(string.Format("Unknown synchronisation mode specified: {0}", paramValue));
                    }
                }
                else if (paramName.Equals("Cache Size"))
                {
                    try
                    {
                        _CacheSize = Convert.ToInt32(paramValue);
                    }
                    catch
                    {
                        throw new SQLiteException(string.Format("Invalid cache size specified: {0}", paramValue));
                    }
                }
                else if (paramName.Equals("DateTimeFormat"))
                {
                    switch (paramValue.ToUpper())
                    {
                        case "ISO8601":
                            _DateTimeFormat = DateTimeFormat.ISO8601;
                            break;
                        case "TICKS":
                            _DateTimeFormat = DateTimeFormat.Ticks;
                            break;
                        case "CURRENTCULTURE":
                            _DateTimeFormat = DateTimeFormat.CurrentCulture;
                            break;
                        default:
                            throw new SQLiteException(string.Format("Unknown DateTimeFormat specified: {0}", paramValue));
                    }
                }
                else
                {
                    throw new SQLiteException(string.Format("Unknown parameter specified: {0}.\r\n{1}", paramName, helpMessage));
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat(CultureInfo.InvariantCulture, "Data Source={0}; ", _DataSource);
            builder.AppendFormat(CultureInfo.InvariantCulture, "NewDatabase={0}; ", _NewDatabase);
            builder.AppendFormat(CultureInfo.InvariantCulture, "Emulator={0}; ", _Emulator);

            string encoding = "UTF8";
            if (_Encoding == Encoding.Unicode) encoding = "UTF16";
            builder.AppendFormat(CultureInfo.InvariantCulture, "Encoding={0}; ", encoding);
            builder.AppendFormat(CultureInfo.InvariantCulture, "Cache Size={0}; ", _CacheSize);
            builder.AppendFormat(CultureInfo.InvariantCulture, "Synchronous={0}; ", _Synchronous);
            builder.AppendFormat(CultureInfo.InvariantCulture, "DateTimeFormat={0}; ", _DateTimeFormat);

            return builder.ToString();
        }
    }
}
