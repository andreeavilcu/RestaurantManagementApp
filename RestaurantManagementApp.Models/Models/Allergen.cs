using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantManagementApp.Models.Interfaces;

namespace RestaurantManagementApp.Models
{
    public class Allergen : EntityBase
    {
       public string Name { get; set; }
       public string Description { get; set; }

        public virtual ICollection<ProductAllergen> ProductAllergens { get; set; }

        public Allergen()
        {
            ProductAllergens = new HashSet<ProductAllergen>();
        }

    }
    
}
