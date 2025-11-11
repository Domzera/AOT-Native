using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTO;
using WebApplication1.Interface;
using WebApplication1.Service;

namespace WebApplication1.Endpoint
{
    public static class UserEndpoint
    {
        public static void Map(WebApplication app)
        {
            ///<summary>
            /// Esse pedaço de código serve para testar se a API está rodando.
            /// Se tem retorno "pong" é porque está tudo ok.
            /// --==* TESTE DE URL *==--
            ///   app.MapGet("/ping", () => "pong");
            /// Esse código é muito útil para testes iniciais de deploy e verificação rápida do status da API.
            ///</summary >

            //GETALL USERS
            /// <summary>
            /// O endpoint de obtenção de todos os usuários permite recuperar uma lista completa de todos os
            /// usuários cadastrados no sistema.
            /// Será mais util para a administração do sistema.
            /// NÃO fica disponivel para usuários do FRONTEND.
            /// Caso ocorra algum erro durante a operação, o erro será registrado utilizando a classe ErrorReport.
            ///</summary>
            app.MapGet("/user/all", async (
                IUserRepository userRepository,
                ErrorReport errorR) =>
            {
                Console.WriteLine("GetAll endpoint hit");
                try
                {
                    var users = await userRepository.GetAllUsersAsync();
                    return Results.Ok(users);
                }
                catch (Exception ex)
                {
                    var operationType = "Get All Users";
                    errorR.AddErrorAll(ex.Message, operationType);
                    return Results.Problem(detail: ex.Message, statusCode: 500);
                }
            });

            //GETBYID USER
            /// <summary>
            /// O endpoint de obtenção de usuário por ID permite buscar um usuário específico com base em seu identificador único.
            /// Fica disponivel para usuários do FRONTEND.
            /// caso ocorra algum erro durante a operação, o erro será registrado utilizando a classe ErrorReport.
            /// </summary>
            app.MapGet("/user/getbyid/{id}", async (
                int id,
                IUserRepository userRepository,
                ErrorReport errorR) =>
            {
                Console.WriteLine("GetById endpoint hit");
                try
                {
                    var user = await userRepository.GetUserById(id);
                    //return user is not null ? Results.Ok(user) : Results.NotFound();
                    return Results.Ok(user);
                }
                catch (Exception ex)
                {
                    var operationType = "Get User By Id";
                    // Assuming you have an instance of ErrorReport available here
                    errorR.AddErrorId(ex.Message, id, operationType);
                    return Results.Problem(detail: ex.Message, statusCode: 500);
                }
            });

            //CREATE USER
            /// <summary>
            /// O endpoint de registro de usuário permite que novos usuários sejam adicionados ao sistema.
            /// Fica disponivel para usuários do FRONTEND.
            /// Caso ocorra algum erro durante a operação, o erro será registrado utilizando a classe ErrorReport.
            /// </summary>
            app.MapPost("/user/register", async (
                RegisterUserDTO user,
                [FromServices]IUserRepository userRepository,
                [FromServices]ErrorReport errorR) =>
            {
                Console.WriteLine("Register endpoint hit");
                try
                {
                    var createdUser = await userRepository.Add(user);
                    return Results.Created($"/users/{createdUser.ToString()}", createdUser);
                }
                catch (Exception ex)
                {
                    var operationType = "Register User";
                    // Assuming you have an instance of ErrorReport available here
                    errorR.AddErrorRegister(ex.Message, operationType);
                    return Results.Problem(detail: ex.Message, statusCode: 500);
                }
            });

            //TESTE DE URL
            /// <summary>
            /// O endpoint de ping serve para testar se a API está rodando.
            /// NÃO fica disponivel para usuários do FRONTEND.
            /// Foi util para testes iniciais de deploy e verificação rápida do status da API.
            /// --==** NÃO ficara disponivel em produção.**==--
            /// </summary>
            app.MapGet("/ping", () =>
            {
                Console.WriteLine("Ping recebido");
                return Results.Ok("pong");
            });
        }
    }
}
