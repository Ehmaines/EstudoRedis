using EstudoRedisAPI;
using EstudoRedisAPI.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var redisConn = builder.Configuration.GetValue<string>("RedisConnection", "localhost:6379");
ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(redisConn);

builder.Services.AddSingleton<IConnectionMultiplexer>(connection);
builder.Services.AddScoped(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
builder.Services.AddScoped<UserRepository>();

var app = builder.Build();

app.MapGet("/", () => "API de Usuarios usando Redis");

app.MapGet("/api/users", async (UserRepository userRepository) =>
{
    var users = await userRepository.GetAllUsers();
    return Results.Ok(users);
});

app.MapGet("/api/users/{id:int}", async (int id, UserRepository userRepository) =>
{
    var user = await userRepository.GetUser(id);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapPost("/api/users", async (User user, UserRepository userRepository) =>
{
    await userRepository.AddUserIdToIndex(user.Id);
    await userRepository.CreateUser(user);

    return Results.Created($"/api/users/{user.Id}", user);
});

app.Run();
