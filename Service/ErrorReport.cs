using Azure;
using Npgsql;

namespace WebApplication1.Service
{
    public class ErrorReport
    {
        /// <summary>
        /// A classe ErrorReport é responsável por registrar erros em um banco de dados PostgreSQL.
        /// Cada erro gerado no REPOSITORY ou SERVICE pode ser registrado utilizando os métodos desta classe.
        /// Existem três metodos principais:
        ///     AddErrorId: Registra um erro associado a um usuário específico, incluindo o ID do
        ///         usuário, a mensagem de erro e a operação realizada.
        ///     AddErrosAll: Registra um erro geral, sem associação a um usuário específico. Inclui a mensagem
        ///         de erro e a operação realizada.
        ///     AddErrorRegister: Registra erros relacionados ao processo de registro de usuários. Inclui a
        ///         mensagem de erro e a operação realizada.
        /// Todos os metodos armazenam a data e hora do erro automaticamente e a mensagem que retorna do erro gerado.
        /// </summary>
        private readonly string _connectionString;

        public ErrorReport(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// Salva os erros e os associa ao ID do usuário que foi gerado.
        /// </summary>
        /// <param name="message">Mensagem de erro que vem do CATCH.</param>
        /// <param name="userId">O ID do susário referente ao erro.</param>
        /// <param name="operation">Qual operação foi efetuda quando o erro foi gerado.</param>
        public void AddErrorId(string message,int userId,string operation)
        {
            using var cnx = new NpgsqlConnection(_connectionString);
            cnx.Open();
            var cmd = cnx.CreateCommand();
            cmd.CommandText = "INSERT INTO ErrorReports (userId,time, mensage) VALUES (@userId.@time, @Message )";
            cmd.Parameters.Add(new NpgsqlParameter("@Message", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = message });
            cmd.Parameters.Add(new NpgsqlParameter("@time", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = DateTime.Now });
            cmd.Parameters.Add(new NpgsqlParameter("@userId", NpgsqlTypes.NpgsqlDbType.Integer) { Value = userId });
            cmd.Parameters.Add(new NpgsqlParameter("@operation", NpgsqlTypes.NpgsqlDbType.Integer) { Value = operation });
            cmd.ExecuteNonQuery();
            cnx.Close();
        }
        /// <summary>
        /// Salva erros gerais que não estão associados a um usuário específico.
        /// </summary>
        /// <param name="message">Mensagem de erro que vem do CATCH.</param>
        /// <param name="operation">Qual operação foi efetuda quando o erro foi gerado.</param>
        public void AddErrorAll(string message,string operation)
        {
            using var cnx = new NpgsqlConnection(_connectionString);
            cnx.Open();
            var cmd = cnx.CreateCommand();
            cmd.CommandText = "INSERT INTO ErrorReports (time, mensage, operation) VALUES (@time, @Message, @operation )";
            cmd.Parameters.Add(new NpgsqlParameter("@Message", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = message });
            cmd.Parameters.Add(new NpgsqlParameter("@time", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = DateTime.Now });
            cmd.Parameters.Add(new NpgsqlParameter("@operation", NpgsqlTypes.NpgsqlDbType.Integer) { Value = operation });
            cmd.ExecuteNonQuery();
            cnx.Close();
        }
        /// <summary>
        /// Salva erros relacionados ao registro de usuários.
        /// </summary>
        /// <param name="message">Mensagem de erro que vem do CATCH.</param>
        /// <param name="operation">Qual operação foi efetuda quando o erro foi gerado.</param>
        internal void AddErrorRegister(string message, string operation)
        {
            using var cnx = new NpgsqlConnection(_connectionString);
            cnx.Open();
            var cmd = cnx.CreateCommand();
            cmd.CommandText = "INSERT INTO ErrorReports (time, mensage, operation) VALUES (@time, @Message, @operation )";
            cmd.Parameters.Add(new NpgsqlParameter("@Message", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = message });
            cmd.Parameters.Add(new NpgsqlParameter("@time", NpgsqlTypes.NpgsqlDbType.Timestamp) { Value = DateTime.Now });
            cmd.Parameters.Add(new NpgsqlParameter("@operation", NpgsqlTypes.NpgsqlDbType.Integer) { Value = operation });
            cmd.ExecuteNonQuery();
            cnx.Close();
        }
    }
}
