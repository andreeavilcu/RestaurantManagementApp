using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using RestaurantManagementApp.Models;

namespace RestaurantManagementApp.DataAccess.Repositories
{
    public interface IProductAllergenRepository
    {
        IEnumerable<ProductAllergen> GetAll();
        ProductAllergen GetById(int productId, int allergenId);
        IEnumerable<ProductAllergen> GetByProductId(int productId);
        void Add(ProductAllergen entity);
        void Remove(ProductAllergen entity);
        void RemoveRange(IEnumerable<ProductAllergen> entities);
    }

    public class ProductAllergenRepository : IProductAllergenRepository
    {
        protected readonly DatabaseContext _context;
        protected readonly DbSet<ProductAllergen> _dbSet;

        public ProductAllergenRepository(DatabaseContext context)
        {
            _context = context;
            _dbSet = context.ProductAllergens;
        }

        public IEnumerable<ProductAllergen> GetAll()
        {
            return _dbSet.ToList();
        }

        public ProductAllergen GetById(int productId, int allergenId)
        {
            return _dbSet.Find(productId, allergenId);
        }

        public IEnumerable<ProductAllergen> GetByProductId(int productId)
        {
            return _dbSet.Where(pa => pa.ProductId == productId).ToList();
        }

        public void Add(ProductAllergen entity)
        {
            _dbSet.Add(entity);
        }

        public void Remove(ProductAllergen entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<ProductAllergen> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}