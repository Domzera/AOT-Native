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

            //TESTE DE URL
            //app.MapGet("/ping", () => "pong");

            //Retirado do REGISTER
            //var createdUser = await userRepository.CreateUserAsync(user);
            //return Results.Created($"/users/{createdUser.Id}", createdUser);

            //GETALL USERS
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
            app.MapGet("/ping", () =>
            {
                Console.WriteLine("Ping recebido");
                return Results.Ok("pong");
            });
            //app.MapPost("/user/register", (HttpContext context) =>
            //{
            //    Console.WriteLine("Register endpoint hit");
            //    return Results.Ok("User registered");
            //});

            /*
            app.MapPut("/users/{id}", async (int id, User user, IUserRepository userRepository) =>
            {
                var updatedUser = await userRepository.UpdateUserAsync(id, user);
                return updatedUser is not null ? Results.Ok(updatedUser) : Results.NotFound();
            });
            app.MapDelete("/users/{id}", async (int id, IUserRepository userRepository) =>
            {
                var deleted = await userRepository.DeleteUserAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            });*/
            //return "User endpoints mapped.";
        }
    }
}
