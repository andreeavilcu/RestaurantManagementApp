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
    public class ManageCategoriesViewModel : ViewModelBase
    {
        private readonly IMenuService _menuService;

        private ObservableCollection<CategoryAdminViewModel> _categories;
        private bool _isEditing;
        private CategoryAdminViewModel _editingCategory;
        private string _formTitle;
        private bool _isNewCategory;
        private bool _hasErrorMessage;
        private string _errorMessage;

        public ObservableCollection<CategoryAdminViewModel> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public CategoryAdminViewModel EditingCategory
        {
            get => _editingCategory;
            set => SetProperty(ref _editingCategory, value);
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

        public bool IsNewCategory
        {
            get => _isNewCategory;
            set => SetProperty(ref _isNewCategory, value);
        }

        public bool HasCategories => Categories.Count > 0;

        public ICommand AddNewCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ManageCategoriesViewModel(IMenuService menuService)
        {
            _menuService = menuService;

            Categories = new ObservableCollection<CategoryAdminViewModel>();

            AddNewCommand = new RelayCommand(ExecuteAddNew);
            EditCommand = new RelayCommand(ExecuteEdit);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            SaveCommand = new RelayCommand(ExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);

            LoadCategories();

        }

        private void LoadCategories()
        {
            Categories.Clear();

            var categories = _menuService.GetAllCategories();
            foreach (var category in categories)
            {
                Categories.Add(new CategoryAdminViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ProductCount = category.Products?.Count ?? 0,
                    MenuCount = category.Menus?.Count ?? 0

                });
            }

            OnPropertyChanged(nameof(HasCategories));
        }


        private void ExecuteAddNew(object parameter)
        {
            IsEditing = true;
            IsNewCategory = true;
            FormTitle = "Add new category";
            EditingCategory = new CategoryAdminViewModel();
            HasErrorMessage = false;
        }

        private void ExecuteEdit(object parameter)
        {
            if (parameter is int categoryId)
            {
                var category = Categories.FirstOrDefault(c => c.Id == categoryId);
                if (category != null)
                {
                    IsEditing = true;
                    IsNewCategory = false;
                    FormTitle = "Edit category";

                    EditingCategory = new CategoryAdminViewModel
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description,

                    };

                    HasErrorMessage = false;

                }
            }
        }

        private void ExecuteDelete(object parameter)
        {
            if (parameter is int categoryId)
            {
                var category = Categories.FirstOrDefault(c => c.Id == categoryId);
                if (category != null)
                {
                    if (category.ProductCount > 0 || category.MenuCount > 0)
                    {
                        MessageBox.Show("You cannot delete a category that contains products or menus.\n\nPlease move the products and menus to other categories before deleting this category.",
                        "Deletion Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var result = MessageBox.Show($"Are you sure you want to delete the category '{category.Name}'?",
                        "Deletion Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {

                        try
                        {
                            bool success = _menuService.DeleteCategory(categoryId);
                            if (success)
                            {
                                LoadCategories();
                            }
                            else
                            {
                                MessageBox.Show("Could not delete the category. Please check if there are any products or menus using it.",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred while deleting the category: {ex.Message}",
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

            if (string.IsNullOrWhiteSpace(EditingCategory.Name))
            {
                HasErrorMessage = true;
                ErrorMessage = "The Name of the category is mandatory.";
                return;
            }

            try
            {
                if (IsNewCategory)
                {
                    var newCategory = new Category
                    {
                        Name = EditingCategory.Name,
                        Description = EditingCategory.Description
                    };

                    _menuService.AddCategory(newCategory);
                }
                else
                {
                    
                    var category = _menuService.GetCategoryById(EditingCategory.Id);
                    if (category == null)
                    {
                        HasErrorMessage = true;
                        ErrorMessage = "Category not found.";
                        return;
                    }

                    category.Name = EditingCategory.Name;
                    category.Description = EditingCategory.Description;

                    _menuService.UpdateCategory(category);
                }

                LoadCategories();
                ExecuteCancel(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the category: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void ExecuteCancel(object parameter)
        {
            IsEditing = false;
            IsNewCategory = false;
            FormTitle = string.Empty;
            EditingCategory = null;
            HasErrorMessage = false;
        }
    }
}
