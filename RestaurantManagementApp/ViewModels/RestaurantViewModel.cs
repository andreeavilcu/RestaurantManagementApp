using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;
using RestaurantManagementApp.Models;
using System.Windows.Input;
using System.Data.Entity.Migrations.Model;
using System.Windows.Controls.Primitives;
using RestaurantManagementApp.ViewModels.EmployeeViewModels;
using System.Windows;

namespace RestaurantManagementApp.ViewModels
{
    public class RestaurantViewModel : ViewModelBase
    {
        private readonly IMenuService _menuService;
        private readonly CartViewModel _cartViewModel;
        private ObservableCollection<CategoryViewModel> _categories;
        private string _searchText;
        private int? _selectedAllergenId;

        public ObservableCollection<CategoryViewModel> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    SearchCommand.Execute(null);
                }
            }
        }

        public int? SelectedAllergenId
        {
            get => _selectedAllergenId;
            set
            {
                if (SetProperty(ref _selectedAllergenId, value))
                {
                    FilterByAllergenCommand.Execute(null);
                }
            }
        }

        public ObservableCollection<Allergen> Allergens { get; private set; }
        
        public ICommand SearchCommand { get; private set; }
        public ICommand FilterByAllergenCommand { get; private set; }
        public ICommand LoadMenuCommand { get; private set; }
        public ICommand CLearFiltersCommand { get; private set; }
        public ICommand AddToCartCommand { get; private set; }

        public RestaurantViewModel(IMenuService menuService, CartViewModel cartViewModel)
        {
            _menuService = menuService;
            _cartViewModel = cartViewModel;
            Categories = new ObservableCollection<CategoryViewModel>();
            Allergens = new ObservableCollection<Allergen>(_menuService.GetAllAllergens());

            SearchCommand = new RelayCommand(ExecuteSearch);
            FilterByAllergenCommand = new RelayCommand(ExecuteFilterByAllergen);
            LoadMenuCommand = new RelayCommand(ExecuteLoadMenu);
            CLearFiltersCommand = new RelayCommand(ExecuteClearFilters);
            AddToCartCommand = new RelayCommand<MenuItemViewModel>(ExecuteAddToCart);
            LoadMenuCommand.Execute(null);
        }


        private void ExecuteAddToCart(MenuItemViewModel item)
        {
            if (_cartViewModel == null)
            {
                MessageBox.Show("Cart is not available", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (item.IsProduct)
            {
                var product = _menuService.GetProductById(item.Id);
                _cartViewModel.AddToCart(product);
            }
            else
            {
                var menu = _menuService.GetMenuById(item.Id);
                _cartViewModel.AddToCart(menu);
            }
        }
        private void ExecuteSearch(object parameter)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            { 
                LoadMenuCommand.Execute(null);
                return;
            }

            var products = _menuService.SearchProducts(SearchText);
            var menus = _menuService.SearchMenus(SearchText);

            UpdateMenuDisplay(products, menus);
        }


        private void ExecuteFilterByAllergen(object parameter)
        {
            if (!_selectedAllergenId.HasValue)
            {
                LoadMenuCommand.Execute(null);
                return;
            }

            
            var productsWithoutAllergen = _menuService.GetProductsWithoutAllergen(SelectedAllergenId.Value);

           
            foreach (var product in productsWithoutAllergen)
            {
                System.Diagnostics.Debug.WriteLine($"Filtered product: {product.Name}, Images count: {product.Images?.Count ?? 0}");

                
                if (product.Images != null)
                {
                    foreach (var img in product.Images)
                    {
                        System.Diagnostics.Debug.WriteLine($"Image path for {product.Name}: {img.ImagePath}");
                    }
                }
            }

            var allMenus = _menuService.GetAllMenus();
            var filteredMenus = new List<Menu>();

            foreach (var menu in allMenus)
            {
                bool menuValid = true;
                if (menu.MenuProducts != null)
                {
                    foreach (var menuProduct in menu.MenuProducts)
                    {
                        if (menuProduct.Product != null && menuProduct.Product.ProductAllergens != null)
                        {
                            if (menuProduct.Product.ProductAllergens.Any(pa => pa.AllergenId == SelectedAllergenId.Value))
                            {
                                menuValid = false;
                                break;
                            }
                        }
                    }
                }

                if (menuValid)
                {
                    filteredMenus.Add(menu);
                }
            }

            
            UpdateMenuDisplay(productsWithoutAllergen, filteredMenus);
        }

        private void ExecuteLoadMenu(object parameter)
        {
            var products = _menuService.GetAllProducts();
            var menus = _menuService.GetAllMenus();

            UpdateMenuDisplay(products, menus);
        }

        private void ExecuteClearFilters(object parameter)
        {
            SearchText = string.Empty;
            SelectedAllergenId = null;
            LoadMenuCommand.Execute(null);
        }

        private void UpdateMenuDisplay(IEnumerable<Product> products, IEnumerable<Menu> menus)
        {
            Categories.Clear();

            var categoryDictionary = new Dictionary<int, CategoryViewModel>();

            foreach (var category in _menuService.GetAllCategories())
            {
                var categoryViewModel = new CategoryViewModel()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Items = new ObservableCollection<MenuItemViewModel>(),
                };

                categoryDictionary.Add(category.Id, categoryViewModel);
            }

            foreach (var product in products)
            {
                if (categoryDictionary.TryGetValue(product.CategoryId, out CategoryViewModel category))
                {
                    
                    var imagesUrls = new List<string>();
                    if (product.Images != null)
                    {
                        
                        System.Diagnostics.Debug.WriteLine($"Processing product {product.Name} with {product.Images.Count} images");

                        
                        foreach (var img in product.Images)
                        {
                            if (!string.IsNullOrEmpty(img.ImagePath))
                            {
                                imagesUrls.Add(img.ImagePath);
                                System.Diagnostics.Debug.WriteLine($"Adding image path: {img.ImagePath}");
                            }
                        }
                    }

                    
                    category.Items.Add(new MenuItemViewModel
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        PortionQuantity = product.PortionQuantity,
                        MeasurementUnit = product.MeasurementUnit,
                        IsAvailable = product.IsAvailable,
                        CategoryName = category.Name,
                        Allergens = new ObservableCollection<string>(
                            product.ProductAllergens?.Select(pa => pa.Allergen?.Name).Where(n => n != null).ToList() ?? new List<string>()),
                        Images = new ObservableCollection<string>(imagesUrls),
                        IsProduct = true
                    });
                }
            }

            foreach (var menu in menus)
            {
                if (categoryDictionary.TryGetValue(menu.CategoryId, out CategoryViewModel category))
                {
                    
                    var menuImages = new List<string>();
                    

                    
                    var allergensInMenu = new HashSet<string>();
                    if (menu.MenuProducts != null)
                    {
                        foreach (var menuProduct in menu.MenuProducts)
                        {
                            if (menuProduct.Product?.ProductAllergens != null)
                            {
                                foreach (var allergen in menuProduct.Product.ProductAllergens)
                                {
                                    if (allergen.Allergen != null && !string.IsNullOrEmpty(allergen.Allergen.Name))
                                    {
                                        allergensInMenu.Add(allergen.Allergen.Name);
                                    }
                                }
                            }
                        }
                    }

                    category.Items.Add(new MenuItemViewModel
                    {
                        Id = menu.Id,
                        Name = menu.Name,
                        Description = menu.Description,
                        Price = menu.CalculatePrice(),
                        PortionQuantity = 1,  
                        MeasurementUnit = "portion",
                        IsAvailable = menu.IsAvailable,
                        CategoryName = category.Name,
                        Allergens = new ObservableCollection<string>(allergensInMenu),
                        Images = new ObservableCollection<string>(menuImages),
                        IsProduct = false  
                    });
                }
            }

            foreach (var category in categoryDictionary.Values.Where(c => c.Items.Count > 0))
            {
                Categories.Add(category);
            }
        }

    }
}
