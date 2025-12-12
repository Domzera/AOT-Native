using Microsoft.AspNetCore.Mvc;
using Npgsql;
using WebApplication1.DTO;
using WebApplication1.Interface;
using WebApplication1.Service;

namespace WebApplication1.Repository
{
    /// <summary>
    /// Na classe UserRepository, implementamos os métodos definidos
    /// na interface IUserRepository para interagir com o banco de dados PostgreSQL.
    /// A classe utiliza a biblioteca Npgsql para executar comandos SQL
    /// A classe IUserRepository éimnplementada, no construtor, para fornecer operações CRUD para a entidade User.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly string _conectionString;
        private readonly RegisterValidation _validation;
        private readonly Crypter _crypter;

        public UserRepository(string connectionString,RegisterValidation validation,Crypter crypter)
        {
            _conectionString = connectionString;
            _validation = validation;
            _crypter = crypter;
        }
        //Registro de Usuário
        /// <summary>
        /// A classe UserRepository implementa o método Add para adicionar um novo usuário ao banco de dados.
        /// Nesta classe devemos tomar cuidado com o PASSWORD e com EMAIL , para que não haja duplicidade e não seja NULO ou VAZIO.
        /// O parametro ID é incrementado automaticamente pelo banco de dados.
        /// O parametro ATIVO é definido como 1 (ativo) por padrão.
        /// O parametro DATA é definido como a data e hora atual no momento da inserção.
        /// </summary>
        /// <param name="usuario">É o objeto que vem do front para podermos trabalar e sdalvar os dados no banco de dados.</param>
        /// <returns>Retorna o objeto USUARIO recem cadastrado ou uma mensagem de erro ao registrar.</returns>
        public Task<RegisterUserDTO> Add(RegisterUserDTO usuario)
        {

            ///<summary>
            ///Valida os atributos do usuário antes de inseri-lo no banco de dados.
            ///A classe REGISTERVALIDATION.CS é responsável por validar cada atributo do usuário.
            ///</summary>
            if (_validation.RegisterUserValidation(usuario) == true)
            {     
                ///<summary>
                /// Refazemos os atriutos do SUSUARIO para pode HASHEAR o password.
                /// Neste processo já aproveitamos para incrementar os atributos que serão inseridos
                ///     no Banco de Dados como USUARIO.ATIVO e USUARIO.DATA.
                
                var user = new RegisterUserDTO
                {
                    Name = usuario.Name,
                    LastName = usuario.LastName,
                    Email = usuario.Email,
                    Password = _crypter.Hasher(usuario.Password),
                    PhoneNumber = usuario.PhoneNumber,
                    CPF = usuario.CPF,
                    Ativo = 1,
                    data = DateTime.Now
                };
                
                try
                {
                    using var cnx = new NpgsqlConnection(_conectionString);
                    cnx.Open();
                    var cmd = cnx.CreateCommand();

                    cmd.CommandText = "INSERT INTO userTable " +
                        " (userFirstName, userLastName, userMail, userPassword, userPhonenumber, userAtivo, userDate, userCPF)" +
                        " VALUES (@userFirstName, @userLastName, @Email, @Password, @PhoneNumber, @Ativo, @Time, @CPF)";
                    cmd.Parameters.Add(new NpgsqlParameter("@userFirstName", user.Name));
                    cmd.Parameters.Add(new NpgsqlParameter("@userLastName", user.LastName));
                    cmd.Parameters.Add(new NpgsqlParameter("@Email", user.Email));
                    cmd.Parameters.Add(new NpgsqlParameter("@Password", user.Password));
                    cmd.Parameters.Add(new NpgsqlParameter("@PhoneNumber", user.PhoneNumber));
                    cmd.Parameters.Add(new NpgsqlParameter("@CPF", user.CPF));
                    cmd.Parameters.Add(new NpgsqlParameter("@Ativo", user.Ativo));
                    cmd.Parameters.Add(new NpgsqlParameter("@Time", user.data));
                    cmd.ExecuteNonQuery();
                    cnx.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao adicionar usuário: " + ex.Message);
                    var errorService = new RegisterUserDTO
                    {
                        Name = "Erro ao adicionar usuário: ",
                        LastName = ex.Message,
                        Email = "",
                        Password = "",
                        PhoneNumber = "",
                        CPF = "",
                        Ativo = 0,
                        data = DateTime.Now
                    };
                    return Task.FromResult<RegisterUserDTO>(errorService);
                };
                return Task.FromResult(usuario);
            }
            else
            {
                Console.WriteLine("Erro na validação dos dados do usuário.");
                var errorService = new RegisterUserDTO
                {
                    Name = "Erro ao adicionar usuário: ",
                    LastName = "",
                    Email = "",
                    Password = "",
                    PhoneNumber = "",
                    CPF = "",
                    Ativo = 0,
                    data = DateTime.Now
                };
                return Task.FromResult<RegisterUserDTO>(errorService);
            }

            
        }
        //Delete lógico do usuário
        /// <summary>
        /// A classe DELETE implementa o método para desativar um usuário no banco de dados.
        /// O usuário não é realmente deletado, mas sim marcado como inativo (userAtivo = 0). Isso é importante
        /// para fins fiscais e de auditoria, garantindo que os dados do usuário permaneçam no sistema por tempo determinado
        /// </summary>
        /// <param name="id">O ID é o parametro utilizado para identificar o usuário que será alterado.</param>
        /// <returns>Retorna TRUE se alterado com sucesso e Falso caso contrário.</returns>
        public bool Delete(int id)
        {
            try
            {
                using var cnx = new NpgsqlConnection(_conectionString);
                cnx.Open();
                var cmd = cnx.CreateCommand();
                cmd.CommandText = "UPDATE public.usertable SET  userAtivo = @userAtivo" +
                    " WHERE userId = @userId";
                cmd.Parameters.Add(new NpgsqlParameter("@userAtivo", value: 0));
                cmd.Parameters.Add(new NpgsqlParameter("@userId", id));
                Console.WriteLine(cmd.CommandText);
                Console.WriteLine(id);
                if (cmd.ExecuteNonQuery()>=1){
                    cnx.Close();
                    return true;
                }else
                {
                    cnx.Close();
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao atualizar usuário: " + ex.Message);
                return false;
            }
        }
        //Atualização do usuário
        /// <summary>
        /// A classe UserRepository implementa o método Update para atualizar as informações de um usuário
        /// existente no banco de dados utilizando seu ID como método de identificação.
        /// Nesta classe é realizada uma busca pelo usuário atual para que os campos que não forem informados
        /// sejam preenchidos com os dados atuais.
        /// </summary>
        /// <param name="userId">É a identificação do usuário que será atualizado.</param>
        /// <param name="usuario">É o objeto que vem do front para atualizar os dados existentes no banco de dados.</param>
        /// <returns>Retorna o objeto do usuário recêm atualizado ou a mensagem de erro.</returns>
        public async Task<UserDTO> Update(int userId,[FromBody]UserUpdateDTO usuario)
        {
            try
            {
                UserDTO userTest = GetUserById(userId).Result;

                using var cnx = new NpgsqlConnection(_conectionString);
                cnx.Open();
                var cmd = cnx.CreateCommand();

                cmd.CommandText = "UPDATE public.usertable SET userFirstName = @userFirstName, userLastName = @userLastName, userMail = @Email, userPassword = @Password, userPhonenumber = @PhoneNumber, userCPF = @CPF WHERE userId = @Id";
                cmd.Parameters.Add(new NpgsqlParameter("@userFirstName", usuario.Name == null || usuario.Name =="" ? userTest.Name : usuario.Name));
                cmd.Parameters.Add(new NpgsqlParameter("@userLastName", usuario.LastName == null || usuario.LastName == "" ? userTest.LastName : usuario.LastName));
                cmd.Parameters.Add(new NpgsqlParameter("@Email", usuario.Email == null || usuario.Email == "" ? userTest.Email : usuario.Email));
                cmd.Parameters.Add(new NpgsqlParameter("@Password", usuario.Password == null || usuario.Password == "" ? userTest.Password : usuario.Password));
                cmd.Parameters.Add(new NpgsqlParameter("@PhoneNumber", usuario.PhoneNumber == null || usuario.PhoneNumber == "" ? userTest.PhoneNumber : usuario.PhoneNumber));
                cmd.Parameters.Add(new NpgsqlParameter("@CPF", usuario.CPF == null || usuario.CPF == "" ? userTest.CPF : usuario.CPF));

                cmd.Parameters.Add(new NpgsqlParameter("@Id", userId));

                cmd.ExecuteNonQuery();
                cnx.Close();

                return await GetUserById(userId);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao atualizar usuário: " + ex.Message);
                return null;
            }
        }
        public Task<UserDTO> GetByName(string name)
        {
            throw new NotImplementedException();
        }
        //Busca todos usuários, ativos e inativos
        /// <summary>
        /// A classe UserRepository implementa o método GetAllUsersAsync para buscar todos os usuários, ativos e
        /// inativos, no banco de dados.
        /// </summary>
        /// <returns>Retorna uma lista de todos o usuários e todos seus atributos.</returns>
        public Task<List<UserDTO>> GetAllUsersAsync()
        {
            using var cnx = new NpgsqlConnection(_conectionString);
            cnx.Open();
            var cmd = cnx.CreateCommand();

            cmd.CommandText = "SELECT * From usertable ORDER BY userId";
            var reader = cmd.ExecuteReader();
            var users = new List<UserDTO>();
            while (reader.Read())
            {
                var user = new UserDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("userId")),
                    Name = reader.GetString(reader.GetOrdinal("userFirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("userLastName")),
                    Email = reader.GetString(reader.GetOrdinal("userMail")),
                    Password = reader.GetString(reader.GetOrdinal("userPassword")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("userPhoneNumber")),
                    Ativo = reader.GetInt32(reader.GetOrdinal("userAtivo")),
                    CPF = reader.GetString(reader.GetOrdinal("userCPF")),
                    data = (DateTime.Now)
                };
                users.Add(user);
            }
            cnx.Close();
            return Task.FromResult(users);
        }
        //Busca todos usuários ATIVOS
        /// <summary>
        /// A classe UserRepository implementa o método GetActiveUsersAsync para buscar todos os usuários ATIVOS.
        /// Essa classe filtra os usuários com base no campo userAtivo, retornando apenas aqueles marcados como ativos (userAtivo = 1).
        /// Esse endpoint fica disponível para o front-end listar apenas os usuários que estão ativos no sistema.
        /// </summary>
        /// <returns>´Retorna uma lista de todos os usuários ativos com todos os seus parametros.</returns>
        public Task<List<UserDTO>> GetActiveUsersAsync()
        {
            using var cnx = new NpgsqlConnection(_conectionString);
            cnx.Open();
            var cmd = cnx.CreateCommand();

            cmd.CommandText = "SELECT * From usertable ORDER BY userId";
            var reader = cmd.ExecuteReader();
            var users = new List<UserDTO>();
            while (reader.Read())
            {
                var user = new UserDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("userId")),
                    Name = reader.GetString(reader.GetOrdinal("userFirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("userLastName")),
                    Email = reader.GetString(reader.GetOrdinal("userMail")),
                    Password = reader.GetString(reader.GetOrdinal("userPassword")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("userPhoneNumber")),
                    Ativo = reader.GetInt32(reader.GetOrdinal("userAtivo")),
                    CPF = reader.GetString(reader.GetOrdinal("userCPF")),
                    data = (DateTime.Now)
                };
                if (user.Ativo == 1) users.Add(user);
            }
            cnx.Close();
            return Task.FromResult(users);
        }
        //Busca usuário por ID
        /// <summary>
        /// A classe UserRepository implementa o método GetUserById para buscar um usuário específico no banco de dados.
        /// </summary>
        /// <param name="id">ID é o parametro utlizado na busca.</param>
        /// <returns>Retorna um objeto do usuario - USERDTO -</returns>
        public Task<UserDTO> GetUserById(int id)
        {
            using var cnx = new NpgsqlConnection(_conectionString);
            cnx.Open();
            var cmd = cnx.CreateCommand();

            cmd.CommandText = "SELECT * From usertable WHERE userId = @Id";
            cmd.Parameters.Add(new NpgsqlParameter("@Id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = id });
            var reader = cmd.ExecuteReader();
            UserDTO? user = null;
            if (reader.Read())
            {
                user = new UserDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("userId")),
                    Name = reader.GetString(reader.GetOrdinal("userFirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("userLastName")),
                    Email = reader.GetString(reader.GetOrdinal("userMail")),
                    Password = reader.GetString(reader.GetOrdinal("userPassword")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("userPhoneNumber")),
                    Ativo = reader.GetInt32(reader.GetOrdinal("userAtivo")),
                    data = reader.GetDateTime(reader.GetOrdinal("userDate")),
                    CPF = reader.GetString(reader.GetOrdinal("userCPF")),
                };
            }
            cnx.Close();
            return Task.FromResult<UserDTO>(user);
        }
    }
}
