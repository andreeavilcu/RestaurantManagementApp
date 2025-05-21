using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantManagementApp.Models.Interfaces;

namespace RestaurantManagementApp.Models
{
    public class ProductImage : EntityBase
    {
        public int ProductId { get; set; }
        public string ImagePath { get; set; }
        public string Caption { get; set; }
        public bool IsMainImage { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Product Product { get; set; }

        public ProductImage()
        {
            IsMainImage = false;
            DisplayOrder = 0;
        }
    }
}
