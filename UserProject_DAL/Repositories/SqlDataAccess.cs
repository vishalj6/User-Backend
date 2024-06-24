using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace UserProject_DAL.Repositories
{
    public class SqlDataAccess
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public SqlDataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        public SqlConnection GetConnection()
        {
            SqlConnection connect = new(_connectionString);
            return connect;
        }
    }
}
