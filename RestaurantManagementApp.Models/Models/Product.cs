using System.Collections.Generic;
using RestaurantManagementApp.Models.Interfaces;

namespace RestaurantManagementApp.Models
{
    public class Product : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal PortionQuantity { get; set; }
        public string MeasurementUnit { get; set; }
        public decimal TotalQuantity { get; set; }
        public int CategoryId { get; set; }
        public bool IsAvailable { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<ProductAllergen> ProductAllergens { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; }
        public virtual ICollection<MenuProduct> MenuProducts { get; set; }

        public bool HasSufficientStock(decimal requiredQuantity)
        {
            return TotalQuantity >= requiredQuantity;
        }

        public void UpdateAvailability()
        {
            IsAvailable = TotalQuantity > 0;
        }
    }
}
