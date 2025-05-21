using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;

namespace RestaurantManagementApp.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    { 
        private readonly IUserService _userService;

        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private string _deliveryAddress;

        private bool _hasSuccessMessage;
        private string _successMessage;
        private bool _hasErrorMessage;
        private string _errorMessage;

        private bool _hasPasswordSuccessMessage;
        private string _passwordSuccessMessage;
        private bool _hasPasswordErrorMessage;
        private string _passwordErrorMessage;


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

        public bool HasSuccessMessage
        {
            get => _hasSuccessMessage;
            set => SetProperty(ref _hasSuccessMessage, value);
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
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

        public bool HasPasswordSuccessMessage
        {
            get => _hasPasswordSuccessMessage;
            set => SetProperty(ref _hasPasswordSuccessMessage, value);
        }

        public string PasswordSuccessMessage
        {
            get => _passwordSuccessMessage;
            set => SetProperty(ref _passwordSuccessMessage, value);
        }

        public bool HasPasswordErrorMessage
        {
            get => _hasPasswordErrorMessage;
            set => SetProperty(ref _hasPasswordErrorMessage, value);
        }

        public string PasswordErrorMessage
        {
            get => _passwordErrorMessage;
            set => SetProperty(ref _passwordErrorMessage, value);
        }

        public ICommand SaveChangesCommand { get; }
        public ICommand ChangePasswordCommand { get; }


        public ProfileViewModel(IUserService userService)
        {
            _userService = userService;

            SaveChangesCommand = new RelayCommand(ExecuteSaveChanges);
            ChangePasswordCommand = new RelayCommand(ExecuteChangePassword);

            LoadUserData();
        }

        private void LoadUserData()
        {
            var user = _userService.GetCurrentUser();
            if (user != null)
            {
                FirstName = user.FirstName;
                LastName = user.LastName;
                Email = user.Email;
                PhoneNumber = user.PhoneNumber;
                DeliveryAddress = user.DeliveryAddress;
            }
        }

        private void ExecuteSaveChanges(object parameter)
        {
            HasSuccessMessage = false;
            HasErrorMessage = false;

            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            {
                HasErrorMessage = true;
                ErrorMessage = "First and last name are mandatory.";
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                HasErrorMessage = true;
                ErrorMessage = "Phone number is mandatory";
                return;
            }

            if (string.IsNullOrWhiteSpace(DeliveryAddress))
            {
                HasErrorMessage = true;
                ErrorMessage = "Delivery address is mandatory";
                return;
            }

            try 
            { 
                var user = _userService.GetCurrentUser();
                if (user != null)
                {
                    user.FirstName = FirstName;
                    user.LastName = LastName;
                    user.PhoneNumber = PhoneNumber;
                    user.DeliveryAddress = DeliveryAddress;

                    bool success = _userService.UpdateUser(user);
                    if (success)
                    {
                        HasSuccessMessage = true;
                        SuccessMessage = "The data has been successfully updated.";
                    }
                    else
                    {
                        HasErrorMessage = true;
                        ErrorMessage = "Could not update the profile. Please try again.";
                    }
                }
                else
                {
                    HasErrorMessage = true;
                    ErrorMessage = "User is not authenticated.";
                }
            }

            catch (Exception ex)
            {
                HasErrorMessage = true;
                ErrorMessage = $"Eroare la actualizarea profilului: {ex.Message}";
            }
        }

        private void ExecuteChangePassword(object parameter)
        {
            HasPasswordSuccessMessage = false;
            HasPasswordErrorMessage = false;

            
            string currentPassword = string.Empty;
            string newPassword = string.Empty;
            string confirmPassword = string.Empty;

            if (parameter is Panel panel)
            {
                var passwordBoxes = new List<PasswordBox>();
                FindPasswordBoxes(panel, passwordBoxes);

                if (passwordBoxes.Count >= 3)
                {
                    currentPassword = passwordBoxes[0].Password;
                    newPassword = passwordBoxes[1].Password;
                    confirmPassword = passwordBoxes[2].Password;
                }
            }

            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                HasPasswordErrorMessage = true;
                PasswordErrorMessage = "Please enter your current password.";
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                HasPasswordErrorMessage = true;
                PasswordErrorMessage = "Please enter a new password.";
                return;
            }

            if (newPassword != confirmPassword)
            {
                HasPasswordErrorMessage = true;
                PasswordErrorMessage = "The new passwords do not match.";
                return;
            }

            try
            {
                bool success = _userService.ChangePassword(Email, currentPassword, newPassword);
                if (success)
                {
                    HasPasswordSuccessMessage = true;
                    PasswordSuccessMessage = "Password changed successfully.";

                    
                    if (parameter is Panel panel2)
                    {
                        var passwordBoxes = new List<PasswordBox>();
                        FindPasswordBoxes(panel2, passwordBoxes);

                        foreach (var passwordBox in passwordBoxes)
                        {
                            passwordBox.Clear();
                        }
                    }
                }
                else
                {
                    HasPasswordErrorMessage = true;
                    PasswordErrorMessage = "The current password is incorrect.";
                }
            }
            catch (Exception ex)
            {
                HasPasswordErrorMessage = true;
                PasswordErrorMessage = $"Error changing password: {ex.Message}";
            }
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
