using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using RestaurantManagementApp.Models;

namespace RestaurantManagementApp.DataAccess.Repositories
{
    public interface IOrderItemRepository
    {
        IEnumerable<OrderItem> GetAll();
        OrderItem GetById(int id);
        IEnumerable<OrderItem> GetByOrderId(int orderId);
        void Add(OrderItem entity);
        void AddRange(IEnumerable<OrderItem> entities);
        void Update(OrderItem entity);
        void Remove(OrderItem entity);
        void RemoveRange(IEnumerable<OrderItem> entities);
    }

    public class OrderItemRepository : IOrderItemRepository
    {
        protected readonly DatabaseContext _context;
        protected readonly DbSet<OrderItem> _dbSet;

        public OrderItemRepository(DatabaseContext context)
        {
            _context = context;
            _dbSet = context.OrderItems;
        }

        public IEnumerable<OrderItem> GetAll()
        {
            return _dbSet.ToList();
        }

        public OrderItem GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<OrderItem> GetByOrderId(int orderId)
        {
            return _dbSet
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.Product)
                .Include(oi => oi.Menu)
                .ToList();
        }

        public void Add(OrderItem entity)
        {
            _dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<OrderItem> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Update(OrderItem entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(OrderItem entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<OrderItem> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }

}
