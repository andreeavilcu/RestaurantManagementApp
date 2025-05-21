using RestaurantManagementApp.ViewModels.Base;

namespace RestaurantManagementApp.ViewModels.EmployeeViewModels
{
    public class AllergenViewModel : ViewModelBase
    {
        private int _id;
        private string _name;
        private string _description;
        private int _productCount;

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
    }
}