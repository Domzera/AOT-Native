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
        /// <param name="connectionString">A ConnectionString está no arquivo appsettings.json.</param>
        public DbContext(string connectionString)
        {
            _connectionString = connectionString;

        }

        public IDbConnection CreateConnection()
        {
            ///<summary>
            ///Testa a conexão com o banco de dados.
            ///</summary>
            SqlConnection cnx = new SqlConnection(_connectionString);
            try
            {
                cnx.Open();
                Console.WriteLine("Conexão com o banco de dados estabelecida com sucesso.");
                ///<summary>
                ///Caso a coneção seja estabelecida com sucesso, retorna a conexão.
                ///</summary>
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Erro ao conectar ao banco de dados: " + ex.Message);
                ///<summary>
                ///caso contrário, lança uma exceção e cria o banco de dados.
                ///</summary>
                cnx.Open();
                var cmd = cnx.CreateCommand();
                //Database=userDataBase;Username=postgres;Password=LuizBd25"
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS public.users\r\n(\r\n " +
                                    "\"userId\" integer NOT NULL DEFAULT nextval('users_id_seq'::regclass),\r\n" +
                                    " \"userFirstName\" character varying(255) COLLATE pg_catalog.\"default\" NOT NULL,\r\n" +
                                    " \"userLastName\" character varying(255) COLLATE pg_catalog.\"default\" NOT NULL,\r\n" +
                                    "  \"userMail\" character varying(255) COLLATE pg_catalog.\"default\" NOT NULL,\r\n" +
                                    " \"userPassword\" character varying(255) COLLATE pg_catalog.\"default\" NOT NULL,\r\n" +
                                    " \"userPhonenumber\" character varying(20) COLLATE pg_catalog.\"default\",\r\n" +
                                    " \"userAtivo\" integer DEFAULT 1,\r\n" +
                                    "  CONSTRAINT users_pkey PRIMARY KEY (\"userId\"),\r\n" +
                                    "  CONSTRAINT users_email_key UNIQUE (\"userMail\")\r\n)\r\n\r\n" +
                                    "  TABLESPACE pg_default;\r\n\r\nALTER TABLE IF EXISTS public.users\r\n" +
                                    "  OWNER to postgres;";
                cmd.ExecuteNonQuery();
            }
            return new SqlConnection(_connectionString);
        }
    }
}
