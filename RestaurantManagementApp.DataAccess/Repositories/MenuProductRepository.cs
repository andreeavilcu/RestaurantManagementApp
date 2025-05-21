using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using RestaurantManagementApp.Models;

namespace RestaurantManagementApp.DataAccess.Repositories
{
    public interface IMenuProductRepository
    {
        IEnumerable<MenuProduct> GetAll();
        MenuProduct GetById(int menuId, int productId);
        IEnumerable<MenuProduct> GetByMenuId(int menuId);
        void Add(MenuProduct entity);
        void Remove(MenuProduct entity);
        void RemoveRange(IEnumerable<MenuProduct> entities);
    }

    public class MenuProductRepository : IMenuProductRepository
    {
        protected readonly DatabaseContext _context;
        protected readonly DbSet<MenuProduct> _dbSet;

        public MenuProductRepository(DatabaseContext context)
        {
            _context = context;
            _dbSet = context.MenuProducts;
        }

        public IEnumerable<MenuProduct> GetAll()
        {
            return _dbSet.ToList();
        }

        public MenuProduct GetById(int menuId, int productId)
        {
            return _dbSet.Find(menuId, productId);
        }

        public IEnumerable<MenuProduct> GetByMenuId(int menuId)
        {
            return _dbSet
                .Where(mp => mp.MenuId == menuId)
                .Include(mp => mp.Product)
                .ToList();
        }

        public void Add(MenuProduct entity)
        {
            _dbSet.Add(entity);
        }

        public void Remove(MenuProduct entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<MenuProduct> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}