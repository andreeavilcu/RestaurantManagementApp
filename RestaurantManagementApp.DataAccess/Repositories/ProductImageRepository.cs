using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using RestaurantManagementApp.Models;

namespace RestaurantManagementApp.DataAccess.Repositories
{
    public interface IProductImageRepository
    {
        IEnumerable<ProductImage> GetAll();
        ProductImage GetById(int id);
        IEnumerable<ProductImage> GetByProductId(int productId);
        void Add(ProductImage entity);
        void Remove(ProductImage entity);
        void RemoveRange(IEnumerable<ProductImage> entities);
    }

    public class ProductImageRepository : IProductImageRepository
    {
        protected readonly DatabaseContext _context;
        protected readonly DbSet<ProductImage> _dbSet;

        public ProductImageRepository(DatabaseContext context)
        {
            _context = context;
            _dbSet = context.ProductImages;
        }

        public IEnumerable<ProductImage> GetAll()
        {
            return _dbSet.ToList();
        }

        public ProductImage GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<ProductImage> GetByProductId(int productId)
        {
            return _dbSet.Where(pi => pi.ProductId == productId).ToList();
        }

        public void Add(ProductImage entity)
        {
            _dbSet.Add(entity);
        }

        public void Remove(ProductImage entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<ProductImage> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}