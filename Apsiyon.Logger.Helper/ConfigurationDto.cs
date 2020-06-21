namespace Apsiyon.Logger.Helper
{
    public class ConfigurationDto
    {
        public static Configuration Config { get; set; }

        public class Configuration
        {
            public SQLServer SqlServer { get; set; }
            public File File { get; set; }
            public Sentry Sentry { get; set; }
        }

        public class SQLServer
        {
            public bool Active { get; set; }
            public string ConnectionString { get; set; }
            public string MigrationHistoryTableName { get; set; }
        }

        public class File
        {
            public bool Active { get; set; }
            public string Path { get; set; }
            public string LogName { get; set; }
        }

        public class Sentry
        {
            public bool Active { get; set; }
            public string DSN { get; set; }
        }
    }
}