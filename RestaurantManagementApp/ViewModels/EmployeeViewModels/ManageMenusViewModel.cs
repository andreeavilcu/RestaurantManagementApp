using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;

namespace RestaurantManagementApp.ViewModels.EmployeeViewModels
{
    public class ManageMenusViewModel : ViewModelBase
    {
        private readonly IMenuService _menuService;
        private readonly IConfigurationService _configService;

        private ObservableCollection<MenuAdminViewModel> _menus;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Product> _availableProducts;
        private ObservableCollection<MenuProductViewModel> _selectedProducts;

        private bool _isEditing;
        private MenuAdminViewModel _editingMenu;
        private string _formTitle;
        private bool _isNewMenu;
        private bool _hasErrorMessage;
        private string _errorMessage;
        private string _searchText;
        private Product _selectedProduct;
        private decimal _selectedQuantity;

        public ObservableCollection<MenuAdminViewModel> Menus
        {
            get => _menus;
            set => SetProperty(ref _menus, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<Product> AvailableProducts
        {
            get => _availableProducts;
            set => SetProperty(ref _availableProducts, value);
        }

        public ObservableCollection<MenuProductViewModel> SelectedProducts
        {
            get => _selectedProducts;
            set => SetProperty(ref _selectedProducts, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public MenuAdminViewModel EditingMenu
        {
            get => _editingMenu;
            set => SetProperty(ref _editingMenu, value);
        }

        public string FormTitle
        {
            get => _formTitle;
            set => SetProperty(ref _formTitle, value);
        }

        public bool HasErrorMessage
        {
            get => _hasErrorMessage;
            set => SetProperty(ref _hasErrorMessage, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsNewMenu
        {
            get => _isNewMenu;
            set => SetProperty(ref _isNewMenu, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public decimal SelectedQuantity
        {
            get => _selectedQuantity;
            set => SetProperty(ref _selectedQuantity, value);
        }

        public bool HasMenus => Menus.Count > 0;

        public ICommand AddNewCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand RemoveProductCommand { get; }


        public ManageMenusViewModel(IMenuService menuService, IConfigurationService configService)
        {
            _menuService = menuService;
            _configService = configService;

            Menus = new ObservableCollection<MenuAdminViewModel>();
            Categories = new ObservableCollection<Category>();
            AvailableProducts = new ObservableCollection<Product>();
            SelectedProducts = new ObservableCollection<MenuProductViewModel>();


            AddNewCommand = new RelayCommand(ExecuteAddNew);
            EditCommand = new RelayCommand(ExecuteEdit);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            SaveCommand = new RelayCommand(ExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);
            SearchCommand = new RelayCommand(ExecuteSearch);
            AddProductCommand = new RelayCommand(ExecuteAddProduct);
            RemoveProductCommand = new RelayCommand(ExecuteRemoveProduct);

            LoadMenus();
            LoadCategories();
            LoadProducts();
        }

        private void LoadMenus()
        {
            Menus.Clear();

            var menus = _menuService.GetAllMenus();
            foreach (var menu in menus)
            {
                var menuVM = new MenuAdminViewModel
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    Description = menu.Description,
                    Price = menu.CalculatePrice(),
                    IsAvailable = menu.IsAvailable,
                    CategoryId = menu.CategoryId,
                    CategoryName = menu.Category?.Name,
                    MenuProducts = new ObservableCollection<MenuProductViewModel>()
                };

                foreach (var menuProduct in menu.MenuProducts)
                {
                    menuVM.MenuProducts.Add(new MenuProductViewModel
                    {
                        ProductId = menuProduct.ProductId,
                        ProductName = menuProduct.Product?.Name,
                        UnitPrice = menuProduct.Product?.Price ?? 0,
                        Quantity = menuProduct.Quantity,
                        MeasurementUnit = menuProduct.Product?.MeasurementUnit
                    });
                }

                Menus.Add(menuVM);
            }

            OnPropertyChanged(nameof(HasMenus));
        }

        private void LoadCategories()
        {
            Categories.Clear();
            var categories = _menuService.GetAllCategories();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        private void LoadProducts()
        {
            AvailableProducts.Clear();
            var products = _menuService.GetAllProducts().Where(p => p.IsAvailable);
            foreach (var product in products)
            {
                AvailableProducts.Add(product);
            }
        }

        private void ExecuteAddNew(object parameter)
        {
            IsEditing = true;
            IsNewMenu = true;
            FormTitle = "Add New Menu";
            EditingMenu = new MenuAdminViewModel
            {
                MenuProducts = new ObservableCollection<MenuProductViewModel>()
            };
            SelectedProducts.Clear();
            HasErrorMessage = false;
            SelectedQuantity = 1;
        }

        private void ExecuteEdit(object parameter)
        {
            if (parameter is int menuId)
            {
                var menu = Menus.FirstOrDefault(m => m.Id == menuId);
                if (menu != null)
                {
                    IsEditing = true;
                    IsNewMenu = false;
                    FormTitle = "Edit Menu";

                    EditingMenu = new MenuAdminViewModel
                    {
                        Id = menu.Id,
                        Name = menu.Name,
                        Description = menu.Description,
                        Price = menu.Price,
                        IsAvailable = menu.IsAvailable,
                        CategoryId = menu.CategoryId,
                        MenuProducts = new ObservableCollection<MenuProductViewModel>(menu.MenuProducts)
                    };

                    SelectedProducts.Clear();
                    foreach (var product in menu.MenuProducts)
                    {
                        SelectedProducts.Add(product);
                    }

                    HasErrorMessage = false;
                }
            }
        }

        private void ExecuteDelete(object parameter)
        {
            if (parameter is int menuId)
            {
                var menu = Menus.FirstOrDefault(m => m.Id == menuId);
                if (menu != null)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete the menu '{menu.Name}'?",
                        "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _menuService.DeleteMenu(menuId);
                            LoadMenus();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error while deleting the menu: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void ExecuteSave(object parameter)
        {
            HasErrorMessage = false;

            if (string.IsNullOrWhiteSpace(EditingMenu.Name))
            {
                HasErrorMessage = true;
                ErrorMessage = "Menu name is required.";
                return;
            }

            if (EditingMenu.CategoryId <= 0)
            {
                HasErrorMessage = true;
                ErrorMessage = "A category must be selected.";
                return;
            }

            if (SelectedProducts.Count == 0)
            {
                HasErrorMessage = true;
                ErrorMessage = "At least one product must be added to the menu.";
                return;
            }

            try
            {
                Menu menu;

                if (IsNewMenu)
                {
                    menu = new Menu
                    {
                        Name = EditingMenu.Name,
                        Description = EditingMenu.Description,
                        IsAvailable = EditingMenu.IsAvailable,
                        CategoryId = EditingMenu.CategoryId,
                        MenuProducts = new List<MenuProduct>()
                    };

                    foreach (var product in SelectedProducts)
                    {
                        menu.MenuProducts.Add(new MenuProduct
                        {
                            ProductId = product.ProductId,
                            Quantity = product.Quantity
                        });
                    }

                    _menuService.AddMenu(menu);
                }
                else
                {
                    menu = _menuService.GetMenuById(EditingMenu.Id);
                    if (menu == null)
                    {
                        HasErrorMessage = true;
                        ErrorMessage = "Menu not found.";
                        return;
                    }

                    menu.Name = EditingMenu.Name;
                    menu.Description = EditingMenu.Description;
                    menu.IsAvailable = EditingMenu.IsAvailable;
                    menu.CategoryId = EditingMenu.CategoryId;

                    
                    _menuService.UpdateMenuProducts(menu.Id, SelectedProducts.Select(p =>
                        new MenuProduct
                        {
                            MenuId = menu.Id,
                            ProductId = p.ProductId,
                            Quantity = p.Quantity
                        }).ToList());
                }

                LoadMenus();
                ExecuteCancel(null);

                MessageBox.Show(IsNewMenu ?
                    "The menu was added successfully." :
                    "The menu was updated successfully.",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HasErrorMessage = true;
                ErrorMessage = $"Error while saving the menu: {ex.Message}";
            }
        }

        private void ExecuteCancel(object parameter)
        {
            IsEditing = false;
            IsNewMenu = false;
            FormTitle = string.Empty;
            EditingMenu = null;
            SelectedProducts.Clear();
            HasErrorMessage = false;
        }


        private void ExecuteSearch(object parameter)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadMenus();
                return;
            }

            Menus.Clear();
            var menus = _menuService.SearchMenus(SearchText);

            foreach (var menu in menus)
            {
                var menuVM = new MenuAdminViewModel
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    Description = menu.Description,
                    Price = menu.CalculatePrice(),
                    IsAvailable = menu.IsAvailable,
                    CategoryId = menu.CategoryId,
                    CategoryName = menu.Category?.Name,
                    MenuProducts = new ObservableCollection<MenuProductViewModel>()
                };

                foreach (var menuProduct in menu.MenuProducts)
                {
                    menuVM.MenuProducts.Add(new MenuProductViewModel
                    {
                        ProductId = menuProduct.ProductId,
                        ProductName = menuProduct.Product?.Name,
                        UnitPrice = menuProduct.Product?.Price ?? 0,
                        Quantity = menuProduct.Quantity,
                        MeasurementUnit = menuProduct.Product?.MeasurementUnit
                    });
                }

                Menus.Add(menuVM);
            }

            OnPropertyChanged(nameof(HasMenus));
        }

        private void ExecuteAddProduct(object parameter)
        {
            if (SelectedProduct == null || SelectedQuantity <= 0)
            {
                HasErrorMessage = true;
                ErrorMessage = "Please select a product and specify a valid quantity.";
                return;
            }

            var existingProduct = SelectedProducts.FirstOrDefault(p => p.ProductId == SelectedProduct.Id);
            if (existingProduct != null)
            {
                existingProduct.Quantity += SelectedQuantity;
            }
            else
            {
                SelectedProducts.Add(new MenuProductViewModel
                {
                    ProductId = SelectedProduct.Id,
                    ProductName = SelectedProduct.Name,
                    UnitPrice = SelectedProduct.Price,
                    Quantity = SelectedQuantity,
                    MeasurementUnit = SelectedProduct.MeasurementUnit
                });
            }

           
            if (EditingMenu != null)
            {
                EditingMenu.Price = CalculateMenuPrice();
            }

            SelectedProduct = null;
            SelectedQuantity = 1;
            HasErrorMessage = false;
        }

        private void ExecuteRemoveProduct(object parameter)
        {
            if (parameter is int productId)
            {
                var product = SelectedProducts.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    SelectedProducts.Remove(product);

                    // Update menu price
                    if (EditingMenu != null)
                    {
                        EditingMenu.Price = CalculateMenuPrice();
                    }
                }
            }
        }

        private decimal CalculateMenuPrice()
        {
            decimal totalPrice = SelectedProducts.Sum(p => p.TotalPrice);
            decimal discountPercentage = _configService.GetMenuDiscountPercentage();
            return totalPrice * (1 - discountPercentage / 100);
        }
    }


}
