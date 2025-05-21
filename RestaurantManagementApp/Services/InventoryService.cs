using RestaurantManagementApp.DataAccess;
using RestaurantManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagementApp.Services
{
    public interface IInventoryService
    {
        IEnumerable<Product> GetLowStockProducts();
        bool UpdateProductStock(int productId, decimal newQuantity);
        bool CheckProductAvailability(int productId, decimal requiredQuantity);
        void UpdateStockForOrder(Order order);
    }

    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IConfigurationService _configService;

        public InventoryService(IUnitOfWork unitOfWork, IConfigurationService configService)
        {
            _unitOfWork = unitOfWork;
            _configService = configService;
        }

        public IEnumerable<Product> GetLowStockProducts()
        {
            decimal threshold = _configService.GetLowStockThreshold();
            return _unitOfWork.Products.GetLowStockProducts(threshold);
        }

        public bool UpdateProductStock(int productId, decimal newQuantity)
        {
            var product = _unitOfWork.Products.GetById(productId);
            if (product == null)
                return false;

            product.TotalQuantity = newQuantity;
            product.UpdateAvailability();
            _unitOfWork.Complete();
            return true;
        }

        public bool CheckProductAvailability(int productId, decimal requiredQuantity)
        {
            var product = _unitOfWork.Products.GetById(productId);
            if (product == null)
                return false;

            return product.HasSufficientStock(requiredQuantity);
        }

        public void UpdateStockForOrder(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                if (item.ProductId.HasValue)
                {
                    var product = _unitOfWork.Products.GetById(item.ProductId.Value);
                    decimal requiredQuantity = item.Quantity * product.PortionQuantity;
                    product.TotalQuantity -= requiredQuantity;
                    product.UpdateAvailability();
                }
                else if (item.MenuId.HasValue)
                {
                    var menu = _unitOfWork.Menus.GetById(item.MenuId.Value);
                    foreach (var menuProduct in menu.MenuProducts)
                    {
                        var product = menuProduct.Product;
                        decimal requiredQuantity = item.Quantity * menuProduct.Quantity;
                        product.TotalQuantity -= requiredQuantity;
                        product.UpdateAvailability();
                    }
                    menu.UpdateAvailability();
                }
            }
            _unitOfWork.Complete();
        }
    }
}
