using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;

namespace RestaurantManagementApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly MainWindowViewModel _mainWindowViewModel;

        private string _email;
        private bool _rememberMe;
        private bool _hasError;
        private string _errorMessage;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }


        public LoginViewModel(IUserService userService, MainWindowViewModel mainWindowViewModel)
        {
            _userService = userService;
            _mainWindowViewModel = mainWindowViewModel;

            LoginCommand = new RelayCommand(ExecuteLogin);
            RegisterCommand = new RelayCommand(ExecuteRegister);

        }

        private void ExecuteLogin(object parameter)
        {
            System.Diagnostics.Debug.WriteLine("============= ÎNCEPUT AUTENTIFICARE =============");
            System.Diagnostics.Debug.WriteLine($"Încercare de autentificare pentru email: {Email}");

            HasError = false;

            if (string.IsNullOrWhiteSpace(Email))
            {
                System.Diagnostics.Debug.WriteLine("Eroare: Email gol");
                HasError = true;
                ErrorMessage = "Please enter your email address.";
                return;
            }

            string password = string.Empty;
            if (parameter is PasswordBox passwordBox)
            {
                password = passwordBox.Password;
                System.Diagnostics.Debug.WriteLine($"Parola introdusă: {(string.IsNullOrEmpty(password) ? "GOALĂ" : "COMPLETATĂ")}");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                System.Diagnostics.Debug.WriteLine("Eroare: Parola goală");
                HasError = true;
                ErrorMessage = "Please enter your password.";
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("Apel către UserService.Authenticate");
                var user = _userService.Authenticate(Email, password);

                if (user != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Autentificare reușită pentru utilizatorul: {user.Email}, Rol: {user.Role}");

                    if (RememberMe)
                    {
                        System.Diagnostics.Debug.WriteLine($"Salvare credențiale pentru: {Email}");
                        SaveLoginToken(user);
                    }

                    _mainWindowViewModel.HandleSuccessfulLogin(user);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Autentificare eșuată: utilizator null");
                    HasError = true;
                    ErrorMessage = "Incorrect email or password.";
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepție în timpul autentificării: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                HasError = true;
                ErrorMessage = $"Authentication error: {ex.Message}";
            }

            System.Diagnostics.Debug.WriteLine("============= SFÂRȘIT AUTENTIFICARE =============");
        }

        private void ExecuteRegister(object parameter)
        {
            _mainWindowViewModel.ShowRegisterCommand.Execute(null);
        }

        private void SaveLoginToken(User user)
        {
            if (!RememberMe)
                return;

            try
            {
                
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RestaurantManagementApp");

                
                if (!Directory.Exists(appDataPath))
                    Directory.CreateDirectory(appDataPath);

                string tokenFilePath = Path.Combine(appDataPath, "auth_token.dat");

               
                string token = GenerateToken(user);

                
                string expiryDate = DateTime.Now.AddDays(30).ToString("o");

                
                File.WriteAllLines(tokenFilePath, new[]
                {
            user.Email,
            token,
            expiryDate
        });

                System.Diagnostics.Debug.WriteLine($"Login token saved for: {user.Email}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving login token: {ex.Message}");
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

    }
}
