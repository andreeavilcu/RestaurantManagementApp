using System;
using System.Windows;
using System.Windows.Input;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;
using RestaurantManagementApp.ViewModels.Authentication;
using RestaurantManagementApp.Views;
using RestaurantManagementApp.ViewModels.EmployeeViewModels;
using System.IO;

namespace RestaurantManagementApp.ViewModels
{
    public class MainWindowViewModel :  ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;
        private readonly IInventoryService _inventoryService;
        private readonly ICartPersistenceService _cartPersistenceService;
        private readonly IConfigurationService _configService;
        private CartViewModel _cartViewModel;


        private object _currentView;
        private string _currentViewTitle;
        private string _searchText;
        private bool _isSearchVisible;
        private bool _isLoggedIn;
        private bool _isEmployee;
        private string _userDisplayName;

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string CurrentViewTitle
        {
            get => _currentViewTitle;
            set => SetProperty(ref _currentViewTitle, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public bool IsSearchVisible
        {
            get => _isSearchVisible;
            set => SetProperty(ref _isSearchVisible, value);
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }

        public bool IsEmployee
        {
            get => _isEmployee;
            set => SetProperty(ref _isEmployee, value);
        }

        public string UserDisplayName
        {
            get => _userDisplayName;
            set => SetProperty(ref _userDisplayName, value);
        }

        public ICommand ShowMenuCommand { get; private set; }
        public ICommand ShowSearchCommand{ get; private set; }
        public ICommand  ShowOrdersCommand{ get; private set; }
        public ICommand  ShowCartCommand{ get; private set; }
        public ICommand  ShowProfileCommand{ get; private set; }
        public ICommand  ShowLoginCommand{ get; private set; }
        public ICommand  ShowRegisterCommand{ get; private set; }
        public ICommand  LogoutCommand{ get; private set; }

        
        
        public ICommand  ShowManageCategoriesCommand{ get; private set; }
        public ICommand  ShowManageProductsCommand{ get; private set; }
        public ICommand  ShowManageMenusCommand{ get; private set; }
        public ICommand  ShowManageOrdersCommand{ get; private set; }
        public ICommand ShowManageAllergensCommand { get; private set; }
        public ICommand  ShowReportsCommand{ get; private set; }

        public ICommand  SearchCommand{ get; private set; }


        public MainWindowViewModel(IUserService userService, IMenuService menuService, IOrderService orderService)
        {
            _userService = userService;
            _menuService = menuService ?? new MenuService(_userService.GetUnitOfWork(), new ConfigurationService());
            _orderService = orderService;
            _configService = new ConfigurationService();
            _inventoryService = new InventoryService(_userService.GetUnitOfWork(), _configService);
            _cartPersistenceService = new CartPersistence();

            
            _cartViewModel = new CartViewModel(_orderService, _menuService, _userService, _cartPersistenceService, this);

            
            ShowMenuCommand = new RelayCommand(param => NavigateToMenu());
            ShowSearchCommand = new RelayCommand(param => NavigateToSearch());
            ShowOrdersCommand = new RelayCommand(param => NavigateToOrders(), param => IsLoggedIn);
            ShowCartCommand = new RelayCommand(param => NavigateToCart(), param => IsLoggedIn);
            ShowProfileCommand = new RelayCommand(param => NavigateToProfile(), param => IsLoggedIn);
            ShowLoginCommand = new RelayCommand(param => NavigateToLogin(), param => !IsLoggedIn);
            ShowRegisterCommand = new RelayCommand(param => NavigateToRegister(), param => !IsLoggedIn);
            LogoutCommand = new RelayCommand(param => Logout(), param => IsLoggedIn);

            ShowManageCategoriesCommand = new RelayCommand(param => NavigateToManageCategories(), param => IsEmployee);
            ShowManageProductsCommand = new RelayCommand(param => NavigateToManageProducts(), param => IsEmployee);
            ShowManageMenusCommand = new RelayCommand(param => NavigateToManageMenus(), param => IsEmployee);
            ShowManageOrdersCommand = new RelayCommand(param => NavigateToManageOrders(), param => IsEmployee);
            ShowReportsCommand = new RelayCommand(param => NavigateToReports(), param => IsEmployee);
            ShowManageAllergensCommand = new RelayCommand(param => NavigateToManageAllergens(), param => IsEmployee);
            SearchCommand = new RelayCommand(param => ExecuteSearch());

            
            CheckSavedLogin();

           
            NavigateToMenu();
        }

        private void NavigateToMenu()
        {
            CurrentView = new RestaurantMenuView()
            {
                DataContext = new RestaurantViewModel(_menuService, _cartViewModel)
            };

            CurrentViewTitle = "Restaurant menu";
            IsSearchVisible = true;
        }

        private void NavigateToSearch()
        {
            NavigateToMenu();
            IsSearchVisible = true;
        }

        private void NavigateToOrders()
        {
            CurrentView = new OrdersView()
            {
                DataContext = new OrdersViewModel(_orderService, _userService, this)
            };
            CurrentViewTitle = "My Orders";
            IsSearchVisible = false;
        }

        private void NavigateToCart()
        {
            CurrentView = new CartView()
            {
                DataContext = _cartViewModel
            };
            CurrentViewTitle = "My Cart";
            IsSearchVisible = false;
        }

        private void NavigateToProfile()
        {
            CurrentView = new ProfileView()
            {
                DataContext = new ProfileViewModel(_userService)
            };
            CurrentViewTitle = "My Profile";
            IsSearchVisible = false;
        }

        private void NavigateToLogin()
        {
            CurrentView = new LoginView()
            {
                DataContext = new LoginViewModel(_userService, this)
            };
            CurrentViewTitle = "Login";
            IsSearchVisible = false;
        }

        private void NavigateToRegister()
        {
            CurrentView = new RegisterView()
            {
                DataContext = new RegisterViewModel(_userService, this)
            };
            CurrentViewTitle = "Register";
            IsSearchVisible = false;
        }

        private void NavigateToManageCategories()
        {
            CurrentView = new ManageCategoriesView()
            {
                DataContext = new ManageCategoriesViewModel(_menuService)
            };
            CurrentViewTitle = "Manage Categories";
            IsSearchVisible = false;
        }

        private void NavigateToManageProducts()
        {
            CurrentView = new ManageProductsView()
            {
                DataContext = new ManageProductsViewModel(_menuService, _inventoryService)
            };
            CurrentViewTitle = "Manage Products";
            IsSearchVisible = false;
        }

        private void NavigateToManageMenus()
        {
            CurrentView = new ManageMenusView()
            {
                DataContext = new ManageMenusViewModel(_menuService, _configService)
            };
            CurrentViewTitle = "Manage Menus";
            IsSearchVisible = false;
        }

        private void NavigateToManageOrders()
        {
            CurrentView = new ManageOrdersView()
            {
                DataContext = new ManageOrdersViewModel(_orderService, _userService)
            };
            CurrentViewTitle = "Manage Orders";
            IsSearchVisible = false;
        }

        private void NavigateToManageAllergens()
        {
            var configService = new ConfigurationService();
            CurrentView = new ManageAllergensView
            {
                DataContext = new ManageAllergensViewModel(_menuService)
            };
            CurrentViewTitle = "Manage Allergens";
            IsSearchVisible = false;
        }

        private void NavigateToReports()
        {
            CurrentView = new ReportsView()
            {
                DataContext = new ReportsViewModel(_orderService, _menuService, _inventoryService)
            };
            CurrentViewTitle = "Reports";
            IsSearchVisible = false;
        }

        private void ExecuteSearch()
        {
            if(CurrentView is RestaurantMenuView menuView && menuView.DataContext is RestaurantViewModel menuVM)
            {
                menuVM.SearchText = SearchText;
                menuVM.SearchCommand.Execute(null);
            }
        }

        private void Logout()
        {
            ClearLoginToken();
            IsLoggedIn = false;
            IsEmployee = false;
            UserDisplayName = null;

            _userService.Logout();
            NavigateToMenu();

            MessageBox.Show("You have been successfully logged out.", "Logout", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearLoginToken()
        {
            try
            {
                string tokenFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RestaurantManagementApp",
                    "auth_token.dat");

                if (File.Exists(tokenFilePath))
                {
                    File.Delete(tokenFilePath);
                    System.Diagnostics.Debug.WriteLine("Login token cleared");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing login token: {ex.Message}");
            }
        }

        private void CheckSavedLogin()
        {
            System.Diagnostics.Debug.WriteLine("Verificare login salvat...");
            try
            {

                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RestaurantManagementApp");

                string tokenFilePath = Path.Combine(appDataPath, "auth_token.dat");


                if (!File.Exists(tokenFilePath))
                {
                    return;
                }


                string[] authData = File.ReadAllLines(tokenFilePath);
                if (authData.Length < 2)
                {
                    return;
                }

                string savedEmail = authData[0];
                string savedToken = authData[1];
                string expiryDateStr = authData.Length > 2 ? authData[2] : string.Empty;


                if (!string.IsNullOrEmpty(expiryDateStr) &&
                    DateTime.TryParse(expiryDateStr, out DateTime expiryDate) &&
                    expiryDate < DateTime.Now)
                {

                    File.Delete(tokenFilePath);
                    return;
                }


                var user = _userService.GetUserByEmail(savedEmail);
                if (user == null)
                {

                    File.Delete(tokenFilePath);
                    return;
                }

               
                string expectedToken = GenerateToken(user);


                if (string.Equals(savedToken, expectedToken))
                {

                    _userService.SetCurrentUser(user);
                    IsLoggedIn = true;
                    IsEmployee = user.Role == UserRole.Employee;
                    UserDisplayName = user.FullName;

                    System.Diagnostics.Debug.WriteLine($"Auto-login successful for: {user.Email}");
                }
                else
                {

                    File.Delete(tokenFilePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during auto-login: {ex.Message}");

            }

        }

        
        private string GenerateToken(User user)
        {
            
            string secretKey = "RestaurantAppSecretKey2023"; 
            string dataToHash = $"{user.Id}|{user.PasswordHash}|{secretKey}";

            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dataToHash));
                return Convert.ToBase64String(hashBytes);
            }
        }

        public void HandleSuccessfulLogin(User user)
        {
            System.Diagnostics.Debug.WriteLine($"HandleSuccessfulLogin: Utilizator={user.Email}, ID={user.Id}, Rol={user.Role}");

            IsLoggedIn = true;
            IsEmployee = user.Role == UserRole.Employee;
            UserDisplayName = user.FullName;

            System.Diagnostics.Debug.WriteLine($"IsLoggedIn setat: {IsLoggedIn}, IsEmployee: {IsEmployee}, DisplayName: {UserDisplayName}");

            
            System.Diagnostics.Debug.WriteLine($"Login successful - IsEmployee: {IsEmployee}, Role: {user.Role}");

            
            CommandManager.InvalidateRequerySuggested();

            NavigateToMenu();
        }

    }
}
