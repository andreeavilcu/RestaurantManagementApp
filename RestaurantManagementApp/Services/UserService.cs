using RestaurantManagementApp.DataAccess;
using RestaurantManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagementApp.Services
{
    public interface IUserService
    {
        User Authenticate(string email, string password);
        User GetUserById(int id);
        User GetUserByEmail(string email);
        IEnumerable<User> GetAllUsers();
        IEnumerable<User> GetUsersByRole(UserRole role);

        
        bool UpdateUser(User user);
        bool DeleteUser(int id);

        User GetCurrentUser();
        void SetCurrentUser(User user);
        bool IsAuthenticated();
        bool IsInRole(UserRole role);
        void Logout();
        bool ChangePassword(string email, string currentPassword, string newPassword);

        bool RegisterUser(User user, string password);

        void UpdateAllUserPasswords();
        void ResetPasswordForInterfaceUsers();
        IUnitOfWork GetUnitOfWork();
    }


    public class UserService : IUserService
    {
        private static Dictionary<string, string> _devCredentials = new Dictionary<string, string>();
        private readonly IUnitOfWork _unitOfWork;
        private User _currentUser;
        private readonly IPasswordService _passwordService;


        
        public void UpdateAllUserPasswords()
        {
            var allUsers = _unitOfWork.Users.GetAll();
            foreach (var user in allUsers)
            {
                
                if (!user.PasswordHash.StartsWith("$2") && !user.PasswordHash.Contains("."))
                {
                    string originalPassword = user.PasswordHash;
                    user.PasswordHash = _passwordService.HashPassword(originalPassword); 
                    _unitOfWork.Users.Update(user);
                }
            }
            _unitOfWork.Complete();
            System.Diagnostics.Debug.WriteLine("Toate parolele au fost actualizate la hash-uri BCrypt");
        }

        public void ResetPasswordForInterfaceUsers()
        {
            var users = _unitOfWork.Users.GetAll()
                .Where(u => (u.Email.Contains("@gmail") || u.Email.Contains("@yahoo") || u.Email.Contains("@hotmail"))
                    && !u.PasswordHash.StartsWith("$2"));

            foreach (var user in users)
            {
                string defaultPassword = user.Email;
                user.PasswordHash = _passwordService.HashPassword(defaultPassword);
                _unitOfWork.Users.Update(user);
            }
            _unitOfWork.Complete();
        }
        public UserService(IUnitOfWork unitOfWork, IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService; 
        }

        public IUnitOfWork GetUnitOfWork()
        {
            return _unitOfWork;
        }
        public User Authenticate(string email, string password)
        {
            var user = _unitOfWork.Users.GetUserByEmail(email);
            if (user == null)
                return null;

           
            if (password == user.PasswordHash)
            {
                _currentUser = user;
                return user;
            }

            return null;
        }

        public User GetUserById(int id)
        {
            return _unitOfWork.Users.GetById(id);
        }

        public User GetUserByEmail(string email)
        {
            return _unitOfWork.Users.GetUserByEmail(email);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _unitOfWork.Users.GetAll();
        }

        public IEnumerable<User> GetUsersByRole(UserRole role)
        {
            return _unitOfWork.Users.GetUsersByRole(role);
        }

        public bool UpdateUser(User user)
        {
            var existingUser = _unitOfWork.Users.GetById(user.Id);
            if (existingUser == null)
                return false;

            _unitOfWork.Users.Update(user);
            _unitOfWork.Complete();

            if (_currentUser != null && _currentUser.Id == user.Id)
            {
                _currentUser = user;
            }
            return true;
        }

        public bool DeleteUser(int id)
        {
            var user = _unitOfWork.Users.GetById(id);
            if (user == null)
                return false;

            _unitOfWork.Users.Remove(user);
            _unitOfWork.Complete();

            if (_currentUser != null && _currentUser.Id == id)
            {
                _currentUser = null;
            }
            return true;
        }


        public User GetCurrentUser()
        {
            return _currentUser;
        }

        public void SetCurrentUser(User user)
        {
            _currentUser = user;
        }

        public bool IsAuthenticated()
        {
            return _currentUser != null;
        }

        public bool IsInRole(UserRole role)
        {
            return _currentUser != null && _currentUser.Role == role;
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public bool ChangePassword(string email, string currentPassword, string newPassword)
        {
            var user = GetUserByEmail(email);


            if (user == null || !_passwordService.VerifyPassword(currentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = _passwordService.HashPassword(newPassword);
            _unitOfWork.Users.Update(user);
            _unitOfWork.Complete();

            return true;
        }

        public bool RegisterUser(User user, string password)
        {
            
            user.PasswordHash = password;
            _unitOfWork.Users.Add(user);
            return _unitOfWork.Complete() > 0;
        }

    }
}
