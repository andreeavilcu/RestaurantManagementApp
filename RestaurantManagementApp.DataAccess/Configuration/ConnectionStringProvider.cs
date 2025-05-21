using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Forms;

namespace RestaurantManagementApp.DataAccess.Configuration
{
    public static class ConnectionStringProvider
    {
        public static string GetConnectionString()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["RestaurantDb"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException("Connection string 'RestaurantDb' not found in configuration.");
            }

            // Testează conexiunea la baza de date
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // Încearcă să deschidă conexiunea
                    connection.Close(); // Închide conexiunea
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Database connection failed: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw; // Aruncă excepția mai departe pentru a opri aplicația
            }

            return connectionString;
        }
    }
}