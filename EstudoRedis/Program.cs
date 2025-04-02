using StackExchange.Redis;

var redisConnectionString = "localhost:6379";

using var conn = ConnectionMultiplexer.Connect(redisConnectionString);
Console.WriteLine("Connected to Redis");

IDatabase db = conn.GetDatabase();

db.StringSet("chave:teste", "Olá Redis!");

var valor = db.StringGet("chave:teste");
Console.WriteLine($"Valor obtido: {valor}");

db.StringSet("contador", 0);
for (int i = 0; i < 10; i++)
{
    var contadorAtual = db.StringGet("contador");
    Console.WriteLine($"Contador atual: {contadorAtual}");
    db.StringIncrement("contador");
}

conn.Close();
Console.WriteLine("Connection closed");