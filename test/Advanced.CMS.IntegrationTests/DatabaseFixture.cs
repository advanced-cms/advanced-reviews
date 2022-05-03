using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Data.SqlClient;

namespace Advanced.CMS.IntegrationTests
{
    public abstract class DatabaseFixture : IDisposable
    {
        protected abstract string CMS_MDF_FILE_PATH { get; }
        protected abstract string DESTINATION_MDF_PATH { get; }
        protected abstract string CONNECTION_STRING_TEMPLATE { get; }

        public string DatabaseName { get; set; }

        protected DatabaseFixture()
        {
            DatabaseName = $"NC_Test_{DateTime.Now.Ticks}";
        }

        protected void SetFolderAccess()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            try
            {
                var dir = new DirectoryInfo(Path.GetDirectoryName(DESTINATION_MDF_PATH));
                var sec = dir.GetAccessControl();
                var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                sec.AddAccessRule(new FileSystemAccessRule(everyone,
                    FileSystemRights.Modify | FileSystemRights.Synchronize,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None,
                    AccessControlType.Allow));
                dir.SetAccessControl(sec);
            }
            catch
            {

            }
        }

        protected void CopyDatabaseFiles()
        {
            EnsureDictionary(new DirectoryInfo(Path.GetDirectoryName(DESTINATION_MDF_PATH)));
            if (File.Exists(DESTINATION_MDF_PATH))
            {
                Thread.Sleep(5000);
            }

            File.Copy(CMS_MDF_FILE_PATH, DESTINATION_MDF_PATH, true);

            if (File.Exists(DESTINATION_MDF_PATH.Replace(".mdf", "_log.ldf")))
            {
                File.Delete(DESTINATION_MDF_PATH.Replace(".mdf", "_log.ldf"));
            }
        }

        public void Dispose()
        {
            TearDownDatabase(DatabaseName);
        }

        protected void DropExistingDatabases(string mdfPath)
        {
            var listSql = @$"
SELECT db.name, mf.physical_name
FROM sys.master_files mf, sys.databases db
WHERE mf.database_id = db.database_id
AND mf.physical_name = '{Path.GetFullPath(mdfPath)}'
";
            var databases = ExecuteSqlQuery(ConnectionString, listSql, (reader) => Tuple.Create(reader[0].ToString(), reader[1].ToString()));
            foreach (var database in databases)
            {
                try
                {   //This throws but database is dropped
                    ExecuteSqlCommand(ConnectionString, $"DROP DATABASE {database.Item1}");
                }
                catch { }
            }

        }

        protected void CreateDatabase(string mdfPath)
        {
            ExecuteSqlCommand(ConnectionString, $@"CREATE DATABASE [{DatabaseName}]
                            ON PRIMARY ( FILENAME =  '{Path.GetFullPath(mdfPath)}' )
                            FOR ATTACH");
            ExecuteSqlCommand(ConnectionString, $@"ALTER DATABASE[{DatabaseName}] SET READ_WRITE WITH NO_WAIT");
        }

        protected void TearDownDatabase(string databaseName)
        {
            var fileNames = ExecuteSqlQuery(ConnectionString, $@"SELECT [physical_name] FROM [sys].[master_files] WHERE [database_id] = DB_ID('{databaseName}')", row => (string)row["physical_name"]);

            if (fileNames.Any())
            {
                ExecuteSqlCommand(ConnectionString, $@"ALTER DATABASE [{databaseName}]
                            SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                            EXEC sp_detach_db '{databaseName}'");
                fileNames.ForEach(File.Delete);
            }
        }

        protected static void ExecuteSqlCommand(SqlConnectionStringBuilder stringBuilder, string commandText)
        {
            using (var connection = new SqlConnection(stringBuilder.ToString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
        }

        protected static List<T> ExecuteSqlQuery<T>(SqlConnectionStringBuilder stringBuilder, string queryText, Func<SqlDataReader, T> read)
        {
            var result = new List<T>();
            using (var connection = new SqlConnection(stringBuilder.ToString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryText;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(read(reader));
                        }
                    }
                }
            }

            return result;
        }

        protected static SqlConnectionStringBuilder ConnectionString => new()
        {
            DataSource = @"(LocalDb)\MSSQLLocalDB",
            InitialCatalog = "master",
            IntegratedSecurity = true
        };

        private static void EnsureDictionary(DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Parent.Exists)
            {
                EnsureDictionary(directoryInfo.Parent);
            }

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }
    }
}
