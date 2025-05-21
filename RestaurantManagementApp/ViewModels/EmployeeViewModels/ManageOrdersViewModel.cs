using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;


namespace RestaurantManagementApp.ViewModels.EmployeeViewModels
{
    public class ManageOrdersViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        private ObservableCollection<Order> _orders;
        private int _filterIndex;
        private string _searchText;
        private Order _selectedOrder;
        private bool _isUpdatingStatus;

        private ObservableCollection<OrderStatus> _availableStatuses;

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        public int FilterIndex
        {
            get => _filterIndex;
            set
            {
                if (SetProperty(ref _filterIndex, value))
                {
                    RefreshOrdersCommand.Execute(null);
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        public bool IsUpdatingStatus
        {
            get => _isUpdatingStatus;
            set => SetProperty(ref _isUpdatingStatus, value);
        }

        public ObservableCollection<OrderStatus> AvailableStatuses
        {
            get => _availableStatuses;
            set => SetProperty(ref _availableStatuses, value);
        }

        public bool HasOrders => Orders.Count > 0;

        public ICommand RefreshOrdersCommand { get; }
        public ICommand ChangeStatusCommand { get; }
        public ICommand UpdateStatusCommand { get; }
        public ICommand PrintReceiptCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand ViewOrderDetailsCommand { get; }


        public ManageOrdersViewModel(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;

            Orders = new ObservableCollection<Order>();
            InitializeAvailableStatuses();

            RefreshOrdersCommand = new RelayCommand(ExecuteRefreshOrders);
            ChangeStatusCommand = new RelayCommand(ExecuteChangeStatus);
            UpdateStatusCommand = new RelayCommand(ExecuteUpdateStatus);
            PrintReceiptCommand = new RelayCommand(ExecutePrintReceipt);
            SearchCommand = new RelayCommand(ExecuteSearch);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearch);
            ViewOrderDetailsCommand = new RelayCommand(ExecuteViewOrderDetails);

            LoadOrders();

        }

        private void InitializeAvailableStatuses()
        {
            AvailableStatuses = new ObservableCollection<OrderStatus>
            {
                OrderStatus.Registered,
                OrderStatus.Preparing,
                OrderStatus.Shipped,
                OrderStatus.Delivered,
                OrderStatus.Canceled
            };
        }

        private void LoadOrders()
        {
            try
            {
                Orders.Clear();

                IEnumerable<Order> ordersToShow;
                switch (FilterIndex)
                {
                    case 1:
                        ordersToShow = _orderService.GetActiveOrders();
                        break;
                    case 2:
                        ordersToShow = _orderService.GetOrdersByStatus(OrderStatus.Registered);
                        break;

                    case 3:
                        ordersToShow = _orderService.GetOrdersByStatus(OrderStatus.Preparing);
                        break;

                    case 4:
                        ordersToShow = _orderService.GetOrdersByStatus(OrderStatus.Delivered);
                        break;

                    case 5:
                        ordersToShow = _orderService.GetOrdersByStatus(OrderStatus.Shipped);
                        break;
                    case 6:
                        ordersToShow = _orderService.GetOrdersByStatus(OrderStatus.Canceled);
                        break;
                    default:
                        ordersToShow = _orderService.GetAllOrders();
                        break;

                }

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    ordersToShow = FilterOrdersBySearchText(ordersToShow, SearchText);
                }

                foreach (var order in ordersToShow)
                {
                    Orders.Add(order);
                }

                OnPropertyChanged(nameof(HasOrders));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private IEnumerable<Order> FilterOrdersBySearchText(IEnumerable<Order> orders, string searchText)
        {
            return orders.Where(o =>
                o.OrderCode.Contains(searchText) ||
                o.User.FirstName.Contains(searchText) ||
                o.User.LastName.Contains(searchText) ||
                o.User.PhoneNumber.Contains(searchText) ||
                o.User.Email.Contains(searchText) ||
                o.User.DeliveryAddress.Contains(searchText));

        }

        private void ExecuteRefreshOrders(object parameter)
        {
            LoadOrders();
        }

        private void ExecuteChangeStatus(object parameter)
        {
            if (parameter is int orderId)
            {

                var order = Orders.FirstOrDefault(o => o.Id == orderId);
                if (order != null)
                {
                    if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Canceled)
                    {
                        MessageBox.Show("Cannot change the status of delivered or canceled orders.",
                            "Status Change Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    IsUpdatingStatus = true;
                    SelectedOrder = order;
                }
            }
        }

        private void ExecuteUpdateStatus(object parameter)
        {
            if (parameter is int orderId)
            {
                var order = Orders.FirstOrDefault(o => o.Id == orderId);
                if (order != null)
                {
                    try
                    {
                        bool success = _orderService.UpdateOrderStatus(orderId, order.Status);
                        if (success)
                        {
                            MessageBox.Show($"Order status updated to {order.Status}.",
                                "Status Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadOrders();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update order status.",
                                "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating order status: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    IsUpdatingStatus = false;
                }
            }

        }

        private void ExecutePrintReceipt(object parameter)
        {
            if (parameter is int orderId)
            {
                try
                {
                   
                    var order = _orderService.GetOrderById(orderId);
                   

                    if (order == null)
                    {
                        MessageBox.Show("Order not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string receiptContent = GenerateReceiptContent(order);

                    var receiptWindow = new Window
                    {
                        Title = $"Receipt - Order #{order.OrderCode}",
                        Width = 400,
                        Height = 600,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    var textBox = new System.Windows.Controls.TextBox
                    {
                        Text = receiptContent,
                        IsReadOnly = true,
                        VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
                        HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Disabled,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(10)
                    };

                    receiptWindow.Content = textBox;
                    receiptWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error printing receipt: {ex.Message}",
                                  "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string GenerateReceiptContent(Order order)
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("RESTAURANT MANAGEMENT APP");
            sb.AppendLine("------------------------");
            sb.AppendLine();
            sb.AppendLine($"ORDER: #{order.OrderCode ?? "N/A"}");
            sb.AppendLine($"DATE: {order.OrderDate.ToString("g")}");
            sb.AppendLine($"STATUS: {order.Status}");
            sb.AppendLine();

            
            if (order.User != null)
            {
                sb.AppendLine("CUSTOMER INFORMATION:");
                sb.AppendLine($"Name: {order.User.FirstName} {order.User.LastName}");
                sb.AppendLine($"Phone: {order.User.PhoneNumber ?? "N/A"}");
                sb.AppendLine($"Delivery Address: {order.User.DeliveryAddress ?? "N/A"}");
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine("CUSTOMER INFORMATION: Not available");
                sb.AppendLine();
            }

            sb.AppendLine("ORDERED ITEMS:");
            sb.AppendLine("------------------------");

            if (order.OrderItems != null && order.OrderItems.Any())
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.ProductId.HasValue)
                    {
                        string productName = item.Product?.Name ?? "Unknown Product";
                        sb.AppendLine($"{item.Quantity} x {productName} - {item.TotalPrice:C}");
                    }
                    else if (item.MenuId.HasValue)
                    {
                        string menuName = item.Menu?.Name ?? "Unknown Menu";
                        sb.AppendLine($"{item.Quantity} x {menuName} - {item.TotalPrice:C}");
                    }
                }
            }
            else
            {
                sb.AppendLine("No items in this order");
            }

            sb.AppendLine("------------------------");
            sb.AppendLine($"Food Cost: {order.FoodCost:C}");
            sb.AppendLine($"Delivery Cost: {order.DeliveryCost:C}");
            sb.AppendLine($"Discount: -{order.DiscountAmount:C}");
            sb.AppendLine($"TOTAL: {order.TotalCost:C}");
            sb.AppendLine();

            sb.AppendLine("Thank you for your order!");
            sb.AppendLine("------------------------");

            return sb.ToString();
        }

        private void ExecuteSearch(object parameter)
        {
            LoadOrders();
        }

        private void ExecuteClearSearch(object parameter)
        {
            SearchText = string.Empty;
            LoadOrders();
        }

        private void ExecuteViewOrderDetails(object parameter)
        {
            if (parameter is int orderId)
            {
                var order = Orders.FirstOrDefault(o => o.Id == orderId);
                if (order != null)
                {
                    SelectedOrder = order;
                    
                }
            }
        }
    }
}
