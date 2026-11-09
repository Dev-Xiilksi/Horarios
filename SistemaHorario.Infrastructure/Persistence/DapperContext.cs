using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace SistemaHorario.Infrastructure.Persistence
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IDbConnection Crearconexion()
        {
            return new SqlConnection(_configuration.GetConnectionString("connection"));
        }
    }
}
