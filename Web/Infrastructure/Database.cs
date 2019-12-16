using System;
using System.Data;

namespace Web.Infrastructure
{
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Collections.Generic;

    public class Database
    {
        // Replace this with a connection pooling system to reduce the opens and number of concurrent connections to 
        // a managed limit.

        private readonly SqlConnection _connection;

        public Database(String ConnectionString)
        {
            _connection = new SqlConnection(ConnectionString);
        }

        ~Database()
        {
            // Make sure the connection is closed in case the uninit method is not called
            uninit();
        }

        /// <summary>
        /// Initialize the database connection.
        /// </summary>
        public void init()
        {
            _connection.Open();
        }

        /// <summary>
        /// Close the database connection.
        /// </summary>
        public void uninit()
        {
            // Close the connection to the database
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Return the execute reader for the query
        /// </summary>
        /// <param name="query"></param>
        /// <returns>The execute reader for the command</returns>
        public DbDataReader ExecuteReader(string query)
        {
            var sqlQuery = new SqlCommand(query, _connection);

            return sqlQuery.ExecuteReader();
        }


        public DbDataReader ExecuteReader(string query, Dictionary<string, string> args)
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
