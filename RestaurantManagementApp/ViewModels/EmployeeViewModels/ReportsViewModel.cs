using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RestaurantManagementApp.Models;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels.Base;

namespace RestaurantManagementApp.ViewModels.EmployeeViewModels
{

    public class SalesReportItem
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AverageOrder { get; set; }
    }

    public class ProductPopularityItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public int OrderCount { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public bool IsMenu { get; set; }
    }

    public class LowStockItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal PortionQuantity { get; set; }
        public string MeasurementUnit { get; set; }
        public decimal EstimatedPortions { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class ReportsViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly IMenuService _menuService;
        private readonly IInventoryService _inventoryService;


        private DateTime _startDate;
        private DateTime _endDate;
        private int _reportType;
        private string _reportTitle;
        private string _errorMessage;
        private bool _hasError;
        private bool _isGenerating;
        private bool _hasReport;

        private ObservableCollection<SalesReportItem> _salesData;
        private decimal _totalSales;
        private int _totalOrders;
        private decimal _averageOrderValue;

        private ObservableCollection<ProductPopularityItem> _popularProducts;

        private ObservableCollection<LowStockItem> _lowStockProducts;


        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public int ReportType
        {
            get => _reportType;
            set => SetProperty(ref _reportType, value);
        }

        public string ReportTitle
        {
            get => _reportTitle;
            set => SetProperty(ref _reportTitle, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public bool IsGenerating
        {
            get => _isGenerating;
            set => SetProperty(ref _isGenerating, value);
        }

        public bool HasReport
        {
            get => _hasReport;
            set => SetProperty(ref _hasReport, value);
        }

        public ObservableCollection<SalesReportItem> SalesData
        {
            get => _salesData;
            set => SetProperty(ref _salesData, value);
        }

        public decimal TotalSales
        {
            get => _totalSales;
            set => SetProperty(ref _totalSales, value);
        }

        public int TotalOrders
        {
            get => _totalOrders;
            set => SetProperty(ref _totalOrders, value);
        }

        public decimal AverageOrderValue
        {
            get => _averageOrderValue;
            set => SetProperty(ref _averageOrderValue, value);
        }

        public ObservableCollection<ProductPopularityItem> PopularProducts
        {
            get => _popularProducts;
            set => SetProperty(ref _popularProducts, value);
        }

        public ObservableCollection<LowStockItem> LowStockProducts
        {
            get => _lowStockProducts;
            set => SetProperty(ref _lowStockProducts, value);
        }


        public ICommand GenerateReportCommand { get; }
        public ICommand ExportReportCommand { get; }
        public ICommand ClearReportCommand { get; }

        public ReportsViewModel(IOrderService orderService, IMenuService menuService, IInventoryService inventoryService)
        {
            _orderService = orderService;
            _menuService = menuService;
            _inventoryService = inventoryService;

            StartDate = DateTime.Now.AddDays(-30);
            EndDate = DateTime.Now;
            ReportType = 0;

            SalesData = new ObservableCollection<SalesReportItem>();
            PopularProducts = new ObservableCollection<ProductPopularityItem>();
            LowStockProducts = new ObservableCollection<LowStockItem>();

            GenerateReportCommand = new RelayCommand(ExecuteGenerateReport);
            
            ClearReportCommand = new RelayCommand(ExecuteClearReport);
        }


        private void ExecuteGenerateReport(object parameter)
        {
            HasError = false;
            IsGenerating = true;
            HasReport = false;

            try
            {
                if (EndDate < StartDate)
                {
                    HasError = true;
                    ErrorMessage = "End date cannot be earlier than start date.";
                    IsGenerating = false;
                    return;
                }

                switch (ReportType)
                {
                    case 0:
                        GenerateSalesReport();
                        break;
                    case 1:
                        GenerateProductPopularityReport();
                        break;
                    case 2:
                        GenerateLowStockReport();
                        break;
                }

                HasReport = true;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error generating report: {ex.Message}";
            }
            finally
            {
                IsGenerating = false;
            }
        }

        private void GenerateSalesReport()
        { 
            SalesData.Clear();

            var orders = _orderService.GetAllOrders()
                .Where(o => o.OrderDate >= StartDate && o.OrderDate <= EndDate)
                .ToList();

            var groupedByDate = orders
                .GroupBy(o => o.OrderDate.Date)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var group in groupedByDate)
            {
                SalesData.Add(new SalesReportItem
                {
                    Date = group.Key,
                    OrderCount = group.Count(),
                    TotalSales = group.Sum(o => o.TotalCost),
                    AverageOrder = group.Count() > 0 ? group.Sum(o => o.TotalCost) / group.Count() : 0
                });
            }

            TotalSales = orders.Sum(o => o.TotalCost);
            TotalOrders = orders.Count;
            AverageOrderValue = TotalOrders > 0 ? TotalSales / TotalOrders : 0;

            ReportTitle = $"Sales Report: {StartDate.ToShortDateString()} - {EndDate.ToShortDateString()}";
        }

        private void GenerateProductPopularityReport()
        {
            PopularProducts.Clear();

            
            System.Diagnostics.Debug.WriteLine($"Generating product popularity report from {StartDate} to {EndDate}");

            
            var orders = _orderService.GetAllOrders()
                .Where(o => o.OrderDate >= StartDate && o.OrderDate <= EndDate && o.Status != OrderStatus.Canceled)
                .ToList();

            System.Diagnostics.Debug.WriteLine($"Found {orders.Count} orders in the selected period");

            if (orders.Count == 0)
            {
                
                HasError = true;
                ErrorMessage = "No orders found in the selected date range.";
                return;
            }

            var orderItems = orders.SelectMany(o => o.OrderItems).ToList();
            System.Diagnostics.Debug.WriteLine($"Found {orderItems.Count} order items in total");

            
            var productGroups = orderItems
                .Where(oi => oi.ProductId.HasValue)
                .GroupBy(oi => oi.ProductId.Value)
                .Select(g => new
                {
                    ProductId = g.Key,
                    OrderCount = g.Count(),
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .ToList();

            System.Diagnostics.Debug.WriteLine($"Found {productGroups.Count} product groups");

            foreach (var group in productGroups)
            {
                var product = _menuService.GetProductById(group.ProductId);
                if (product != null)
                {
                    PopularProducts.Add(new ProductPopularityItem
                    {
                        ProductId = group.ProductId,
                        ProductName = product.Name,
                        CategoryName = product.Category?.Name ?? "Unknown",
                        OrderCount = group.OrderCount,
                        TotalQuantity = (int)group.TotalQuantity,
                        TotalRevenue = group.TotalRevenue,
                        IsMenu = false
                    });
                }
            }

            
            var menuGroups = orderItems
                .Where(oi => oi.MenuId.HasValue)
                .GroupBy(oi => oi.MenuId.Value)
                .Select(g => new
                {
                    MenuId = g.Key,
                    OrderCount = g.Count(),
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .ToList();

            System.Diagnostics.Debug.WriteLine($"Found {menuGroups.Count} menu groups");

            foreach (var group in menuGroups)
            {
                var menu = _menuService.GetMenuById(group.MenuId);
                if (menu != null)
                {
                    PopularProducts.Add(new ProductPopularityItem
                    {
                        ProductId = group.MenuId,
                        ProductName = menu.Name + " (Menu)",
                        CategoryName = menu.Category?.Name ?? "Unknown",
                        OrderCount = group.OrderCount,
                        TotalQuantity = (int)group.TotalQuantity,
                        TotalRevenue = group.TotalRevenue,
                        IsMenu = true
                    });
                }
            }

            System.Diagnostics.Debug.WriteLine($"Total items in report: {PopularProducts.Count}");
            ReportTitle = $"Product Popularity Report: {StartDate.ToShortDateString()} - {EndDate.ToShortDateString()}";
        }

        private void GenerateLowStockReport()
        {
            LowStockProducts.Clear();
            System.Diagnostics.Debug.WriteLine("Generating low stock report");

            var products = _inventoryService.GetLowStockProducts();
            System.Diagnostics.Debug.WriteLine($"Found {products.Count()} products with low stock");

            if (!products.Any())
            {
                
                HasError = true;
                ErrorMessage = "No products with low stock were found.";
                return;
            }

            foreach (var product in products)
            {
                System.Diagnostics.Debug.WriteLine($"Processing low stock product: {product.Name}, Stock: {product.TotalQuantity}");

                LowStockProducts.Add(new LowStockItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    CategoryName = product.Category?.Name ?? "Unknown",
                    TotalQuantity = product.TotalQuantity,
                    PortionQuantity = product.PortionQuantity,
                    MeasurementUnit = product.MeasurementUnit,
                    EstimatedPortions = product.TotalQuantity / (product.PortionQuantity > 0 ? product.PortionQuantity : 1),
                    IsAvailable = product.IsAvailable
                });
            }

            System.Diagnostics.Debug.WriteLine($"Added {LowStockProducts.Count} items to low stock report");
            ReportTitle = "Low Stock Products Report";
        }

        

        private void ExecuteClearReport(object parameter)
        {
            SalesData.Clear();
            PopularProducts.Clear();
            LowStockProducts.Clear();
            HasReport = false;
            ReportTitle = string.Empty;
        }
    }
}
