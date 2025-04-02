using EstudoRedisAPI.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace EstudoRedisAPI
{
    public class UserRepository
    {
        private readonly IDatabase _db;
        private const string USER_KEY_PREFIX = "user:";

        public UserRepository(IDatabase db)
        {
            _db = db;
        }

        public async Task CreateUser(User user)
        {
            var json = JsonSerializer.Serialize(user);
            await _db.StringSetAsync($"{USER_KEY_PREFIX}{user.Id}", json);
        }

        public async Task<User?> GetUser(int id)
        {
            var data = await _db.StringGetAsync($"{USER_KEY_PREFIX}{id}");
            if (data.IsNullOrEmpty) return null;

            return JsonSerializer.Deserialize<User>(data);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var indexKey = "user:index";
            var ids = await _db.SetMembersAsync(indexKey);

            var users = new List<User>();
            foreach (var redisValue in ids)
            {
                var idString = redisValue.ToString();
                if (int.TryParse(idString, out int id))
                {
                    var user = await GetUser(id);
                    if (user != null) users.Add(user);
                }
            }
            return users;
        }

        public async Task AddUserIdToIndex(int id)
        {
            var indexKey = "user:index";
            await _db.SetAddAsync(indexKey, id);
        }
    }
}
