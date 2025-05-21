

namespace RestaurantManagementApp.Models
{
    public class MenuProduct 
    {
        public int MenuId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; } 

        public virtual Menu Menu { get; set; }
        public virtual Product Product { get; set; }
    }
}
