using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;

namespace RestaurantManagementApp.ViewModels
{
    public class OrdersViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly MainWindowViewModel _mainWindowViewModel;

        private ObservableCollection<Order> _orders;
        private int _selectedFilterIndex;
        private Order _selectedOrder;

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        public int SelectedFilterIndex
        {
            get => _selectedFilterIndex;
            set
            {
                if (SetProperty(ref _selectedFilterIndex, value))
                {

                    RefreshOrdersCommand.Execute(null);
                }
            }
        }

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        public bool HasOrders => Orders.Count > 0;

        public ICommand RefreshOrdersCommand { get; }
        public ICommand ViewOrderDetailsCommand { get; }
        public ICommand CancelOrderCommand { get; }



        public OrdersViewModel(IOrderService orderService, IUserService userService, MainWindowViewModel mainWindowViewModel)
        {
            _orderService = orderService;
            _userService = userService;
            _mainWindowViewModel = mainWindowViewModel;

            Orders = new ObservableCollection<Order>();

            RefreshOrdersCommand = new RelayCommand(ExecuteRefreshOrders);
            ViewOrderDetailsCommand = new RelayCommand(ExecuteViewOrderDetails);
            CancelOrderCommand = new RelayCommand(ExecuteCancelOrder);

            ExecuteRefreshOrders(null);
        }

        private void ExecuteRefreshOrders(object parameter)
        {
            Orders.Clear();

            var currentUser = _userService.GetCurrentUser();
            if (currentUser == null)
            {
                return;
            }

            IEnumerable<Order> ordersToShow;

            switch (SelectedFilterIndex)
            {
                case 1:
                    ordersToShow = _orderService.GetOrdersByUser(currentUser.Id)
                        .Where(o => o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Canceled);
                    break;
                case 2:
                    ordersToShow = _orderService.GetOrdersByUser(currentUser.Id)
                        .Where(o => o.Status == OrderStatus.Delivered);
                    break;
                case 3:
                    ordersToShow = _orderService.GetOrdersByUser(currentUser.Id)
                        .Where(o => o.Status == OrderStatus.Canceled);
                    break;
                default:
                    ordersToShow = _orderService.GetOrdersByUser(currentUser.Id);
                    break;
            }

            ordersToShow = ordersToShow.OrderByDescending(o => o.OrderDate);

            foreach (var order in ordersToShow)
            {
                Orders.Add(order);
            }

            OnPropertyChanged(nameof(HasOrders));
        }

        private void ExecuteViewOrderDetails(object parameter)
        {
            if (parameter is int orderId)
            {
                var order = _orderService.GetOrderById(orderId);
                if (order != null)
                {
                    
                    MessageBox.Show($"Details for order #{order.OrderCode}:\n" +
                                    $"Status: {order.Status}\n" +
                                    $"Data: {order.OrderDate}\n" +
                                    $"Total: {order.TotalCost:C2}",
                                    "Order Details", MessageBoxButton.OK);
                }
            }
        }


        private void ExecuteCancelOrder(object parameter)
        {
            if (parameter is int orderId)
            {
                var order = _orderService.GetOrderById(orderId);
                if (order != null && order.CanCancel())
                {
                    var result = MessageBox.Show($"Are you sure you want to cancel order #{order.OrderCode}?",
                                               "Cancellation Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        bool success = _orderService.CancelOrder(orderId);
                        if (success)
                        {
                            MessageBox.Show("The order has been successfully canceled.",
                                          "Order Canceled", MessageBoxButton.OK, MessageBoxImage.Information);

                            
                            ExecuteRefreshOrders(null);
                        }
                        else
                        {
                            MessageBox.Show("The order could not be canceled. It may already be in delivery or delivered.",
                                          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("This order cannot be canceled. It may already be in delivery or delivered.",
                                  "Cancellation Not Possible", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }



    }

}
