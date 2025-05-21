using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;

namespace RestaurantManagementApp.ViewModels.EmployeeViewModels
{
    public class CategoryAdminViewModel : ViewModelBase
    {
        private int _id;
        private string _name;
        private string _description;
        private int _productCount;
        private int _menuCount;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public int ProductCount
        {
            get => _productCount;
            set => SetProperty(ref _productCount, value);
        }

        public int MenuCount
        {
            get => _menuCount;
            set => SetProperty(ref _menuCount, value);
        }
    }
}
