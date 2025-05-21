using RestaurantManagementApp.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System;

namespace RestaurantManagementApp.DataAccess.Configuration
{
    public class DatabaseInitializer : CreateDatabaseIfNotExists<DatabaseContext>
    {

       
        protected override void Seed(DatabaseContext context)
      {
            //Seed categories
            var categories = new List<Category>
            {
                new Category{ Name = "Mic dejun", Description = "Preparate pentru micul dejun"},
                new Category{ Name ="Aperitive" , Description ="Aperitive delicioase"},
                new Category{ Name ="Supe/Ciorbe" , Description ="Supe și ciorbe tradiționale"},
                new Category{ Name ="Fel principal" , Description ="Feluri principale"},
                new Category{ Name ="Deserturi" , Description ="Deserturi dulci"},
                new Category{ Name ="Băuturi" , Description ="Băuturi răcoritoare și calde"}
            };

            categories.ForEach(c => context.Categories.Add(c));
            context.SaveChanges();

            var allergens = new List<Allergen>
            {
                new Allergen{ Name = "Gluten", Description = "Conține gluten"},
                new Allergen{ Name = "Lactate", Description = "Conține lactate"},
                new Allergen{ Name = "Nuci", Description = "Conține nuci"},
                new Allergen { Name = "Pește", Description = "Conține pește" },
                new Allergen{ Name = "Telina", Description = "Conține telina"}
            };

            allergens.ForEach(a => context.Allergens.Add(a));
            context.SaveChanges();

            var products = new List<Product>
            {
                new Product
                {
                    Name ="Supă cremă de ciuperci",
                    Description ="Supă cremă de ciuperci proaspete",
                    Price = 15.99M,
                    PortionQuantity =  300,
                    MeasurementUnit = "g",
                    TotalQuantity = 3000,
                    CategoryId = 3,
                    IsAvailable = true
                },

                new Product
                {
                    Name ="Pastrav la grătar",
                    Description ="Păstrăv proaspăt la grătar cu lămâie",
                    Price = 35.99M,
                    PortionQuantity = 300,
                    MeasurementUnit = "g",
                    TotalQuantity = 5000,
                    CategoryId = 4,
                    IsAvailable = true
                },

                new Product
                {
                    Name ="Cartofi prajiti",
                    Description ="Cartofi prajiti proaspeti",
                    Price = 12.99M,
                    PortionQuantity = 300,
                    MeasurementUnit = "g",
                    TotalQuantity = 10000,
                    CategoryId = 2,
                    IsAvailable = true
                },

                new Product
                {
                    Name ="Capuccino",
                    Description ="Cafea cu spuma de lapte",
                    Price = 10.99M,
                    PortionQuantity = 200,
                    MeasurementUnit = "ml",
                    TotalQuantity = 5000,
                    CategoryId = 6,
                    IsAvailable = true
                },
            };

            products.ForEach(p => context.Products.Add(p));
            context.SaveChanges();

            var productAllergens = new List<ProductAllergen>
            {
                new ProductAllergen { ProductId = 1, AllergenId = 2},
                new ProductAllergen { ProductId = 2, AllergenId = 5 },
                new ProductAllergen { ProductId = 4, AllergenId = 2 }
            };

            productAllergens.ForEach(pa => context.ProductAllergens.Add(pa));
            context.SaveChanges();

            var adminUser = new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin1@restaurant.com",
                PasswordHash = "admin123",
                PhoneNumber = "0721810663",
                DeliveryAddress = "Strada Harmanului 31",
                Role = UserRole.Employee,
                CreatedAt = DateTime.Now
            };
            context.Users.Add(adminUser);

            var customer = new User
            {
                FirstName = "Mihai",
                LastName = "Vilcu",
                Email = "mihaivilcu11@yahoo.com",
                PasswordHash = "client123",
                PhoneNumber = "0734510663",
                DeliveryAddress = "Strada Harmanului 31",
                Role = UserRole.Customer,
                CreatedAt = DateTime.Now
            };
            context.Users.Add(customer);
            context.SaveChanges();

            base.Seed(context);
        }
    }
}
