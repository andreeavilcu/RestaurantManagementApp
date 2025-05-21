using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;

namespace RestaurantManagementApp.ViewModels
{
    public class CartViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly IMenuService _menuService;
        private readonly IUserService _userService;
        private readonly ICartPersistenceService _cartPersistenceService;
        private readonly MainWindowViewModel _mainWindowViewModel;

        private ObservableCollection<CartItemViewModel> _cartItems;
        private decimal _subtotal;
        private decimal _deliveryCost;
        private decimal _discountAmount;
        private decimal _totalCost;
        private string _specialInstructions;
        private string _deliveryAddress;

        public ObservableCollection<CartItemViewModel> CartItems
        {
            get => _cartItems;
            set => SetProperty(ref _cartItems, value);
        }

        public decimal Subtotal
        {
            get => _subtotal;
            set => SetProperty(ref _subtotal, value);
        }

        public decimal DeliveryCost
        {
            get => _deliveryCost;
            set => SetProperty(ref _deliveryCost, value);
        }

        public decimal DiscountAmount
        {
            get => _discountAmount;
            set => SetProperty(ref _discountAmount, value);
        }

        public decimal TotalCost
        {
            get => _totalCost;
            set => SetProperty(ref _totalCost, value);
        }

        public string SpecialInstructions
        {
            get => _specialInstructions;
            set => SetProperty(ref _specialInstructions, value);
        }

        public string DeliveryAddress
        {
            get => _deliveryAddress;
            set => SetProperty(ref _deliveryAddress, value);
        }

        public bool HasItems => CartItems.Count > 0;


        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand ClearCartCommand { get; }


