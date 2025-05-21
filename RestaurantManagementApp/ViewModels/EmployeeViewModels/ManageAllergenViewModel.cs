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
    public class ManageAllergensViewModel : ViewModelBase
    {
        private readonly IMenuService _menuService;

        private ObservableCollection<AllergenViewModel> _allergens;
        private bool _isEditing;
        private AllergenViewModel _editingAllergen;
        private string _formTitle;
        private bool _isNewAllergen;
        private bool _hasErrorMessage;
        private string _errorMessage;

        public ObservableCollection<AllergenViewModel> Allergens
        {
            get => _allergens;
            set => SetProperty(ref _allergens, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public AllergenViewModel EditingAllergen
        {
            get => _editingAllergen;
            set => SetProperty(ref _editingAllergen, value);
        }

        public string FormTitle
        {
            get => _formTitle;
            set => SetProperty(ref _formTitle, value);
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

        public bool IsNewAllergen
        {
            get => _isNewAllergen;
            set => SetProperty(ref _isNewAllergen, value);
        }

        public bool HasAllergens => Allergens.Count > 0;

        public ICommand AddNewCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ManageAllergensViewModel(IMenuService menuService)
        {
            _menuService = menuService;

            Allergens = new ObservableCollection<AllergenViewModel>();

            AddNewCommand = new RelayCommand(ExecuteAddNew);
            EditCommand = new RelayCommand(ExecuteEdit);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            SaveCommand = new RelayCommand(ExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);

            LoadAllergens();
        }

        private void LoadAllergens()
        {
            Allergens.Clear();

            var allergens = _menuService.GetAllAllergens();
            foreach (var allergen in allergens)
            {
                Allergens.Add(new AllergenViewModel
                {
                    Id = allergen.Id,
                    Name = allergen.Name,
                    Description = allergen.Description,
                    ProductCount = allergen.ProductAllergens?.Count ?? 0
                });
            }

            OnPropertyChanged(nameof(HasAllergens));
        }

        private void ExecuteAddNew(object parameter)
        {
            IsEditing = true;
            IsNewAllergen = true;
            FormTitle = "Add New Allergen";
            EditingAllergen = new AllergenViewModel();
            HasErrorMessage = false;
        }

        private void ExecuteEdit(object parameter)
        {
            if (parameter is int allergenId)
            {
                var allergen = Allergens.FirstOrDefault(a => a.Id == allergenId);
                if (allergen != null)
                {
                    IsEditing = true;
                    IsNewAllergen = false;
                    FormTitle = "Edit Allergen";

                    EditingAllergen = new AllergenViewModel
                    {
                        Id = allergen.Id,
                        Name = allergen.Name,
                        Description = allergen.Description
                    };

                    HasErrorMessage = false;
                }
            }
        }

        private void ExecuteDelete(object parameter)
        {
            if (parameter is int allergenId)
            {
                var allergen = Allergens.FirstOrDefault(a => a.Id == allergenId);
                if (allergen != null)
                {
                    if (allergen.ProductCount > 0)
                    {
                        MessageBox.Show("You cannot delete an allergen that is associated with products.\n\nPlease remove the allergen from all products before deleting it.",
                            "Deletion Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var result = MessageBox.Show($"Are you sure you want to delete the allergen '{allergen.Name}'?",
                        "Deletion Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            bool success = _menuService.DeleteAllergen(allergenId);
                            if (success)
                            {
                                LoadAllergens();
                            }
                            else
                            {
                                MessageBox.Show("Could not delete the allergen. Please check if there are any products using it.",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred while deleting the allergen: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }
            }
        }

        private void ExecuteSave(object parameter)
        {
            HasErrorMessage = false;

            if (string.IsNullOrWhiteSpace(EditingAllergen.Name))
            {
                HasErrorMessage = true;
                ErrorMessage = "The name of the allergen is mandatory.";
                return;
            }

            try
            {
                if (IsNewAllergen)
                {
                    var newAllergen = new Allergen
                    {
                        Name = EditingAllergen.Name,
                        Description = EditingAllergen.Description
                    };

                    _menuService.AddAllergen(newAllergen);
                }
                else
                {
                    var allergen = _menuService.GetAllergenById(EditingAllergen.Id);
                    if (allergen == null)
                    {
                        HasErrorMessage = true;
                        ErrorMessage = "Allergen not found.";
                        return;
                    }

                    allergen.Name = EditingAllergen.Name;
                    allergen.Description = EditingAllergen.Description;

                    _menuService.UpdateAllergen(allergen);
                }

                LoadAllergens();
                ExecuteCancel(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the allergen: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void ExecuteCancel(object parameter)
        {
            IsEditing = false;
            IsNewAllergen = false;
            FormTitle = string.Empty;
            EditingAllergen = null;
            HasErrorMessage = false;
        }


    }
}
