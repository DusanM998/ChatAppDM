using ChatAppWithDb.DbContexts;
using ChatAppWithDb.Models;

namespace ChatAppWithDb.Services
{
    public class ChatService
    {
        private static readonly Dictionary<string, string> Users = new Dictionary<string, string>();
        private readonly ApplicationDbContexts _db;

        public ChatService(ApplicationDbContexts db)
        {
            _db = db;
        }

        public bool AddUserToList(string userToAdd)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Name.ToLower() == userToAdd.ToLower());

            if (existingUser != null)
            {
                return false;
            }

            _db.Users.Add(new User { Name = userToAdd });
            _db.SaveChanges();
            return true;
        }

        public void AddUserConnectionId(string user, int connectionId)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Name.ToLower() == user.ToLower());

            if (existingUser != null)
            {
                existingUser.Id = connectionId;
                _db.SaveChanges();
            }
        }

        public string GetUserByConnectionId(int connectionId)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == connectionId);
            return user?.Name;
        }

        public string GetConnectionIdByUser(string user)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Name == user);
            return existingUser?.Name;
        }

        public void RemoveUserFromList(string user)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Name == user);

            if (existingUser != null)
            {
                _db.Users.Remove(existingUser);
                _db.SaveChanges();
            }
        }

        public string[] GetOnlineUsers()
        {
            var onlineUsers = _db.Users
                .Where(u => u.Id > 0)
                .OrderBy(u => u.Name)
                .Select(u => u.Name)
                .ToArray();

            return onlineUsers;
        }
    }
}
