using RestaurantManagementApp.Models.Interfaces;

namespace RestaurantManagementApp.Models
{
    public class OrderItem : EntityBase
    {
        public int OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? MenuId { get; set; }
        public  int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }



        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public virtual Menu Menu { get; set; }

        public string ItemName => Product != null ? Product.Name : Menu?.Name; 
    }
}
