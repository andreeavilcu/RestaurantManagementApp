using RestaurantManagementApp.DataAccess.Repositories.Implementations;
using RestaurantManagementApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace RestaurantManagementApp.DataAccess.Repositories
{

    public interface IOrderRepository : IRepository<Models.Order>
    {
        IEnumerable<Models.Order> GetOrdersWithDetails();
        IEnumerable<Models.Order> GetOrdersByUser(int userId);
        IEnumerable<Models.Order> GetActiveOrders();
        IEnumerable<Models.Order> GetOrdersByStatus(Models.OrderStatus status);

        Models.Order GetOrderWithDetails(int id);
    }
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(DatabaseContext context) : base(context)
        {
        }

        public IEnumerable<Order> GetOrdersWithDetails()
        {
            return _context.Orders
                .Include(o => o.User)           
                .Include(o => o.OrderItems)
                .Include("OrderItems.Product")
                .Include("OrderItems.Menu")
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public IEnumerable<Order> GetOrdersByUser(int userId)
        {
            return _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems.Select(oi => oi.Product))
                .Include(o => o.OrderItems.Select(oi => oi.Menu))
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public IEnumerable<Order> GetActiveOrders()
        {
            return _context.Orders
                .Where(o => o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Canceled)
                .Include(o => o.User)
                .Include(o => o.OrderItems.Select(oi => oi.Product))
                .Include(o => o.OrderItems.Select(oi => oi.Menu))
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public IEnumerable<Order> GetOrdersByStatus(OrderStatus status)
        {
            return _context.Orders
                .Where(o => o.Status == status)
                .Include(o => o.User)
                .Include(o => o.OrderItems.Select(oi => oi.Product))
                .Include(o => o.OrderItems.Select(oi => oi.Menu))
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public Order GetOrderWithDetails(int id)
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .Include("OrderItems.Product")
                .Include("OrderItems.Menu")
                .FirstOrDefault(o => o.Id == id);
        }
    }


}
