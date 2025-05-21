using System;
using System.Linq;
using System.Security.Cryptography;


namespace RestaurantManagementApp.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }

    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
           
            return password;
        }

        public bool VerifyPassword(string password, string storedPassword)
        {
            
            return password == storedPassword;
        }
    }
}


