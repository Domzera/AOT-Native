using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Endpoint;
using WebApplication1.Interface;
using WebApplication1.Repository;
using WebApplication1.Service;


var builder = WebApplication.CreateSlimBuilder(args);

// Configura o JSON serialization options
// A configuaração do JSON é feita em todos os serviços HTTP, pois o ATO não trabalha com frameworks como o ASP.NET Core MVC
// Por isso, é necessário configurar manualmente as opções de serialização JSON para os endpoints HTTP
// As configurações são feitas no arquivo AppJsonSerializerContext.cs que está na pasta Service
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Adiciona os serviços necessários para a injeção de dependência
// Cada serviço tem que ser registrado para que possa ser injetado nos endpoints HTTP
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ErrorReport>();
builder.Services.AddScoped<RegisterValidation>();
builder.Services.AddScoped<Crypter>();

// Adiciona a string de conexão ao container de serviços
//No meu caso estou usando o PostgreSQL 17.
builder.Services.AddSingleton(provider =>
    builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
    );

// Esse service adiciona o serviço de Endpoints para que possa ser injetado nos endpoints HTTP
builder.Services.AddScoped<Endpoints>();

builder.Services.AddScoped<DbContext>();

var app = builder.Build();

// Define os endpoints HTTP
// Esses endpoints estão no Program.cs mas poderiam ser definidos em arquivo serparado usando o Map

//Esta é a pagina de hello world da minha API em AOT_Native
app.MapGet("/hello", (Endpoints endpoint) =>
{
    return endpoint.hello();
});

// O /byid/{id} busca os usuarios pelo ID
app.MapGet("/user/byid/{id}", (int id,Endpoints endpoint) =>
{
    return endpoint.FindById(id);
});
// O /all busca todos os usuarios, ativos e não ativos
app.MapGet("/user/all", async (Endpoints endpoint) =>
{
    return await endpoint.FindAll();
});
// O /active busca todos os usuarios ativos
app.MapGet("/user/active", async (Endpoints endpoint) =>
{
    return await endpoint.FindAllActivate();
});
// O /register cadastra um novo usuario
app.MapPost("/user/register", async (RegisterUserDTO regUser,Endpoints endpoint) =>
{
    return await endpoint.Register(regUser);
});
// O /alter/{id} altera os dados do usuario pelo ID
app.MapPut("/user/alter/{id}", async (int id,[FromBody]UserUpdateDTO atualiza, Endpoints endpoint) =>
{
    var updated = await endpoint.Alter(id, atualiza);
    return Results.Ok(updated);
});
// O /delet/{id} altera o ATRIBUTO 'ativo' no PostmanSQL para falso, ou seja, desativa o usuario
// Não deleta os dados do usuario da base de dados
app.MapDelete("/user/delet/{id}", async (int id,Endpoints endpoint) =>
{
    return endpoint.Delete(id) ? true : false;
});

app.Run();


