using Microsoft.Data.SqlClient;
using System.Data;

namespace WebApplication1.Data
{
    /// <summary>
    /// O DbContext é responsável por gerenciar a conexão com o banco de dados.
    /// Essa classe pode e é importada em repositórios para executar operações de banco de dados.
    /// </summary>
    public class DbContext
    {
        private readonly string _connectionString;

        /// <summary>
        /// A conctionString deve ser passada via injeção de dependência.
        /// Ela está configurada no arquivo appsettings.json
        /// </summary>
        /// <param name="connectionString">appsettings.json</param>
        public DbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
