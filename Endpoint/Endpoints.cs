using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTO;
using WebApplication1.Interface;
using WebApplication1.Service;

namespace WebApplication1.Endpoint
{
    /// <summary>
    /// No arquivo Endpoints.cs, a classe Endpoints define vários métodos assíncronos para gerenciar usuários,
    /// O endpoints são chamados pelo arquivo Program.cs que configura as rotas da API.
    /// </summary>
    public class Endpoints
    {
        /// <summary>
        /// As injeções de outras classes nescessárias para o funcionamento dos endpoints são feitas via construtor.
        /// As classes usadas são IUserRepository para operações de banco de dados relacionadas a usuários e
        /// ErrorReport para registrar erros que possam ocorrer durante essas operações.
        /// </summary>
        private readonly IUserRepository _userRep;
        private readonly ErrorReport _error;

        public Endpoints(IUserRepository userRep, ErrorReport error)
        {
            _userRep = userRep;
            _error = error;

        }
        //REGISTRO DE NOVO USUÁRIO

        /// <summary>
        /// Registra um novo usuário no sistema.
        /// <remarks>Valida cada atributo do usuário antes de criar o registro, pois o AOT não trabalha com frameworks de validação.
        /// Caso ocorra algum erro, ele é registrado em um banco de dados de erros que estão no mesmo banco de dados dos usuarios,
        /// porém em uma tabela separada, afim de facilitar a busca e análise dos erros.
        /// É um metodo asincrono, que espera ser terminado para dar o retorno</remarks>
        /// </summary>
        /// <param name="user">É o REGISTERUSERDTO que representa os dados de registro do usuário</param>
        /// <returns><see cref="RegisterUserDTO"/> retona o RegisterUserDTO que não contém ID, pois o mesmo é gerado automatticamente pelo PostgreSQL</returns>
        public async Task<RegisterUserDTO> Register(RegisterUserDTO user)
        {
            ///<summary>
            /// Realiza a validação dos dados do usuário,na classe REGISTERVALIDATION.CS, antes de registrá-lo.
            /// Na REGISTERVALIDATION.CS, cada atributo do usuário é verificado para garantir que atende aos critérios necessários.
            /// O sistema AOT não trabalha com frameworks de nenhum tipo, então a validação é feita manualmente para cada atributo.
            /// </summary>
            try
            {
                var createdUser = await _userRep.Add(user);
                return createdUser;
            }
            ///<summary>
            ///Caso haja algum erro durante o processo de registro, ele é capturado e registrado usando a classe ErrorReport.
            ///Essa calsse tem um BD independente para armazenar os erros, facilitando a análise e solução de problemas.
            ///</summary>
            catch (Exception ex)
            {
                var operationType = "Register User - ENDPOINT";
                _error.AddErrorRegister(ex.Message, operationType);
            }
            return null;
        }

        //BUSCAR USUÁRIO POR ID

        /// <summary>
        /// Busca de usuário pelo ID.
        /// </summary>
        /// <remarks>Este metodo busca usuário pelo ID fornecido.´Se o usuário for encontrado, retorna um objeto UserDTO,
        /// caso contrário, retorna um resultado de bad request.
        /// É um metodo asincrono, que espera ser terminado para dar o retorno</remarks>
        /// <param name="id">O identificador unico do usuário. Tem que ser um integer positivo.</param>
        /// <returns>Um <see cref="UserDTO"/> representa identificador especifico do usuário. Retorna um resultado de
        /// bad request se o usuário não for encontrado ou ocorrer um erro.
        /// O erro e o tipo de função fica salvo na tabela de erros junto ao banco de dados do usuário só que em tabelas separadas.</returns>
        public async Task<UserDTO> FindById(int id)
        {
            try
            {
                 return (UserDTO)await _userRep.GetUserById(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _error.AddErrorRegister(ex.Message, "Find User By Id");
                return (UserDTO)Results.BadRequest("Falha na busca do usuário");
            }
        }

        //BUSCA TODOS USUÁRIOS

        /// <summary>
        /// É um metodo asincrono que espera ternar a requisção para retornar a lista de todos os usuários, ativos e inativos.
        /// Esse Endpoint é o que NÃO fica a disposição para o front-end consumir. Será usado mais pela administração do sistema.
        /// </summary>
        /// <returns>Uma task asincrona que representa a operação de busca de todos os usuários e retorna
        /// uma lista de objetos <see cref="UserDTO"/>.
        /// Retorna <see langword="null"/> caso ocorra um erro durante a recuperação da lista de usuários.</returns>
        /// Os erros são registrados na tabela de erros do banco de dados.
        public async Task<List<UserDTO>> FindAll()
        {
            try
            {
                List<UserDTO> result = await _userRep.GetAllUsersAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _error.AddErrorRegister(ex.Message, "Find All Users");
                return null;
            }
        }

        //BUSCA TODOS USUÁRIOS ATIVOS

        /// <summary>
        /// Retorna uma lista de todos os usuários ativos.
        /// Esse Endpoint é o que fica a disposição para o front-end consumir.
        /// </summary>
        /// <returns> Uma Task asincrona que representa a operação de busca de todos os usuários ativos.
        /// Seu retorno é uma lista de objetos <see cref="UserDTO"/>, que inclui o ID.
        /// Retorna <see langword="null"/> caso ocorra um erro durante a recuperação da lista de usuários ativos.</returns>
        public async Task<List<UserDTO>> FindAllActivate()
        {
            try
            {
                List<UserDTO> result = await _userRep.GetActiveUsersAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _error.AddErrorRegister(ex.Message, "Find All Users");
                return null;
            }
        }

        //ALTERAR USUÁRIO

        /// <summary>
        /// Atualiza o usuário com o id especificado.
        /// </summary>
        /// <param name="id"> O Identificador unico do usuário a ser atualizado.</param>
        /// <returns>Um <see cref="UserDTO"/> representa o usuário atualizado
        /// Retornando <see langword="null"/> se a atualização do usuário apresentar algum erro </returns>
        /// Casso ocorra algum erro, ele é registrado na tabela de erros do banco de dados.
        public async Task<UserDTO> Alter(int id, [FromBody]UserUpdateDTO atualiza)
        {
            try
            {
                var updatedUser = await _userRep.Update(id,atualiza);
                return updatedUser;
            }
            catch (Exception ex)
            {
                var operationType = "Update User - ENDPOINT";
                _error.AddErrorRegister(ex.Message, operationType);
            }
            return null;
        }

        //DELETA USUÁRIO => Transforma o "ATIVO" em "INATIVO" trocando o 1 po 0

        /// <summary>
        /// Altera o atributo ATIVO, do Banco de dados, de um usuário para inativo.
        /// Troca o valor 1 por 0 no campo ATIVO.
        /// </summary>
        /// <param name="id">É o identificador unico do usuário que será desativado. O ID tem que ser de um usuário valido.</param>
        /// <returns><see langword="true"/> se o usuário for desativado com sucesso; caso contrário, <see langword="false"/>.</returns>
        /// Caso ocorra algum erro, ele é registrado na tabela de erros do banco de dados.
        public bool Delete(int id)
        {
            try
            {
                bool result = _userRep.Delete(id);
                return result;
            }
            catch (Exception ex)
            {
                var operationType = "Delete User - ENDPOINT";
                _error.AddErrorRegister(ex.Message, operationType);
                return false;
            }
        }
    }
}
