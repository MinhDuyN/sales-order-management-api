using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.Entities;

namespace OrderAPI.Data
{
    public static class DataSeed
    {
        public static void Seed(AppDbContext db)
        {
            SeedRoles(db);
            SeedUsers(db);
            SeedCategories(db);
            SeedProducts(db);
            SeedOrders(db);
        }

        // ROLES
        private static void SeedRoles(AppDbContext db)
        {
            if (db.Roles.Any()) return;

            db.Roles.AddRange(
                new Role { RoleName = "Admin" },
                new Role { RoleName = "Staff" },
                new Role { RoleName = "Customer" }
            );
            db.SaveChanges();
        }

        // USERS
        private static void SeedUsers(AppDbContext db)
        {
            if (db.Users.Any()) return;

            var adminRole = db.Roles.AsNoTracking().First(r => r.RoleName == "Admin");
            var staffRole = db.Roles.AsNoTracking().First(r => r.RoleName == "Staff");
            var customerRole = db.Roles.AsNoTracking().First(r => r.RoleName == "Customer");

            db.Users.AddRange(
                new User { Name = "Admin", Email = "admin@erp.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@erp"), RoleId = adminRole.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new User { Name = "Staff", Email = "staff@erp.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("staff@erp"), RoleId = staffRole.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new User { Name = "Customer A", Email = "customer@erp.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("customer@erp"), RoleId = customerRole.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new User { Name = "Customer B", Email = "customer2@erp.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("customer@erp"), RoleId = customerRole.Id, IsActive = true, CreatedDate = DateTime.UtcNow }
            );
            db.SaveChanges();
        }

        // CATEGORIES
        private static void SeedCategories(AppDbContext db)
        {
            if (db.Categories.Any()) return;

            db.Categories.AddRange(
                new Category { CategoryName = "Laptop", CreatedDate = DateTime.UtcNow },
                new Category { CategoryName = "Điện thoại", CreatedDate = DateTime.UtcNow },
                new Category { CategoryName = "Phụ kiện", CreatedDate = DateTime.UtcNow },
                new Category { CategoryName = "Màn hình", CreatedDate = DateTime.UtcNow }
            );
            db.SaveChanges();
        }

        // PRODUCTS
        private static void SeedProducts(AppDbContext db)
        {
            if (db.Products.Any()) return;

            var laptop = db.Categories.AsNoTracking().First(c => c.CategoryName == "Laptop");
            var phone = db.Categories.AsNoTracking().First(c => c.CategoryName == "Điện thoại");
            var accessory = db.Categories.AsNoTracking().First(c => c.CategoryName == "Phụ kiện");
            var monitor = db.Categories.AsNoTracking().First(c => c.CategoryName == "Màn hình");

            db.Products.AddRange(
                new Product { ProductName = "MacBook Air M2", Price = 28_000_000, StockQuantity = 20, CategoryId = laptop.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Product { ProductName = "Dell XPS 13", Price = 25_000_000, StockQuantity = 15, CategoryId = laptop.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Product { ProductName = "Asus ZenBook 14", Price = 18_000_000, StockQuantity = 30, CategoryId = laptop.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Product { ProductName = "iPhone 15 Pro", Price = 30_000_000, StockQuantity = 50, CategoryId = phone.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Product { ProductName = "Samsung Galaxy S24", Price = 22_000_000, StockQuantity = 40, CategoryId = phone.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Product { ProductName = "Xiaomi 14", Price = 15_000_000, StockQuantity = 35, CategoryId = phone.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Product { ProductName = "Chuột Logitech MX Master 3", Price = 2_500_000, StockQuantity = 60, CategoryId = accessory.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Product { ProductName = "Bàn phím Keychron K2", Price = 2_200_000, StockQuantity = 45, CategoryId = accessory.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Product { ProductName = "Tai nghe Sony WH-1000XM5", Price = 8_000_000, StockQuantity = 25, CategoryId = accessory.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Product { ProductName = "LG UltraWide 34\"", Price = 12_000_000, StockQuantity = 10, CategoryId = monitor.Id, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Product { ProductName = "Dell U2723D 27\"", Price = 10_500_000, StockQuantity = 12, CategoryId = monitor.Id, IsActive = true, CreatedDate = DateTime.UtcNow }
            );
            db.SaveChanges();
        }

        // ORDERS + ORDER ITEMS + PAYMENTS
        private static void SeedOrders(AppDbContext db)
        {
            if (db.Orders.Any()) return;

            var now = DateTime.UtcNow;

            var customerA = db.Users.AsNoTracking().First(u => u.Email == "customer@erp.com");
            var customerB = db.Users.AsNoTracking().First(u => u.Email == "customer2@erp.com");

            var iphone = db.Products.AsNoTracking().First(p => p.ProductName == "iPhone 15 Pro");
            var mouse = db.Products.AsNoTracking().First(p => p.ProductName == "Chuột Logitech MX Master 3");
            var macbook = db.Products.AsNoTracking().First(p => p.ProductName == "MacBook Air M2");
            var dell = db.Products.AsNoTracking().First(p => p.ProductName == "Dell XPS 13");
            var sony = db.Products.AsNoTracking().First(p => p.ProductName == "Tai nghe Sony WH-1000XM5");
            var keyboard = db.Products.AsNoTracking().First(p => p.ProductName == "Bàn phím Keychron K2");
            var xiaomi = db.Products.AsNoTracking().First(p => p.ProductName == "Xiaomi 14");
            var samsung = db.Products.AsNoTracking().First(p => p.ProductName == "Samsung Galaxy S24");

            var order1 = new Order
            {
                UserId = customerA.Id,
                Status = "Shipped",
                TotalAmount = iphone.Price + mouse.Price * 2,
                CreatedDate = now.AddDays(-20),
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = iphone.Id, Quantity = 1, UnitPrice = iphone.Price, LineTotal = iphone.Price },
                    new OrderItem { ProductId = mouse.Id,  Quantity = 2, UnitPrice = mouse.Price,  LineTotal = mouse.Price * 2 }
                }
            };

            var order2 = new Order
            {
                UserId = customerA.Id,
                Status = "Confirmed",
                TotalAmount = macbook.Price,
                CreatedDate = now.AddDays(-10),
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = macbook.Id, Quantity = 1, UnitPrice = macbook.Price, LineTotal = macbook.Price }
                }
            };

            var order3 = new Order
            {
                UserId = customerB.Id,
                Status = "Shipped",
                TotalAmount = dell.Price + sony.Price + keyboard.Price * 2,
                CreatedDate = now.AddDays(-15),
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = dell.Id,     Quantity = 1, UnitPrice = dell.Price,     LineTotal = dell.Price },
                    new OrderItem { ProductId = sony.Id,     Quantity = 1, UnitPrice = sony.Price,     LineTotal = sony.Price },
                    new OrderItem { ProductId = keyboard.Id, Quantity = 2, UnitPrice = keyboard.Price, LineTotal = keyboard.Price * 2 }
                }
            };

            var order4 = new Order
            {
                UserId = customerB.Id,
                Status = "Pending",
                TotalAmount = xiaomi.Price,
                CreatedDate = now.AddDays(-2),
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = xiaomi.Id, Quantity = 1, UnitPrice = xiaomi.Price, LineTotal = xiaomi.Price }
                }
            };

            var order5 = new Order
            {
                UserId = customerA.Id,
                Status = "Cancelled",
                TotalAmount = samsung.Price,
                CreatedDate = now.AddDays(-25),
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = samsung.Id, Quantity = 1, UnitPrice = samsung.Price, LineTotal = samsung.Price }
                }
            };

            db.Orders.AddRange(order1, order2, order3, order4, order5);
            db.SaveChanges();

            db.Payments.AddRange(
                new Payment { OrderId = order1.Id, Amount = order1.TotalAmount, Status = "Paid", Method = "BankTransfer", CreatedDate = order1.CreatedDate.AddHours(2) },
                new Payment { OrderId = order3.Id, Amount = order3.TotalAmount, Status = "Paid", Method = "CreditCard", CreatedDate = order3.CreatedDate.AddHours(1) }
            );
            db.SaveChanges();
        }
    }
}