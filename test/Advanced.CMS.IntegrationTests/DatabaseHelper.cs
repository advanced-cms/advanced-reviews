using System.Data.Common;
using System.IO;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Advanced.CMS.IntegrationTests
{
    public class DatabaseHelper
    {
        private DbProviderFactory _dbFactory;
        private readonly string _connectionString;

        public DatabaseHelper()
          : this(null) { }


        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }


        /// <summary>
        /// The DbProviderFactory for the current provider.
        /// </summary>
        public DbProviderFactory DbFactory => _dbFactory ??= SqlClientFactory.Instance;

        /// <summary>
        /// Get a connection object for the current test database.
        /// </summary>
        /// <returns>An initialized DbConnection object</returns>
        /// <remarks>
        /// Note that the connection is initialized, but not opened.
        /// </remarks>
        public DbConnection GetConnection()
        {
            var conn = DbFactory.CreateConnection();
            conn.ConnectionString = _connectionString;
            return conn;
        }

        /// <summary>
        /// Get an open connection object for the current test database.
        /// </summary>
        /// <returns>An open DbConnection object</returns>
        public DbConnection OpenConnection()
        {
            var conn = GetConnection();
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Execute a series of commands against the current test database
        /// </summary>
        /// <param name="commands">A string array of SQL commands</param>
        /// <remarks>
        /// Each string in the array is executed separately without a surrounding transaction. It is the responsibility of the caller
        /// to use the correct SQL dialect for the current test database.
        /// </remarks>
        public void ExecuteSql(string commands)
        {
            ExecuteSql(new StringReader(commands));
        }

        /// <summary>
        /// Execute a single SQL statement and return the scalar result.
        /// </summary>
        /// <param name="command">A SQL statement</param>
        /// <returns>An object with the scalar result.</returns>
        public object ExecuteScalarSql(string command)
        {
            using (var c = OpenConnection())
            {
                using (var cmd = DbFactory.CreateCommand())
                {
                    cmd.CommandText = command;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = c;
                    return cmd.ExecuteScalar();
                }
            }
        }

        public virtual void ExecuteSqlFile(string fileName)
        {
            using (var s = File.OpenRead(fileName))
            {
                using (var reader = new StreamReader(s))
                {
                    ExecuteSql(reader);
                }
            }
        }

        private void ExecuteSql(TextReader reader)
        {
            using (var c = OpenConnection())
            {
                var line = string.Empty;
                var sqlStatement = new StringBuilder();

                while (line != null)
                {
                    line = reader.ReadLine();
                    if (line == null || line.Trim().ToUpper() == "GO")
                    {
                        if (sqlStatement.Length > 0)
                        {
                            using (var cmd = DbFactory.CreateCommand())
                            {
                                cmd.Connection = c;
                                cmd.CommandText = sqlStatement.ToString();
                                cmd.ExecuteNonQuery();
                            }
                            sqlStatement.Clear();
                        }
                    }
                    else if (line.Length > 0)
                    {
                        sqlStatement.Append(line);
                        sqlStatement.Append("\r\n");
                    }
                }
            }
        }

    }
}
