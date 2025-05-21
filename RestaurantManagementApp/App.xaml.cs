using System;
using System.Linq;
using System.Windows;
using RestaurantManagementApp.DataAccess;
using RestaurantManagementApp.Services;
using RestaurantManagementApp.ViewModels;
using RestaurantManagementApp.Views;

namespace RestaurantManagementApp
{
    public partial class App
    {
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                using (var context = new DatabaseContext())
                {
                    System.Diagnostics.Debug.WriteLine("Testare conexiune la baza de date...");
                    var test = context.Categories.ToList();
                    System.Diagnostics.Debug.WriteLine($"Conexiune reușită! Am găsit {test.Count} categorii.");

                   
                    var userCount = context.Users.Count();
                    System.Diagnostics.Debug.WriteLine($"Număr utilizatori în baza de date: {userCount}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EROARE BAZĂ DE DATE: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

               
                var fullMessage = ex.Message;
                var innerMessage = ex.InnerException?.Message;

                MessageBox.Show($"Error: {fullMessage}\n\nInner: {innerMessage}", "Database Error");
            }

            var dbContext = new DatabaseContext();
            var unitOfWork = new UnitOfWork(dbContext);
            var passwordService = new PasswordService();
            var configService = new ConfigurationService();

            var userService = new UserService(unitOfWork, passwordService);
            var menuService = new MenuService(unitOfWork, configService);
            var inventoryService = new InventoryService(unitOfWork, configService);
            var orderService = new OrderService(unitOfWork, inventoryService, configService);
            var cartPersistenceService = new CartPersistence();

            //userService.UpdateAllUserPasswords();
            //userService.ResetPasswordForInterfaceUsers();

            var mainViewModel = new MainWindowViewModel(userService, menuService, orderService);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Show();

        }
    }
}