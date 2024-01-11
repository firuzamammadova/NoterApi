using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        SqlTransaction BeginTransaction();
        SqlConnection GetConnection();
        SqlTransaction GetTransaction();

        void SaveChanges();
    }
    public class UnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private bool disposed = false;

        private SqlTransaction sqlTransaction;
        private SqlConnection SqlConnection;



        public UnitOfWork(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            SqlConnection = new SqlConnection(_connectionString);
        }
        public SqlTransaction BeginTransaction()
        {
            if (SqlConnection.State != ConnectionState.Open)
            {
                SqlConnection.Open();
                sqlTransaction = SqlConnection.BeginTransaction();
            }

            return sqlTransaction;
        }

        public SqlConnection GetConnection()
        {
            return SqlConnection;
        }

        public SqlTransaction GetTransaction()
        {
            return sqlTransaction;
        }

        public void SaveChanges()
        {
            sqlTransaction.Commit();
            SqlConnection.Close();
            sqlTransaction = null;
        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    sqlTransaction = null;
                }

                // Release unmanaged resources.
                if (SqlConnection.State == ConnectionState.Open)
                {
                    SqlConnection.Close();
                }
                disposed = true;
            }
        }
        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
