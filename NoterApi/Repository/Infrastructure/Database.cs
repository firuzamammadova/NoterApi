using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Infrastructure
{
    public class Database
    {
        private readonly IUnitOfWork _context;
        public Database(IUnitOfWork context)
        {
            _context = context;
        }
        public void CreateDatabase(string dbName)
        {
            var query = "SELECT * FROM sys.databases WHERE name = @name";
            var parameters = new DynamicParameters();
            parameters.Add("name", dbName);
            using (var connection = _context.GetConnection())
            {
                var records = connection.Query(query, parameters);
                if (!records.Any())
                    connection.Execute($"CREATE DATABASE {dbName}");
            }
        }
    }
}
