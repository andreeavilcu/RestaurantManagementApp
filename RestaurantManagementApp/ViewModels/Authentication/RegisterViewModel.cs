using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;

namespace RestaurantManagementApp.ViewModels.Authentication
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly MainWindowViewModel _mainWindowViewModel;


        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private string _deliveryAddress;
        private bool _agreeToTerms;
        private bool _hasError;
        private string _errorMessage;

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public string DeliveryAddress
        {
            get => _deliveryAddress;
            set => SetProperty(ref _deliveryAddress, value);
        }

        public bool AgreeToTerms
        {
            get => _agreeToTerms;
            set => SetProperty(ref _agreeToTerms, value);
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

        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        public RegisterViewModel(IUserService userService, MainWindowViewModel mainWindowViewModel)
        {
            _userService = userService;
            _mainWindowViewModel = mainWindowViewModel;

            RegisterCommand = new RelayCommand(ExecuteRegister);
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        private void ExecuteRegister(object parameter)
        {
            HasError = false;

            if(string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                HasError = true;
                ErrorMessage = "Please enter your first and last name.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                HasError = true;
                ErrorMessage = "Please enter your email.";
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                HasError = true;
                ErrorMessage = "Please enter your phone number.";
                return;
            }

            if (string.IsNullOrWhiteSpace(DeliveryAddress))
            {
                HasError = true;
                ErrorMessage = "Please enter your delivery address";
                return;
            }

            string password = string.Empty;
            string confirmPassword = string.Empty;

            

            if (parameter is Panel panel)
            {
                var passwordBoxes = new List<PasswordBox>();
                FindPasswordBoxes(panel, passwordBoxes);

                if (passwordBoxes.Count >= 2)
                {
                    password = passwordBoxes[0].Password;
                    confirmPassword = passwordBoxes[1].Password;
                }
            }

            

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

            //if (!(hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar))
            //{
            //    HasError = true;
            //    ErrorMessage = "Please enter a strong password that includes uppercase letters, lowercase letters, numbers, and special characters.";
            //    return;
            //}

            if (password != confirmPassword)
            {
                HasError = true;
                ErrorMessage = "Passwords do not match.";
                return;
            }

            if (!AgreeToTerms)
            {
                HasError = true;
                ErrorMessage = "You must agree to the terms and conditions to continue.";
                return;
            }

            try
            {
                var user = new User
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    DeliveryAddress = DeliveryAddress,
                    
                    Role = UserRole.Customer
                };

                bool isSuccess = _userService.RegisterUser(user, password);
                if (isSuccess)
                {
                    MessageBox.Show("Registration successful! You can now log in.",
                                  "Register", MessageBoxButton.OK, MessageBoxImage.Information);
                    _mainWindowViewModel.ShowLoginCommand.Execute(null);
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Registration failed. The email might already be in use.";
                }
            }

            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Registration error: {ex.Message}";
            }

        }

        private void ExecuteLogin(object parameter)
        {
            _mainWindowViewModel.ShowLoginCommand.Execute(null);
        }

        private void FindPasswordBoxes(DependencyObject parent, List<PasswordBox> passwordBoxes)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is PasswordBox passwordBox)
                {
                    passwordBoxes.Add(passwordBox);
                }
                else
                {
                    FindPasswordBoxes(child, passwordBoxes);
                }
            }
        }
    }
    
}
