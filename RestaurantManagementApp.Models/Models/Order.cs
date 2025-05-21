using System;
using System.Collections.Generic;
using RestaurantManagementApp.Models.Interfaces;

namespace RestaurantManagementApp.Models
{

    public enum OrderStatus
    {
        Registered = 0,
        Preparing = 1,
        Shipped = 2,
        Delivered = 3,
        Canceled = 4
    }
    public class Order : EntityBase
    {
        public string OrderCode { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal FoodCost { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public Order()
        { 
            OrderItems = new HashSet<OrderItem>();
            OrderDate = DateTime.Now;
            Status =  OrderStatus.Registered;
            OrderCode = GenerateOrderCode();
        }

        private string GenerateOrderCode()
        {
            return $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        public bool CanCancel()
        { 
            return Status == OrderStatus.Registered || Status == OrderStatus.Preparing;
        }

        public bool IsActive() 
        {
            return Status != OrderStatus.Delivered && Status != OrderStatus.Canceled;
        }
    }
}
