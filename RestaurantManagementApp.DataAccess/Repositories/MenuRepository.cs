using RestaurantManagementApp.DataAccess.Repositories.Implementations;
using RestaurantManagementApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace RestaurantManagementApp.DataAccess.Repositories
{
    public interface IMenuRepository : IRepository<Models.Menu>
    {
        IEnumerable<Models.Menu> GetMenusWithDetails();
        IEnumerable<Models.Menu> GetMenusByCategory(int categoryId);
        IEnumerable<Models.Menu> SearchMenus(string keyword);
    }

    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        public MenuRepository(DatabaseContext context) : base(context)
        {
        }

        public IEnumerable<Menu> GetMenusWithDetails()
        {
            return _context.Menus
                .Include(m => m.Category)
                .Include(m => m.MenuProducts.Select(mp => mp.Product))
                .ToList();
        }

        public IEnumerable<Menu> GetMenusByCategory(int categoryId)
        {
            return _context.Menus
                .Where(m => m.CategoryId == categoryId)
                .Include(m => m.MenuProducts.Select(mp => mp.Product))
                .ToList();
        }

        public IEnumerable<Menu> SearchMenus(string keyword)
        {
            return _context.Menus
                .Where(m => m.Name.Contains(keyword) || m.Description.Contains(keyword))
                .Include(m => m.Category)
                .Include(m => m.MenuProducts.Select(mp => mp.Product))
                .ToList();
        }
    }
}
