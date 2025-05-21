#  Restaurant Management App

**Restaurant Management App** is a modern desktop application designed to manage restaurant operations. It is built with .NET Framework 4.7.2, using WPF and Entity Framework 6. The application follows the **MVVM** architectural pattern along with the **Repository** and **Unit of Work** patterns.

---

##  Key Features

- **Category Management**  
  Add, edit, and delete product and menu categories.

- **Product Management**  
  Full CRUD operations for products, allergen association, image management, stock control, filtering by allergens and categories.

- **Menu Management**  
  Full CRUD for menus, product association, and menu discounts.

- **Order Management**  
  View, filter, update order status, delivery details, and print receipts.

- **User Management**  
  Authentication, roles (employee/customer), and personal data management.

- **Allergen Management**  
  CRUD operations for allergens and association with products.

- **Stock Management**  
  Check and update stock when placing orders.

- **Discounts & Delivery**  
  Calculate discounts based on thresholds and loyalty, manage delivery costs.

- **Cart Persistence**  
  Save and restore shopping cart state.

- **Transactions**  
  Atomic operations for order creation.

---

##  Technologies Used

- .NET Framework 4.7.2  
- WPF (Windows Presentation Foundation)  
- Entity Framework 6 (Code First)  
- SQL Server  
- MVVM Pattern  
- Repository Pattern  
- Unit of Work Pattern  

---

##  Project Structure

- **Models**  
  Core entities (Product, Category, Menu, Order, User, Allergen, etc.)

- **DataAccess**  
  Database context, repositories, Unit of Work, and data initializer.

- **Services**  
  Business logic for products, menus, orders, users, and inventory.

- **ViewModels**  
  ViewModels for each view, bindings, and UI commands.

- **Views**  
  User interface (WPF windows and controls).

- **Converters**  
  Value converters for advanced XAML bindings.

---
