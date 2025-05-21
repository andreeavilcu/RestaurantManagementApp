using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantManagementApp.ViewModels.Base;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;

namespace RestaurantManagementApp.ViewModels
{
    public class CartItemViewModel : ViewModelBase
    {
        private int _id;
        private string _name;
        private string _portionInfo;
        private decimal _unitPrice;
        private int _quantity;
        private bool _isProduct;


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

        public string PortionInfo
        {
            get => _portionInfo;
            set => SetProperty(ref _portionInfo, value);
        }

        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                if (SetProperty(ref _unitPrice, value))
                {
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (SetProperty(ref _quantity, value))
                {
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public bool IsProduct
        {
            get => _isProduct;
            set => SetProperty(ref _isProduct, value);
        }

        public decimal TotalPrice => UnitPrice * Quantity;
    }
}