using System;
using System.Collections.Generic;
using System.Linq;
using RestaurantManagementApp.Models.Interfaces;

namespace RestaurantManagementApp.Models
{
    public class Menu : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsAvailable { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<MenuProduct> MenuProducts { get; set; }

        public Menu()
        {
            MenuProducts = new HashSet<MenuProduct>();
            IsAvailable = true;
        }

        public decimal CalculatePrice()
        {
            decimal totalPrice = 0;
            if (MenuProducts != null)
            {
                foreach (var menuProduct in MenuProducts)
                {
                    if (menuProduct.Product != null)
                    {
                        totalPrice += menuProduct.Product.Price * menuProduct.Quantity;
                    }
                }
            }
            return totalPrice * 0.9m;
        }

        public void UpdateAvailability()
        {
            IsAvailable = MenuProducts != null && MenuProducts.All(mp => mp.Product.IsAvailable);
        }
    }
}
