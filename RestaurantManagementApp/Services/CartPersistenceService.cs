using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using RestaurantManagementApp.ViewModels;
using RestaurantManagementApp.Models;
using System.Text.Json;


namespace RestaurantManagementApp.Services
{
    public interface ICartPersistenceService
    {
        void SaveCart(List<CartItemViewModel> cartItems, int userId);
        List<CartItemViewModel> LoadCart(int userId);
        void ClearCart(int userId);

    }

    [Serializable]
    public class SerializableCartItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PortionInfo { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public bool IsProduct { get; set; }
    }


    public class CartPersistence : ICartPersistenceService
    {

        private readonly string _cartDirectoryPath;

        public CartPersistence()
        {
            _cartDirectoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RestaurantManagementApp",
                "Carts");

            if (!Directory.Exists(_cartDirectoryPath))
            {
                Directory.CreateDirectory(_cartDirectoryPath);
            }
        }
        public void ClearCart(int userId)
        {
            try
            {
                string filePath = GetCartFilePath(userId);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la ștergerea coșului: {ex.Message}");
            }
        }

        public List<CartItemViewModel> LoadCart(int userId)
        {
            try
            {
                string filePath = GetCartFilePath(userId);
                if (!File.Exists(filePath))
                {
                    return new List<CartItemViewModel>();
                }

                string jsonData = File.ReadAllText(filePath);
                var serializableItems = JsonSerializer.Deserialize<List<SerializableCartItem>>(jsonData);

                
                return serializableItems.Select(item => new CartItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    PortionInfo = item.PortionInfo,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    IsProduct = item.IsProduct
                }).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cart: {ex.Message}");
                return new List<CartItemViewModel>();
            }
        }

        public void SaveCart(List<CartItemViewModel> cartItems, int userId)
        {
            try
            {
                
                var serializableItems = cartItems.Select(item => new SerializableCartItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    PortionInfo = item.PortionInfo,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    IsProduct = item.IsProduct
                }).ToList();

                
                string filePath = GetCartFilePath(userId);
                string jsonData = JsonSerializer.Serialize(serializableItems);
                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving cart: {ex.Message}");
            }
        }

        private string GetCartFilePath(int userId)
        {
            return Path.Combine(_cartDirectoryPath, $"cart_{userId}.json");
        }

    } 
}

