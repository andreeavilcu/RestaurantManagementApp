using RestaurantManagementApp.DataAccess.Repositories;
using RestaurantManagementApp.Models;
using System;
using System.Data.Entity;


namespace RestaurantManagementApp.DataAccess
{

    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IMenuRepository Menus { get; }
        IMenuProductRepository MenuProducts { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IUserRepository Users { get; }
        IAllergenRepository Allergens { get; }

        IProductAllergenRepository ProductAllergens { get; }
        IProductImageRepository ProductImages { get; }


        int Complete();

        void Save();
        DbContextTransaction BeginTransaction();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;
        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }
        public IMenuRepository Menus { get; private set; }

        public IMenuProductRepository MenuProducts { get; private set; }
        public IOrderRepository Orders { get; private set; }

        public IOrderItemRepository OrderItems { get; private set; }
        public IUserRepository Users { get; private set; }
        public IAllergenRepository Allergens { get; private set; }

        public IProductAllergenRepository ProductAllergens { get; private set; }
        public IProductImageRepository ProductImages { get; private set; }

        public UnitOfWork (DatabaseContext context)
        {  
            _context = context;
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            Menus = new MenuRepository(_context);
            MenuProducts = new MenuProductRepository(_context);
            Orders = new OrderRepository(_context);
            OrderItems = new OrderItemRepository(_context);
            Users = new UserRepository(_context);
            Allergens = new AllergenRepository(_context);
            ProductAllergens = new ProductAllergenRepository(_context);
            ProductImages = new ProductImageRepository(_context);

        }

        public int Complete() 
        {
            return _context.SaveChanges();
        }

        public void Dispose() 
        {
            _context.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
            
        }

        public DbContextTransaction BeginTransaction() 
        {
            return _context.Database.BeginTransaction();
        }

    }
}
