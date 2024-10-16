using System.Collections.Generic;
using System.Linq;
using LangLang.Models;

namespace LangLang.Repositories.PostgresRepositories
{
    public class UserPostgresRepository:IUserRepository
    {
        private readonly DatabaseContext _databaseContext;

        public UserPostgresRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<User> GetAll()
        {
            return _databaseContext.Users.ToList();
        }

        public User? GetById(int id)
        {
            return _databaseContext.Users.Find(id);
        }

        public void Add(User user)
        {
            var addedUser = _databaseContext.Users.Add(user);
            _databaseContext.SaveChanges();
        }

        public void Update(User user)
        {
            _databaseContext.Users.Update(user);
            _databaseContext.SaveChanges();
        }

        public void Delete(int id)
        {
            User? user = _databaseContext.Users.Find(id);
            if (user == null) 
                return;
            _databaseContext.Users.Remove(user);
            _databaseContext.SaveChanges();
        }
    }
}
