using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;

namespace RestaurantManagementApp.ViewModels.EmployeeViewModels
{
    public class ProductAdminViewModel : ViewModelBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal PortionQuantity { get; set; }
        public string MeasurementUnit { get; set; }
        public decimal TotalQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ObservableCollection<int> SelectedAllergenIds { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<string> Images { get; set; } = new ObservableCollection<string>();
    }
}

