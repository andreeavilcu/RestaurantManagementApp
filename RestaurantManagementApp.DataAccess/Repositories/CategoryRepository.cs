using RestaurantManagementApp.DataAccess.Repositories.Implementations;
using RestaurantManagementApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;


namespace RestaurantManagementApp.DataAccess.Repositories
{
    public interface ICategoryRepository : IRepository<Models.Category>
    {
        IEnumerable<Models.Category> GetCategoriesWithProducts();
    }


    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(DatabaseContext context) : base(context)
        {
        }

        public IEnumerable<Category> GetCategoriesWithProducts()
        {
            return _context.Categories
                .Include(c => c.Products)
                .Include(c => c.Menus)
                .ToList();
        }
    }
}
