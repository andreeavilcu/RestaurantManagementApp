using RestaurantManagementApp.DataAccess.Repositories.Implementations;
using RestaurantManagementApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.SqlClient;

namespace RestaurantManagementApp.DataAccess.Repositories
{
    public interface IProductRepository : IRepository<Models.Product>
    {
        IEnumerable<Models.Product> GetProductsWithDetails();
        IEnumerable<Models.Product> GetProductsByCategory(int categoryId);
        IEnumerable<Models.Product> GetProductsWithAllergen(int allergenId);
        IEnumerable<Models.Product> GetProductsWithoutAllergen(int allergenId);
        IEnumerable<Models.Product> SearchProducts(string keyword);
        IEnumerable<Models.Product> GetLowStockProducts(decimal threshold);
    }


    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(DatabaseContext context) : base(context)
        {
        }

        public IEnumerable<Product> GetProductsWithDetails()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductAllergens.Select(pa => pa.Allergen))
                .Include(p => p.Images)
                .ToList();
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Database.SqlQuery<Product>("EXEC SP_GetProductsByCategory @CategoryId",
                 new SqlParameter("@CategoryId", categoryId)).ToList();
        }

        public IEnumerable<Product> GetProductsWithAllergen(int allergenId)
        {
            return _context.Products
                .Where(p => p.ProductAllergens.Any(pa => pa.AllergenId == allergenId))
                .Include(p => p.Category)
                .Include(p => p.ProductAllergens.Select(pa => pa.Allergen))
                .ToList();
        }

        public IEnumerable<Product> GetProductsWithoutAllergen(int allergenId)
        {
            return _context.Products
                .Include(p => p.Category)                       
                .Include(p => p.ProductAllergens)               
                .Include(p => p.ProductAllergens.Select(pa => pa.Allergen))  
                .Include(p => p.Images)                        
                .Where(p => !p.ProductAllergens.Any(pa => pa.AllergenId == allergenId))
                .ToList(); 
        }

        public IEnumerable<Product> SearchProducts(string keyword)
        {
            return _context.Products
         .Include(p => p.Images)
         .Include(p => p.Category)
         .Include(p => p.ProductAllergens.Select(pa => pa.Allergen))
         .Where(p => p.Name.Contains(keyword) ||
                     p.Description.Contains(keyword) ||
                     p.Category.Name.Contains(keyword))
         .ToList();
        }

        public IEnumerable<Product> GetLowStockProducts(decimal threshold)
        {
            return _context.Database.SqlQuery<Product>("EXEC SP_GetLowStockProducts @Threshold",
                new SqlParameter("@Threshold", threshold)).ToList();
        }
    }
}
