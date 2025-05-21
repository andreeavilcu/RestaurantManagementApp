using System.Collections.ObjectModel;
using RestaurantManagementApp.ViewModels.Base;

namespace RestaurantManagementApp.ViewModels
{
    public class MenuItemViewModel : ViewModelBase
    {
        private int _id;
        private string _name;
        private string _description;
        private decimal _price;
        private decimal _portionQuantity;
        private string _measurementUnit;
        private bool _isAvailable;
        private string _categoryName;
        private ObservableCollection<string> _allergens;
        private ObservableCollection<string> _images;
        private bool _isProduct;
        private bool _isSelected;


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

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public decimal PortionQuantity
        {
            get => _portionQuantity;
            set => SetProperty(ref _portionQuantity, value);
        }

        public string MeasurementUnit
        {
            get => _measurementUnit;
            set => SetProperty(ref _measurementUnit, value);
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set => SetProperty(ref _isAvailable, value);
        }

        public string CategoryName
        {
            get => _categoryName;
            set => SetProperty(ref _categoryName, value);
        }

        public ObservableCollection<string> Allergens
        {
            get => _allergens;
            set => SetProperty(ref _allergens, value);
        }

        public ObservableCollection<string> Images
        {
            get => _images;
            set => SetProperty(ref _images, value);
        }

        public bool IsProduct
        {
            get => _isProduct;
            set => SetProperty(ref _isProduct, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string DisplayText => $"{Name} - {PortionQuantity} {MeasurementUnit}";
        public string PriceText => $"{Price:C2}";

        public string AvailabilityText => IsAvailable ? "Disponibil" : "Indisponibil";


    }
    
}
