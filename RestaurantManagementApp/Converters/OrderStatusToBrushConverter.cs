using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using RestaurantManagementApp.Models;

namespace RestaurantManagementApp.Converters
{
    public class OrderStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                switch (status)
                {
                    case OrderStatus.Registered:
                        return new SolidColorBrush(Color.FromRgb(52, 152, 219)); 
                    case OrderStatus.Preparing:
                        return new SolidColorBrush(Color.FromRgb(243, 156, 18)); 
                    case OrderStatus.Shipped:
                        return new SolidColorBrush(Color.FromRgb(155, 89, 182)); 
                    case OrderStatus.Delivered:
                        return new SolidColorBrush(Color.FromRgb(39, 174, 96));  
                    case OrderStatus.Canceled:
                        return new SolidColorBrush(Color.FromRgb(231, 76, 60));  
                    default:
                        return new SolidColorBrush(Color.FromRgb(127, 140, 141)); 
                }
            }
            return new SolidColorBrush(Color.FromRgb(127, 140, 141));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
