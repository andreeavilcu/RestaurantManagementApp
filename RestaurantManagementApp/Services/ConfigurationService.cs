using System;
using System.Configuration;

namespace RestaurantManagementApp.Services
{
    public interface IConfigurationService
    {
        decimal GetMenuDiscountPercentage();
        decimal GetOrderDiscountThreshold();
        decimal GetOrderDiscountPercentage();
        int GetLoyaltyOrderCount();
        int GetLoyaltyTimePeriod();
        decimal GetLoyaltyDiscountPercentage();
        decimal GetFreeDeliveryThreshold();
        decimal GetDeliveryCost();
        decimal GetLowStockThreshold();
    }

    public class ConfigurationService : IConfigurationService
    {
        public decimal GetMenuDiscountPercentage()
        {
            return GetDecimalSetting("MenuDiscountPercentage", 10);
        }

        public decimal GetOrderDiscountThreshold()
        {
            return GetDecimalSetting("OrderDiscountThreshold", 100);
        }

        public decimal GetOrderDiscountPercentage()
        {
            return GetDecimalSetting("OrderDiscountPercentage", 5);
        }

        public int GetLoyaltyOrderCount()
        {
            return GetIntSetting("LoyaltyOrderCount", 5);
        }

        public int GetLoyaltyTimePeriod()
        {
            return GetIntSetting("LoyaltyTimePeriod", 30);
        }

        public decimal GetLoyaltyDiscountPercentage()
        {
            return GetDecimalSetting("LoyaltyDiscountPercentage", 10);
        }

        public decimal GetFreeDeliveryThreshold()
        {
            return GetDecimalSetting("FreeDeliveryThreshold", 75);
        }

        public decimal GetDeliveryCost()
        {
            return GetDecimalSetting("DeliveryCost", 15);
        }

        public decimal GetLowStockThreshold()
        {
            return GetDecimalSetting("LowStockThreshold", 1000);
        }

        private decimal GetDecimalSetting(string key, decimal defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (decimal.TryParse(value, out decimal result))
                return result;
            return defaultValue;
        }

        private int GetIntSetting(string key, int defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (int.TryParse(value, out int result))
                return result;
            return defaultValue;
        }
    }
}
