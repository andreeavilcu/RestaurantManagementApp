using System;
using System.Collections.Generic;

namespace RestaurantManagementApp.Models
{
    public class ProductAllergen
    {
        public int ProductId { get; set; }
        public int AllergenId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Allergen Allergen { get; set; }
    }
}
