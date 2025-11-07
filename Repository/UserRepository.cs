using Microsoft.AspNetCore.Mvc;
using Npgsql;
using WebApplication1.DTO;
using WebApplication1.Interface;

namespace WebApplication1.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _conectionString;

        public UserRepository(string connectionString)
        {
            _conectionString = connectionString;
        }
        public Task<RegisterUserDTO> Add(RegisterUserDTO usuario)
        {
            try
            {
                using var cnx = new NpgsqlConnection(_conectionString);
                cnx.Open();
                var cmd = cnx.CreateCommand();

                cmd.CommandText = "INSERT INTO userTable " +
                    " (userFirstName, userLastName, userMail, userPassword, userPhonenumber, userAtivo, userDate, userCPF)" +
                    " VALUES (@userFirstName, @userLastName, @Email, @Password, @PhoneNumber, @Ativo, @Time, @CPF)";
                cmd.Parameters.Add(new NpgsqlParameter("@userFirstName", usuario.Name));
                cmd.Parameters.Add(new NpgsqlParameter("@userLastName", usuario.LastName));
                cmd.Parameters.Add(new NpgsqlParameter("@Email", usuario.Email));
                cmd.Parameters.Add(new NpgsqlParameter("@Password", usuario.Password));
                cmd.Parameters.Add(new NpgsqlParameter("@PhoneNumber", usuario.PhoneNumber));
                cmd.Parameters.Add(new NpgsqlParameter("@CPF", usuario.CPF));
                cmd.Parameters.Add(new NpgsqlParameter("@Ativo", value: 1));
                cmd.Parameters.Add(new NpgsqlParameter("@Time", DateTime.Now));
                cmd.ExecuteNonQuery();
                cnx.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao adicionar usuário: " + ex.Message);
                return Task.FromResult<RegisterUserDTO>(null);
            }
            ;
            return Task.FromResult(usuario);
        }
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
                cmd.ExecuteNonQuery();
                cnx.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao atualizar usuário: " + ex.Message);
                return false;
            }
        }
        public async Task<UserDTO> Update(int userId,[FromBody]RegisterUserDTO usuario)
        {
            //UserDTO usuario = await GetUserById(id).ConfigureAwait(false);
            //UserDTO usuario = [FromBody] GetUserById(userId).Result;
            try
            {
                using var cnx = new NpgsqlConnection(_conectionString);
                cnx.Open();
                var cmd = cnx.CreateCommand();
                cmd.CommandText = "UPDATE public.usertable SET userFirstName = @userFirstName, userLastName = @userLastName, userMail = @Email, userPassword = @Password, userPhonenumber = @PhoneNumber, userAtivo = @Ativo, userCPF = @CPF" +
                    " WHERE userId = @Id";
                cmd.Parameters.Add(new NpgsqlParameter("@userFirstName", usuario.Name));
                cmd.Parameters.Add(new NpgsqlParameter("@userLastName", usuario.LastName));
                cmd.Parameters.Add(new NpgsqlParameter("@Email", usuario.Email));
                cmd.Parameters.Add(new NpgsqlParameter("@Password", usuario.Password));
                cmd.Parameters.Add(new NpgsqlParameter("@PhoneNumber", usuario.PhoneNumber));
                cmd.Parameters.Add(new NpgsqlParameter("@Ativo", usuario.Ativo));
                cmd.Parameters.Add(new NpgsqlParameter("@Id", userId));
                cmd.Parameters.Add(new NpgsqlParameter("@CPF", usuario.CPF));
                cmd.ExecuteNonQuery();
                cnx.Close();
                //return Task.FromResult(usuario);
                //return usuario;
                return new UserDTO
                {
                    Id = userId,
                    Name = usuario.Name,
                    LastName = usuario.LastName,
                    Email = usuario.Email,
                    Password = usuario.Password,
                    PhoneNumber = usuario.PhoneNumber,
                    Ativo = usuario.Ativo,
                    CPF = usuario.CPF,
                    data = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao atualizar usuário: " + ex.Message);
                //return Task.FromResult<UserDTO>(null);
                return null;
            }
        }
        public Task<UserDTO> GetByName(string name)
        {
            throw new NotImplementedException();
        }
        //Busca todos ativos e inativos
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
        //Busca todos ATIVOS
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
