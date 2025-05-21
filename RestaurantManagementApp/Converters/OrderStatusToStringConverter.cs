using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using RestaurantManagementApp.Models;

namespace RestaurantManagementApp.Converters
{
    /// <summary>
    /// Convertește un OrderStatus într-un string pentru afișarea stării comenzii
    /// </summary>
    public class OrderStatusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                switch (status)
                {
                    case OrderStatus.Registered:
                        return "Registered";
                    case OrderStatus.Preparing:
                        return "Preparing";
                    case OrderStatus.Shipped:
                        return "Shipped";
                    case OrderStatus.Delivered:
                        return "Delivered";
                    case OrderStatus.Canceled:
                        return "Canceled";
                    default:
                        return "Unknown";
                }
            }

            return "Unknown";
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
