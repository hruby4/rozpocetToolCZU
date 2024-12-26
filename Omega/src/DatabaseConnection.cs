using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Omega
{
    public static class DatabaseConnection
    {
        private static MySqlConnection connection = null;
        private static readonly object lockObject = new object();

        /// <summary>
        /// Method <c>getConnection</c> ensures a single instance of the connection is created and opened.
        /// If the connection already exists and is closed, it will be reopened.
        /// </summary>
        /// <returns>
        /// A MySqlConnection object, or null if an exception occurs.
        /// </returns>
        public static MySqlConnection getConnection()
        {
            lock (lockObject)
            {
                if (connection == null)
                {
                    var connectionString = new MySqlConnectionStringBuilder
                    {
                        Server = ConfigReader.getAddress(),
                        Port = ConfigReader.getPort(),
                        UserID = ConfigReader.getUser(),
                        Password = ConfigReader.getPassword(),
                        Database = ConfigReader.getSchema()
                    };
                    connection = new MySqlConnection(connectionString.ConnectionString);
                    Console.WriteLine(connectionString.ToString());
                }
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    try { connection.Open(); }
                    catch { throw new Exception("Nelze se pripojit k databazi"); }
                }
            }
            return connection;
        }

        /// <summary>
        /// Method <c>closeConnection</c> closes database connection
        /// </summary>
        public static void closeConnection() {
            if (connection != null) {
                connection.Close();
                connection.Dispose();
            }
        }
    }
 }

