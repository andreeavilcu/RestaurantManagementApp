using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using System.Text;

namespace RestaurantManagementApp.ViewModels.EmployeeViewModels
{
    public class ManageProductsViewModel : ViewModelBase
    {
        private readonly IMenuService _menuService;
        private IInventoryService _inventoryService;

        private ObservableCollection<ProductAdminViewModel> _products;
        private bool _isEditing;
        private ProductAdminViewModel _editingProduct;
        private string _formTitle;
        private bool _isNewProduct;
        private bool _hasErrorMessage;
        private string _errorMessage;
        private string _searchText;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Allergen> _allergens;
        private ObservableCollection<Allergen> _selectedAllergens;
        private int? _selectedAllergenId;


        public int? SelectedAllergenId
        {
            get => _selectedAllergenId;
            set => SetProperty(ref _selectedAllergenId, value);
        }
        public ObservableCollection<ProductAdminViewModel> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public ProductAdminViewModel EditingProduct
        {
            get => _editingProduct;
            set => SetProperty(ref _editingProduct, value);
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

        public bool IsNewProduct
        {
            get => _isNewProduct;
            set => SetProperty(ref _isNewProduct, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<Allergen> Allergens
        {
            get => _allergens;
            set => SetProperty(ref _allergens, value);
        }

        public ObservableCollection<Allergen> SelectedAllergens
        {
            get => _selectedAllergens;
            set => SetProperty(ref _selectedAllergens, value);
        }

        public bool HasProducts => Products.Count > 0;

        public ICommand AddNewCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddAllergenCommand { get; }
        public ICommand RemoveAllergenCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand RemoveImageCommand { get; }
        
        
        public ManageProductsViewModel(IMenuService menuService, IInventoryService inventoryService)
        {

            _menuService = menuService;
            _inventoryService = inventoryService;

            Products = new ObservableCollection<ProductAdminViewModel>();
            Categories = new ObservableCollection<Category>();
            Allergens = new ObservableCollection<Allergen>();
            SelectedAllergens = new ObservableCollection<Allergen>();

            
            AddNewCommand = new RelayCommand(ExecuteAddNew);
            EditCommand = new RelayCommand(ExecuteEdit);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            SaveCommand = new RelayCommand(ExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);
            SearchCommand = new RelayCommand(ExecuteSearch);
            AddAllergenCommand = new RelayCommand(ExecuteAddAllergen);
            RemoveAllergenCommand = new RelayCommand(ExecuteRemoveAllergen);
            AddImageCommand = new RelayCommand(ExecuteAddImage);
            RemoveImageCommand = new RelayCommand(ExecuteRemoveImage);
            

            

            LoadProducts();
            LoadCategories();
            LoadAllergens();
        }

      

        private void LoadProducts()
        {
            Products.Clear();

            var products = _menuService.GetAllProducts();
            foreach (var product in products)
            {
                Products.Add(new ProductAdminViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    PortionQuantity = product.PortionQuantity,
                    MeasurementUnit = product.MeasurementUnit,
                    TotalQuantity = product.TotalQuantity,
                    IsAvailable = product.IsAvailable,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name,
                    SelectedAllergenIds = new ObservableCollection<int>(
                        product.ProductAllergens?.Select(pa => pa.AllergenId) ?? new List<int>()),
                    Images = new ObservableCollection<string>(
                            product.Images?.Select(i => i.ImagePath) ?? new List<string>())
                });
            }

            OnPropertyChanged(nameof(HasProducts));
        }



        private void LoadCategories()
        {
            Categories.Clear();
            var categories = _menuService.GetAllCategories();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        private void LoadAllergens()
        {
            Allergens.Clear();
            var allergens = _menuService.GetAllAllergens();
            foreach (var allergen in allergens)
            {
                Allergens.Add(allergen);
            }
        }

        private void ExecuteAddNew(object Parameter)
        {
            IsEditing = true;
            IsNewProduct = true;
            FormTitle = "Add New Product";
            EditingProduct = new ProductAdminViewModel
            {
                SelectedAllergenIds = new ObservableCollection<int>(),
                Images = new ObservableCollection<string>()
            };
            SelectedAllergens.Clear();
            HasErrorMessage = false;
        }

        private void ExecuteEdit(object parameter)
        {
            if (parameter is int productId)
            {
                var product = Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    IsEditing = true;
                    IsNewProduct = false;
                    FormTitle = "Edit Product";

                    EditingProduct = new ProductAdminViewModel
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        PortionQuantity = product.PortionQuantity,
                        MeasurementUnit = product.MeasurementUnit,
                        TotalQuantity = product.TotalQuantity,
                        IsAvailable = product.IsAvailable,
                        CategoryId = product.CategoryId,
                        SelectedAllergenIds = new ObservableCollection<int>(product.SelectedAllergenIds),
                        Images = new ObservableCollection<string>(product.Images)
                    };

                    SelectedAllergens.Clear();
                    foreach (var allergenId in product.SelectedAllergenIds)
                    {
                        var allergen = _menuService.GetAllergenById(allergenId);
                        if (allergen != null)
                        {
                            SelectedAllergens.Add(allergen);
                        }
                    }

                    HasErrorMessage = false;
                }
            }
        }

        private void ExecuteDelete(object parameter)
        {
            if (parameter is int productId)
            {
                var product = Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    var isUsedInMenus = _menuService.GetAllMenus()
                        .Any(m => m.MenuProducts != null && m.MenuProducts.Any(mp => mp.ProductId == productId));

                    if (isUsedInMenus)
                    {
                        MessageBox.Show("This product cannot be deleted because it is used in one or more menus.",
                            "Deletion Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var result = MessageBox.Show($"Are you sure you want to delete the product '{product.Name}'?",
                        "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            DeleteProduct(productId);
                            LoadProducts();
                            MessageBox.Show("The product was successfully deleted.",
                                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error while deleting the product: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void DeleteProduct(int productId)
        {
           
            var product = _menuService.GetProductById(productId);
            if (product != null && product.ProductAllergens != null)
            {
                foreach (var allergen in product.ProductAllergens.ToList())
                {
                    
                    _menuService.RemoveProductAllergen(productId, allergen.AllergenId);
                }
            }

            
            if (product != null && product.Images != null)
            {
                foreach (var image in product.Images.ToList())
                {
                    
                    _menuService.RemoveProductImage(productId, image.Id);
                }
            }

            
            _menuService.DeleteProduct(productId);
        }

        private void ExecuteSave(object parameter)
        {
            HasErrorMessage = false;

            
            if (string.IsNullOrWhiteSpace(EditingProduct.Name))
            {
                HasErrorMessage = true;
                ErrorMessage = "Product name is required.";
                return;
            }

            if (EditingProduct.Price <= 0)
            {
                HasErrorMessage = true;
                ErrorMessage = "Price must be greater than zero.";
                return;
            }

            if (EditingProduct.PortionQuantity <= 0)
            {
                HasErrorMessage = true;
                ErrorMessage = "Portion quantity must be greater than zero.";
                return;
            }

            if (EditingProduct.CategoryId <= 0)
            {
                HasErrorMessage = true;
                ErrorMessage = "A category must be selected.";
                return;
            }

            if (string.IsNullOrWhiteSpace(EditingProduct.MeasurementUnit))
            {
                HasErrorMessage = true;
                ErrorMessage = "Measurement unit is required.";
                return;
            }

            try
            {
                Product product;

                if (IsNewProduct)
                {
                    product = new Product
                    {
                        Name = EditingProduct.Name,
                        Description = EditingProduct.Description,
                        Price = EditingProduct.Price,
                        PortionQuantity = EditingProduct.PortionQuantity,
                        MeasurementUnit = EditingProduct.MeasurementUnit,
                        TotalQuantity = EditingProduct.TotalQuantity,
                        IsAvailable = EditingProduct.IsAvailable,
                        CategoryId = EditingProduct.CategoryId
                    };

                    int productId = SaveNewProduct(product);
                    EditingProduct.Id = productId;
                }
                else
                {
                    product = _menuService.GetProductById(EditingProduct.Id);
                    if (product == null)
                    {
                        HasErrorMessage = true;
                        ErrorMessage = "Product not found.";
                        return;
                    }

                    product.Name = EditingProduct.Name;
                    product.Description = EditingProduct.Description;
                    product.Price = EditingProduct.Price;
                    product.PortionQuantity = EditingProduct.PortionQuantity;
                    product.MeasurementUnit = EditingProduct.MeasurementUnit;
                    product.TotalQuantity = EditingProduct.TotalQuantity;
                    product.IsAvailable = EditingProduct.IsAvailable;
                    product.CategoryId = EditingProduct.CategoryId;

                    UpdateExistingProduct(product);
                }

                UpdateProductAllergens(EditingProduct.Id, EditingProduct.SelectedAllergenIds.ToList());
                System.Diagnostics.Debug.WriteLine("========== DEBUGGING SALVARE IMAGINI ==========");
                System.Diagnostics.Debug.WriteLine($"ID produs: {EditingProduct.Id}");
                System.Diagnostics.Debug.WriteLine($"Număr imagini în EditingProduct.Images: {EditingProduct.Images.Count}");
                foreach (var imgPath in EditingProduct.Images)
                {
                    System.Diagnostics.Debug.WriteLine($"Cale imagine: {imgPath}");
                }
                UpdateProductImages(EditingProduct.Id, EditingProduct.Images.ToList());

                
                if (product.TotalQuantity <= 0)
                {
                    product.IsAvailable = false;
                    _inventoryService.UpdateProductStock(product.Id, product.TotalQuantity);
                }

                LoadProducts();
                ExecuteCancel(null);

                MessageBox.Show(IsNewProduct ?
                    "The product was added successfully." :
                    "The product was updated successfully.",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HasErrorMessage = true;
                ErrorMessage = $"Error while saving the product: {ex.Message}";
            }
        }


        private int SaveNewProduct(Product product)
        {
            
            return _menuService.AddProduct(product);
        }

        private void UpdateExistingProduct(Product product)
        {
            
            _menuService.UpdateProduct(product);
        }



        private void UpdateProductAllergens(int productId, List<int> newAllergenIds)
        {
            
            var existingAllergens = _menuService.GetProductById(productId)?.ProductAllergens?.ToList();
            if (existingAllergens == null)
                return;

            
            var existingAllergenIds = existingAllergens.Select(pa => pa.AllergenId).ToList();

            
            var allergenIdsToRemove = existingAllergenIds.Except(newAllergenIds).ToList();

            
            var allergenIdsToAdd = newAllergenIds.Except(existingAllergenIds).ToList();

           
            foreach (var allergenId in allergenIdsToRemove)
            {
                _menuService.RemoveProductAllergen(productId, allergenId);
            }

            
            foreach (var allergenId in allergenIdsToAdd)
            {
                _menuService.AddProductAllergen(productId, allergenId);
            }
        }

        
        private void UpdateProductImages(int productId, List<string> newImagePaths)
        {
            
            var product = _menuService.GetProductById(productId);
            if (product == null || product.Images == null)
                return;

            
            List<string> normalizedNewPaths = new List<string>();
            foreach (var path in newImagePaths)
            {
                
                if (Path.IsPathRooted(path))
                {
                    
                    normalizedNewPaths.Add(path);
                }
                else
                {
                    
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                    normalizedNewPaths.Add(fullPath);
                }
            }

            
            var existingImagePaths = product.Images.Select(img => img.ImagePath).ToList();

           
            List<string> normalizedExistingPaths = new List<string>();
            foreach (var path in existingImagePaths)
            {
                if (Path.IsPathRooted(path))
                {
                    normalizedExistingPaths.Add(path);
                }
                else
                {
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                    normalizedExistingPaths.Add(fullPath);
                }
            }

            
            var imagesToRemove = product.Images
                .Where(img => !normalizedNewPaths.Contains(img.ImagePath) &&
                              !normalizedNewPaths.Contains(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, img.ImagePath)))
                .ToList();

            
            var imagePathsToAdd = normalizedNewPaths
                .Where(path => !normalizedExistingPaths.Contains(path) &&
                               !existingImagePaths.Contains(path))
                .ToList();

            
            System.Diagnostics.Debug.WriteLine($"Updating images for product {productId}:");
            System.Diagnostics.Debug.WriteLine($"Images to remove: {imagesToRemove.Count}");
            System.Diagnostics.Debug.WriteLine($"Images to add: {imagePathsToAdd.Count}");

            
            foreach (var image in imagesToRemove)
            {
                System.Diagnostics.Debug.WriteLine($"Removing image: {image.ImagePath}");
                _menuService.RemoveProductImage(productId, image.Id);
            }

           
            foreach (var imagePath in imagePathsToAdd)
            {
                System.Diagnostics.Debug.WriteLine($"Adding image: {imagePath}");
                _menuService.AddProductImage(productId, imagePath);
            }
        }

        private void ExecuteCancel(object parameter)
        {
            IsEditing = false;
            IsNewProduct = false;
            FormTitle = string.Empty;
            EditingProduct = null;
            SelectedAllergens.Clear();
            HasErrorMessage = false;
        }
        

        private void ExecuteSearch(object parameter)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadProducts();
                
                return;
            }

            Products.Clear();
            var products = _menuService.SearchProducts(SearchText);

            foreach (var product in products)
            {
                Products.Add(new ProductAdminViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    PortionQuantity = product.PortionQuantity,
                    MeasurementUnit = product.MeasurementUnit,
                    TotalQuantity = product.TotalQuantity,
                    IsAvailable = product.IsAvailable,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name,
                    SelectedAllergenIds = new ObservableCollection<int>(
                        product.ProductAllergens?.Select(pa => pa.AllergenId) ?? new List<int>()),
                    Images = new ObservableCollection<string>(
    product.Images?.Select(i => NormalizeImagePath(i.ImagePath)) ?? new List<string>())
                });
            }

            OnPropertyChanged(nameof(HasProducts));
        }

        private string NormalizeImagePath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return null;
            string fileName = Path.GetFileName(imagePath);
            return Path.Combine("Images", "Products", fileName);
        }

        private void ExecuteAddAllergen(object parameter)
        {
            if (parameter is int allergenId)
            {
                var allergen = _menuService.GetAllergenById(allergenId);
                if (allergen != null && !EditingProduct.SelectedAllergenIds.Contains(allergenId))
                {
                    EditingProduct.SelectedAllergenIds.Add(allergenId);
                    SelectedAllergens.Add(allergen);
                }
            }
        }
        private void ExecuteRemoveAllergen(object parameter)
        {
            if (parameter is int allergenId)
            {
                EditingProduct.SelectedAllergenIds.Remove(allergenId);
                var allergen = SelectedAllergens.FirstOrDefault(a => a.Id == allergenId);
                if (allergen != null)
                {
                    SelectedAllergens.Remove(allergen);
                }
            }
        }

        
        private void ExecuteAddImage(object parameter)
        {
            if (EditingProduct == null)
                return;

            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select an image",
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.gif|All files|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string selectedFilePath = openFileDialog.FileName;
                    string fileName = System.IO.Path.GetFileName(selectedFilePath);

                   
                    string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string productsDirectory = System.IO.Path.Combine(appDirectory, "Images", "Products");

                    
                    if (!System.IO.Directory.Exists(productsDirectory))
                        System.IO.Directory.CreateDirectory(productsDirectory);

                    string destinationPath = System.IO.Path.Combine(productsDirectory, fileName);


                    bool fileAlreadyExists = System.IO.File.Exists(destinationPath);

                    
                    System.IO.File.Copy(selectedFilePath, destinationPath, true);

                    
                    if (!System.IO.File.Exists(destinationPath))
                    {
                        MessageBox.Show("Failed to copy image file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    
                    try
                    {
                        var testBitmap = new BitmapImage();
                        testBitmap.BeginInit();
                        testBitmap.CacheOption = BitmapCacheOption.OnLoad;

                        using (var stream = new FileStream(destinationPath, FileMode.Open, FileAccess.Read))
                        {
                            testBitmap.StreamSource = stream;
                            testBitmap.EndInit();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Image file is not valid: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                   
                    EditingProduct.Images.Add(destinationPath);
                   

                    MessageBox.Show(fileAlreadyExists ?
                        "Image was updated successfully." :
                        "Image was added successfully.",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while adding image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteRemoveImage(object parameter)
        {
            if (parameter is string imagePath && EditingProduct != null)
            {
                EditingProduct.Images.Remove(imagePath);
            }
        }

        
    }
}