        public CartViewModel(IOrderService orderService, IMenuService menuService, IUserService userService, ICartPersistenceService cartPersistenceService, MainWindowViewModel mainWindowViewModel)
        {
            _orderService = orderService;
            _menuService = menuService;
            _userService = userService;
            _cartPersistenceService = cartPersistenceService;
            _mainWindowViewModel = mainWindowViewModel;

            CartItems = new ObservableCollection<CartItemViewModel>();

            IncreaseQuantityCommand = new RelayCommand(ExecuteIncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(ExecuteDecreaseQuantity);
            RemoveItemCommand = new RelayCommand(ExecuteRemoveItem);
            CheckoutCommand = new RelayCommand(ExecuteCheckout);
            ClearCartCommand = new RelayCommand(ExecuteClearCart);

            LoadCart();

            var currentUser = _userService.GetCurrentUser();
            if (currentUser != null)
            {
                DeliveryAddress = currentUser.DeliveryAddress;
            }

            CalculateTotals();

        }

        private void LoadCart()
        {
            try
            {
                var currentUser = _userService.GetCurrentUser();
                if (currentUser != null)
                {
                    var savedItems = _cartPersistenceService.LoadCart(currentUser.Id);
                    if (savedItems != null && savedItems.Count > 0)
                    {
                        foreach (var item in savedItems)
                        {
                            CartItems.Add(item);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load cart. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void SaveCart()
        {
            try
            {
                var currentUser = _userService.GetCurrentUser();
                if (currentUser != null)
                {
                    _cartPersistenceService.SaveCart(CartItems.ToList(), currentUser.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save cart. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private void CalculateTotals()
        {
            Subtotal = CartItems.Sum(item => item.TotalPrice);

            DeliveryCost = Subtotal >= 75 ? 0 : 15;

            DiscountAmount = Subtotal >= 100 ? Subtotal * 0.05m : 0;

            TotalCost = Subtotal + DeliveryCost - DiscountAmount;

            OnPropertyChanged(nameof(HasItems));

            SaveCart();
        }

        private void ExecuteIncreaseQuantity(object parameter)
        {
            if (parameter is int itemId)
            {
                var item = CartItems.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    item.Quantity++;
                    CalculateTotals();
                }
            }
        }

        private void ExecuteDecreaseQuantity(object parameter)
        {
            if (parameter is int itemId)
            {
                var item = CartItems.FirstOrDefault(i => i.Id == itemId);
                if (item != null && item.Quantity > 1)
                {
                    item.Quantity--;
                    CalculateTotals();
                }
                else if (item != null && item.Quantity == 1)
                {
                    
                    ExecuteRemoveItem(itemId);
                }
            }

        }

        private void ExecuteRemoveItem(object parameter)
        {
            if (parameter is int itemId)
            {
                var item = CartItems.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    CartItems.Remove(item);
                    CalculateTotals();
                }
            }
        }

        private void ExecuteClearCart(object parameter)
        {
            if (CartItems.Count > 0)
            {
                var result = MessageBox.Show("Are you sure you want to clear the cart?",
                                           "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    CartItems.Clear();
                    CalculateTotals();

                    var currentUser = _userService.GetCurrentUser();
                    if (currentUser != null)
                    {
                        _cartPersistenceService.ClearCart(currentUser.Id);
                    }
                }
            }
        }

        private void ExecuteCheckout(object parameter)
        {
            if (!HasItems)
            {
                MessageBox.Show("The cart is empty. Please add items before placing the order.",
                              "Empty Cart", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(DeliveryAddress))
            {
                MessageBox.Show("Please enter your delivery address",
                              "Delivery address missing", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var orderItems = CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.IsProduct ? (int?)ci.Id : null,
                    MenuId = ci.IsProduct ? null : (int?)ci.Id,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                    TotalPrice = ci.TotalPrice
                }).ToList();

                var currentUser = _userService.GetCurrentUser();
                if (currentUser == null)
                {
                    MessageBox.Show("Trebuie să fiți autentificat pentru a plasa o comandă.",
                                  "Autentificare necesară", MessageBoxButton.OK, MessageBoxImage.Warning);
                    _mainWindowViewModel.ShowLoginCommand.Execute(null);
                    return;
                }

                var order = _orderService.CreateOrder(currentUser.Id, orderItems);
                if (order != null)
                {

                    if (currentUser.DeliveryAddress != DeliveryAddress)
                    {
                        currentUser.DeliveryAddress = DeliveryAddress;
                        _userService.UpdateUser(currentUser);
                    }


                    CartItems.Clear();
                    SpecialInstructions = string.Empty;
                    CalculateTotals();

                    _cartPersistenceService.ClearCart(currentUser.Id);

                    MessageBox.Show($"The order has been placed successfully!\nOrder code: {order.OrderCode}",
                                  "Order Placed", MessageBoxButton.OK, MessageBoxImage.Information);

                    _mainWindowViewModel.ShowOrdersCommand.Execute(null);
                }
                else
                {
                    MessageBox.Show("An error occurred while placing the order. Please try again.",
                                "Order Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error placing the order: {ex.Message}",
                              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddToCart(Product product, int quantity = 1)
        {
            if (product == null)
            {
                return;
            }

            var existingItem = CartItems.FirstOrDefault(i => i.IsProduct && i.Id == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                CartItems.Add(new CartItemViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    PortionInfo = $"{product.PortionQuantity} {product.MeasurementUnit}",
                    UnitPrice = product.Price,
                    Quantity = quantity,
                    IsProduct = true
                });
            }

            CalculateTotals();

            MessageBox.Show($"The product '{product.Name}' has been added to the cart.",
                        "Added to Cart", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public void AddToCart(Menu menu, int quantity = 1)
        {
            if (menu == null)
                return;

            var existingItem = CartItems.FirstOrDefault(i => !i.IsProduct && i.Id == menu.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                CartItems.Add(new CartItemViewModel
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    PortionInfo = "porție",
                    UnitPrice = menu.CalculatePrice(),
                    Quantity = quantity,
                    IsProduct = false
                });
            }

            CalculateTotals();
            MessageBox.Show($"The menu '{menu.Name}' has been added to the cart.",
                        "Added to Cart", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
