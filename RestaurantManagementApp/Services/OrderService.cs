using RestaurantManagementApp.DataAccess;
using RestaurantManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace RestaurantManagementApp.Services
{
    public interface IOrderService
    {
        IEnumerable<Order> GetAllOrders();
        IEnumerable<Order> GetOrdersByUser(int userId);
        IEnumerable<Order> GetActiveOrders();
        IEnumerable<Order> GetOrdersByStatus(OrderStatus status);

        Order CreateOrder(int userId, List<OrderItem> items);
        bool UpdateOrderStatus(int orderId, OrderStatus newStatus);
        bool CancelOrder(int orderId);

        Order GetOrderById(int id);

        decimal CalculateOrderTotal(List<OrderItem> items);
        decimal CalculateDiscount(decimal subtotal, int userId);
        decimal CalculateDeliveryCost(decimal subtotal);
    }

    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryService _inventoryService;
        private readonly IConfigurationService _configService;

        public OrderService(IUnitOfWork unitOfWork, IInventoryService inventoryService, IConfigurationService configService)
        {
            _unitOfWork = unitOfWork;
            _inventoryService = inventoryService;
            _configService = configService;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _unitOfWork.Orders.GetOrdersWithDetails();
        }

        public IEnumerable<Order> GetOrdersByUser(int userId)
        {
            return _unitOfWork.Orders.GetOrdersByUser(userId);
        }

        public IEnumerable<Order> GetActiveOrders()
        {
            return _unitOfWork.Orders.GetActiveOrders();
        }

        public IEnumerable<Order> GetOrdersByStatus(OrderStatus status)
        {
            return _unitOfWork.Orders.GetOrdersByStatus(status);
        }

        public Order CreateOrder(int userId, List<OrderItem> items)
        {
            foreach (var item in items)
            {
                if (item.ProductId.HasValue)
                {
                    var product = _unitOfWork.Products.GetById(item.ProductId.Value);
                    if (!product.HasSufficientStock(item.Quantity * product.PortionQuantity))
                    {
                        throw new InvalidOperationException($"Insufficient stock for product: {product.Name}");
                    }
                }
            }

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    decimal foodCost = CalculateOrderTotal(items);
                    decimal deliveryCost = CalculateDeliveryCost(foodCost);
                    decimal discountAmount = CalculateDiscount(foodCost, userId);
                    decimal totalCost = foodCost + deliveryCost - discountAmount;
                    var estimatedDeliveryTime = DateTime.Now.AddDays(1);
                    var order = new Order
                    {
                        UserId = userId,
                        FoodCost = foodCost,
                        DeliveryCost = deliveryCost,
                        DiscountAmount = discountAmount,
                        TotalCost = totalCost,
                        EstimatedDeliveryTime = estimatedDeliveryTime,
                    };

                    _unitOfWork.Orders.Add(order);
                    _unitOfWork.Complete();

                    foreach (var item in items)
                    {
                        item.OrderId = order.Id;
                        if (item.ProductId.HasValue)
                        {
                            var product = _unitOfWork.Products.GetById(item.ProductId.Value);
                            item.UnitPrice = product.Price;
                            item.TotalPrice = item.UnitPrice * item.Quantity;
                            _unitOfWork.OrderItems.Add(item);
                        }
                        else if (item.MenuId.HasValue)
                        {
                            var menu = _unitOfWork.Menus.GetById(item.MenuId.Value);
                            item.UnitPrice = menu.CalculatePrice();
                            item.TotalPrice = item.UnitPrice * item.Quantity;
                            _unitOfWork.OrderItems.Add(item);
                        }
                    }

                    _unitOfWork.Complete();

                    _inventoryService.UpdateStockForOrder(order);

                    transaction.Commit();
                    return order;
                }
                catch (Exception ex)
                { 
                    transaction.Rollback();
                    throw new Exception($"Error creating order: {ex.Message}", ex);
                }
            }
          
        }

       

        public bool UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = _unitOfWork.Orders.GetById(orderId);
            if (order == null)
                return false;

            order.Status = newStatus;
            _unitOfWork.Complete();
            return true;
        }

        public bool CancelOrder(int orderId)
        {
            var order = _unitOfWork.Orders.GetById(orderId);
            if (order == null || !order.CanCancel())
                return false;

            order.Status = OrderStatus.Canceled;
            _unitOfWork.Complete();
            return true;
        }

       

        public Order GetOrderById(int id)
        {
            
            return _unitOfWork.Orders.GetOrderWithDetails(id);
        }

        public decimal CalculateOrderTotal(List<OrderItem> items)
        {
            decimal total = 0;
            foreach (var item in items)
            {
                if (item.ProductId.HasValue)
                {
                    var product = _unitOfWork.Products.GetById(item.ProductId.Value);
                    total += product.Price * item.Quantity;
                }
                else if (item.MenuId.HasValue)
                {
                    var menu = _unitOfWork.Menus.GetById(item.MenuId.Value);
                    total += menu.CalculatePrice() * item.Quantity;
                }
            }
            return total;
        }

        public decimal CalculateDiscount(decimal subtotal, int userId)
        {
            decimal discountThreshold = _configService.GetOrderDiscountThreshold();
            decimal discountPercentage = _configService.GetOrderDiscountPercentage();

            if (subtotal >= discountThreshold)
            {
                return subtotal * (discountPercentage / 100);
            }

            // Loyalty discount
            int loyaltyOrderCount = _configService.GetLoyaltyOrderCount();
            int loyaltyTimePeriod = _configService.GetLoyaltyTimePeriod();
            decimal loyaltyDiscountPercentage = _configService.GetLoyaltyDiscountPercentage();
            var loyaliyTimePeriodAddDAys = DateTime.Now.AddDays(-loyaltyTimePeriod);
            var recentOrders = _unitOfWork.Orders.Find(o =>
                o.UserId == userId &&
                o.OrderDate >= loyaliyTimePeriodAddDAys &&
                o.Status != OrderStatus.Canceled);

            if (recentOrders.Count() >= loyaltyOrderCount)
            {
                return subtotal * (loyaltyDiscountPercentage / 100);
            }

            return 0;
        }

        public decimal CalculateDeliveryCost(decimal subtotal)
        {
            decimal freeDeliveryThreshold = _configService.GetFreeDeliveryThreshold();
            decimal deliveryCost = _configService.GetDeliveryCost();

            return subtotal >= freeDeliveryThreshold ? 0 : deliveryCost;
        }
    }
}
