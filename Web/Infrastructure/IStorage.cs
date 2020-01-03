using System.Collections.Generic;
using System.Data;

namespace Web.Infrastructure
{
    public interface IStorage
    {
        IDataReader ExecuteQuery(string query);

        IDataReader ExecuteQuery(string query, Dictionary<string, string> args);

    }
}
