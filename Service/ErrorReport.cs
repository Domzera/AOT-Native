using Azure;
using Npgsql;

namespace WebApplication1.Service
{
    public class ErrorReport
    {
        private readonly string _connectionString;

        public ErrorReport(string connectionString)
        {
            _connectionString = connectionString;
        }

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
