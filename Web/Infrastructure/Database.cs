using System;
using System.Data;

namespace Web.Infrastructure
{
    using System.Data.SqlClient;
    using System.Collections.Generic;

    public class Database : IStorage
    {
        // Replace this with a connection pooling system to reduce the opens and number of concurrent connections to 
        // a managed limit.
        private const string DB_CONNECTION_STRING = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=BrainWAre;Integrated Security=SSPI;AttachDBFilename=e:\\BrainWare\\Web\\App_Data\\BrainWare.mdf";

        private readonly SqlConnection _connection;

        public Database()
        {
            _connection = new SqlConnection(DB_CONNECTION_STRING);
            _connection.Open();
        }

        ~Database()
        {
            // Close the connection to the database
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Return the executed query reader
        /// </summary>
        /// <param name="query"></param>
        /// <returns>The execute reader for the command</returns>
        public IDataReader ExecuteQuery(string query)
        {
            var sqlQuery = new SqlCommand(query, _connection);

            return sqlQuery.ExecuteReader();
        }


        public IDataReader ExecuteQuery(string query, Dictionary<string, string> args)
        {
            var sqlQuery = new SqlCommand(query, _connection);
            if(args != null && args.Count > 0)
            {
                foreach(var arg in args.Keys)
                {
                    sqlQuery.Parameters.AddWithValue(arg, args[arg]);
                }
            }
            else
            {
                throw new Exception("Bad Args for Sql Query Suppiled.");
            }

            return sqlQuery.ExecuteReader();
        }
    }
}
