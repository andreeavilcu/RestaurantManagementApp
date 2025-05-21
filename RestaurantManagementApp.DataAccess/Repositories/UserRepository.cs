using RestaurantManagementApp.DataAccess.Repositories.Implementations;
using RestaurantManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagementApp.DataAccess.Repositories
{

    public interface IUserRepository : IRepository<Models.User>
    {
        Models.User GetUserByEmail(string email);
        IEnumerable<Models.User> GetUsersByRole(Models.UserRole role);
    }
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DatabaseContext context) : base(context)
        {
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users
                .FirstOrDefault(u => u.Email == email);
        }

        public IEnumerable<User> GetUsersByRole(UserRole role)
        {
            return _context.Users
                .Where(u => u.Role == role)
                .ToList();
        }
    }
}
