using RestaurantManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagementApp.ViewModels.EmployeeViewModels
{
    public class MenuAdminViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ObservableCollection<MenuProductViewModel> MenuProducts { get; set; } = new ObservableCollection<MenuProductViewModel>();
    }

    public class MenuProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public string MeasurementUnit { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;

    }
}