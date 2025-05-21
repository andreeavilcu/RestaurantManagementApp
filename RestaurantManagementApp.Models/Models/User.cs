using System;
using System.Collections.Generic;
using RestaurantManagementApp.Models.Interfaces;

namespace RestaurantManagementApp.Models
{
    public enum UserRole
    { 
        Customer = 0,
        Employee = 1
    }
    public class User : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string DeliveryAddress { get; set; }
        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Order> Orders { get; set; }

        public User()
        {
            Orders = new HashSet<Order>();
            Role = UserRole.Customer;
            CreatedAt = DateTime.Now;
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}
