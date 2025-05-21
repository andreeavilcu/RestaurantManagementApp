using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantManagementApp.Models.Interfaces;

namespace RestaurantManagementApp.Models
{
    public class Category : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }

        public Category()
        {
            Products = new HashSet<Product>();
            Menus = new HashSet<Menu>();
        }
    }
}
