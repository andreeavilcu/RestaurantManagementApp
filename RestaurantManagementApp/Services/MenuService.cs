using RestaurantManagementApp.DataAccess;
using RestaurantManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagementApp.Services
{
    public interface IMenuService
    {
        IEnumerable<Category> GetAllCategories();
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Menu> GetAllMenus();
        IEnumerable<Allergen> GetAllAllergens();

        IEnumerable<Product> GetProductsByCategory(int categoryId);
        IEnumerable<Menu> GetMenusByCategory(int categoryId);

        IEnumerable<Product> SearchProducts(string keyword);
        IEnumerable<Menu> SearchMenus(string keyword);

        IEnumerable<Product> GetProductsWithAllergen(int allergenId);
        IEnumerable<Product> GetProductsWithoutAllergen(int allergenId);

        Product GetProductById(int id);
        Menu GetMenuById(int id);
        Category GetCategoryById(int id);
        Allergen GetAllergenById(int id);

        bool DeleteCategory(int categoryId);

        void AddCategory(Category category);

        int AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int productId);

        void AddProductAllergen(int productId, int allergenId);
        void RemoveProductAllergen(int productId, int allergenId);

        void AddProductImage(int productId, string imagePath);
        void RemoveProductImage(int productId, int imageId);
        int AddMenu(Menu menu);
        void UpdateMenu(Menu menu);
        void DeleteMenu(int menuId);
        void UpdateMenuProducts(int menuId, List<MenuProduct> menuProducts);

        void AddAllergen(Allergen allergen);
        void UpdateAllergen(Allergen allergen);
        bool DeleteAllergen(int allergenId);

        void UpdateCategory(Category category);

    }

    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfigurationService _configService;

        public MenuService(IUnitOfWork unitOfWork, IConfigurationService configService)
        {
            _unitOfWork = unitOfWork;
            _configService = configService;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _unitOfWork.Categories.GetAll();
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _unitOfWork.Products.GetProductsWithDetails();
        }

        public IEnumerable<Menu> GetAllMenus()
        {
            return _unitOfWork.Menus.GetMenusWithDetails();
        }

        public IEnumerable<Allergen> GetAllAllergens()
        {
            return _unitOfWork.Allergens.GetAll();
        }

        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            return _unitOfWork.Products.GetProductsByCategory(categoryId);
        }

        public IEnumerable<Menu> GetMenusByCategory(int categoryId)
        {
            return _unitOfWork.Menus.GetMenusByCategory(categoryId);
        }

        public IEnumerable<Product> SearchProducts(string keyword)
        {
            return _unitOfWork.Products.SearchProducts(keyword);
        }

        public IEnumerable<Menu> SearchMenus(string keyword)
        {
            return _unitOfWork.Menus.SearchMenus(keyword);
        }

        public IEnumerable<Product> GetProductsWithAllergen(int allergenId)
        {
            return _unitOfWork.Products.GetProductsWithAllergen(allergenId);
        }

        public IEnumerable<Product> GetProductsWithoutAllergen(int allergenId)
        {
            var products = _unitOfWork.Products.GetProductsWithoutAllergen(allergenId);

            foreach (var product in products)
            {
                
                if (product.Images != null)
                {
                    var imageCount = product.Images.Count;
                }
            }

            return products;
        }

        public Product GetProductById(int id)
        {
            return _unitOfWork.Products.GetById(id);
        }

        public Menu GetMenuById(int id)
        {
            return _unitOfWork.Menus.GetById(id);
        }

        public Category GetCategoryById(int id)
        {
            return _unitOfWork.Categories.GetById(id);
        }

        public Allergen GetAllergenById(int id)
        {
            return _unitOfWork.Allergens.GetById(id);
        }

        public bool DeleteCategory(int categoryId)
        {
            var category = _unitOfWork.Categories.GetById(categoryId);
            if (category == null)
                return false;

            var hasProducts = _unitOfWork.Products.GetProductsByCategory(categoryId).Any();
            var hasMenus = _unitOfWork.Menus.GetMenusByCategory(categoryId).Any();

            if (hasProducts || hasMenus)
                return false;

            _unitOfWork.Categories.Remove(category);
            _unitOfWork.Save();
            return true;
        }

        public void AddCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            _unitOfWork.Categories.Add(category);
            _unitOfWork.Save();
        }

        public int AddProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            _unitOfWork.Products.Add(product);
            _unitOfWork.Complete();

            return product.Id;
        }

        public void UpdateProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var existingProduct = _unitOfWork.Products.GetById(product.Id);
            if (existingProduct == null)
                throw new InvalidOperationException($"Produsul cu ID-ul {product.Id} nu a fost găsit.");


            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.PortionQuantity = product.PortionQuantity;
            existingProduct.MeasurementUnit = product.MeasurementUnit;
            existingProduct.TotalQuantity = product.TotalQuantity;
            existingProduct.IsAvailable = product.IsAvailable;
            existingProduct.CategoryId = product.CategoryId;
                   


            _unitOfWork.Complete();
        }

        public void DeleteProduct(int productId)
        {
            var product = _unitOfWork.Products.GetById(productId);
            if (product == null)
                throw new InvalidOperationException($"Produsul cu ID-ul {productId} nu a fost găsit.");

            _unitOfWork.Products.Remove(product);
            _unitOfWork.Complete();
        }


        public void AddProductAllergen(int productId, int allergenId)
        {
            var product = _unitOfWork.Products.GetById(productId);
            var allergen = _unitOfWork.Allergens.GetById(allergenId);

            if (product == null)
                throw new InvalidOperationException($"Produsul cu ID-ul {productId} nu a fost găsit.");
            if (allergen == null)
                throw new InvalidOperationException($"Alergenul cu ID-ul {allergenId} nu a fost găsit.");

           
            if (product.ProductAllergens == null)
                product.ProductAllergens = new List<ProductAllergen>();

            if (!product.ProductAllergens.Any(pa => pa.AllergenId == allergenId))
            {
                product.ProductAllergens.Add(new ProductAllergen
                {
                    ProductId = productId,
                    AllergenId = allergenId
                });

                _unitOfWork.Complete();
            }
        }

        public void RemoveProductAllergen(int productId, int allergenId)
        {
            var productAllergen = _unitOfWork.ProductAllergens.GetById(productId, allergenId);
            if (productAllergen != null)
            {
                _unitOfWork.ProductAllergens.Remove(productAllergen);
                _unitOfWork.Complete();
            }
        }
        

        public void AddProductImage(int productId, string imagePath)
        {
            var product = _unitOfWork.Products.GetById(productId);

            if (product == null)
                throw new InvalidOperationException($"Produsul cu ID-ul {productId} nu a fost găsit.");

            if (product.Images == null)
                product.Images = new List<ProductImage>();

            product.Images.Add(new ProductImage
            {
                ProductId = productId,
                ImagePath = imagePath
            });

            _unitOfWork.Complete();
        }

        public void RemoveProductImage(int productId, int imageId)
        {
            var image = _unitOfWork.ProductImages.GetById(imageId);
            if (image != null && image.ProductId == productId)
            {
                _unitOfWork.ProductImages.Remove(image);
                _unitOfWork.Complete();
            }
        }

        public decimal CalculateMenuPrice(Menu menu)
        {
            if (menu == null)
                throw new ArgumentNullException(nameof(menu));

            decimal totalPrice = 0;
            foreach (var menuProduct in menu.MenuProducts)
            {
                var product = _unitOfWork.Products.GetById(menuProduct.ProductId);
                if (product != null)
                {
                    totalPrice += product.Price * menuProduct.Quantity;
                }
            }

            decimal discountPercentage = _configService.GetMenuDiscountPercentage();
            return totalPrice * (1 - discountPercentage / 100);
        }

        public int AddMenu(Menu menu)
        {
            if (menu == null)
                throw new ArgumentNullException(nameof(menu));

            _unitOfWork.Menus.Add(menu);
            _unitOfWork.Complete();

            return menu.Id;
        }

        public void UpdateMenu(Menu menu)
        {
            if (menu == null)
                throw new ArgumentNullException(nameof(menu));

            var existingMenu = _unitOfWork.Menus.GetById(menu.Id);
            if (existingMenu == null)
                throw new InvalidOperationException($"Menu with ID {menu.Id} not found.");

            existingMenu.Name = menu.Name;
            existingMenu.Description = menu.Description;
            existingMenu.IsAvailable = menu.IsAvailable;
            existingMenu.CategoryId = menu.CategoryId;

            _unitOfWork.Complete();
        }

        public void DeleteMenu(int menuId)
        {
            var menu = _unitOfWork.Menus.GetById(menuId);
            if (menu == null)
                throw new InvalidOperationException($"Menu with ID {menuId} not found.");

            _unitOfWork.Menus.Remove(menu);
            _unitOfWork.Complete();
        }

        public void UpdateMenuProducts(int menuId, List<MenuProduct> menuProducts)
        {
            var menu = _unitOfWork.Menus.GetById(menuId);
            if (menu == null)
                throw new InvalidOperationException($"Menu with ID {menuId} not found.");

           
            foreach (var menuProduct in menu.MenuProducts.ToList())
            {
                _unitOfWork.MenuProducts.Remove(menuProduct);
            }

            
            foreach (var menuProduct in menuProducts)
            {
                menuProduct.MenuId = menuId;
                _unitOfWork.MenuProducts.Add(menuProduct);
            }

            _unitOfWork.Complete();
        }

        public void AddAllergen(Allergen allergen)
        {
            if (allergen == null)
                throw new ArgumentNullException(nameof(allergen));

            _unitOfWork.Allergens.Add(allergen);
            _unitOfWork.Complete();
        }

        public void UpdateAllergen(Allergen allergen)
        {
            if (allergen == null)
                throw new ArgumentNullException(nameof(allergen));

            var existingAllergen = _unitOfWork.Allergens.GetById(allergen.Id);
            if (existingAllergen == null)
                throw new InvalidOperationException($"Allergen with ID {allergen.Id} not found.");

            existingAllergen.Name = allergen.Name;
            existingAllergen.Description = allergen.Description;

            _unitOfWork.Complete();
        }

        public bool DeleteAllergen(int allergenId)
        {
            var allergen = _unitOfWork.Allergens.GetById(allergenId);
            if (allergen == null)
                return false;

            
            var hasProducts = allergen.ProductAllergens != null && allergen.ProductAllergens.Any();
            if (hasProducts)
                return false;

            _unitOfWork.Allergens.Remove(allergen);
            _unitOfWork.Complete();
            return true;
        }

        public void UpdateCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var existingCategory = _unitOfWork.Categories.GetById(category.Id);
            if (existingCategory == null)
                throw new InvalidOperationException($"Categoria cu ID-ul {category.Id} nu a fost găsită.");

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            _unitOfWork.Complete();
        }
    }


}
