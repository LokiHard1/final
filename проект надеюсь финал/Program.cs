using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Bogus;

class Program
{
    static string connectionString = "Server=LAPTOP-T7TF19OC\\MSSQLSERVER01;Database=ShopDB;Trusted_Connection=True;";

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Регистрация пользователя");
            Console.WriteLine("2. Добавить продукт");
            Console.WriteLine("3. Показать продукты");
            Console.WriteLine("4. Фильтровать продукты");
            Console.WriteLine("5. Удалить продукт");
            Console.WriteLine("6. Показать юзеров");
            Console.WriteLine("7. Добавить категорию");
            Console.WriteLine("8. Показать категории");
            Console.WriteLine("9. Создать заказ");
            Console.WriteLine("10. Показать заказы");
            Console.WriteLine("11. Добавить 1000 рандомных записей");
            Console.WriteLine("12. Сортировать продукты по цене");
            Console.WriteLine("13. Поиск продукта по имени");
            Console.WriteLine("14. Обновить информацию о продукте");
            Console.WriteLine("0. Выход");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterUser();
                    break;
                case "2":
                    AddProduct();
                    break;
                case "3":
                    ShowProducts();
                    break;
                case "4":
                    FilterProducts();
                    break;
                case "5":
                    DeleteProduct();
                    break;
                case "6":
                    ShowUsers();
                    break;
                case "7":
                    AddCategory();
                    break;
                case "8":
                    ShowCategories();
                    break;
                case "9":
                    CreateOrder();
                    break;
                case "10":
                    ShowOrders();
                    break;
                case "11":
                    SeedDatabase();
                    break;
                case "12":
                    SortProducts();
                    break;
                case "13":
                    SearchProductByName();
                    break;
                case "14":
                    UpdateProduct();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неправильно ввели.");
                    break;
            }
        }
    }

    static void AddCategory()
    {
        Console.Write("Введите название категории: ");
        var categoryName = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("INSERT INTO Categories (Name) VALUES (@Name)", connection);
            command.Parameters.AddWithValue("@Name", categoryName);
            command.ExecuteNonQuery();
            Console.WriteLine("Категория добавлена!");
        }
    }

    static void ShowCategories()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Categories", connection);
            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine("Категории:");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["Id"]}, Name: {reader["Name"]}");
                }
            }
        }
    }

    static void CreateOrder()
    {
        Console.Write("Введите ID пользователя: ");
        var userId = int.Parse(Console.ReadLine());
        Console.Write("Введите ID продукта: ");
        var productId = int.Parse(Console.ReadLine());
        Console.Write("Введите количество: ");
        var quantity = int.Parse(Console.ReadLine());

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("INSERT INTO Orders (UserId, ProductId, Quantity, OrderDate) VALUES (@UserId, @ProductId, @Quantity, @OrderDate)", connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@ProductId", productId);
            command.Parameters.AddWithValue("@Quantity", quantity);
            command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
            command.ExecuteNonQuery();
            Console.WriteLine("Заказ создан!");
        }
    }

    static void ShowOrders()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Orders", connection);
            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine("Заказы:");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["Id"]}, UserId: {reader["UserId"]}, ProductId: {reader["ProductId"]}, Quantity: {reader["Quantity"]}, OrderDate: {reader["OrderDate"]}");
                }
            }
        }
    }
    static void ShowUsers()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Users", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["Id"]}, Username: {reader["Username"]}, Password: {reader["PasswordHash"]}");
                }
            }
        }
    }

    static void RegisterUser()
    {
        Console.Write("Введите имя пользователя: ");
        var username = Console.ReadLine();
        Console.Write("Введите пароль: ");
        var password = Console.ReadLine();
        var passwordHash = HashPassword(password);

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)", connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            command.ExecuteNonQuery();
            Console.WriteLine("Пользователь зарегистрирован!");
        }
    }

    static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    static void AddProduct()
    {
        Console.Write("Введите имя продукта: ");
        var name = Console.ReadLine();
        Console.Write("Введите цену продукта: ");
        var price = decimal.Parse(Console.ReadLine());
        Console.Write("Введите количество на складе: ");
        var stock = int.Parse(Console.ReadLine());

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("INSERT INTO Products (Name, Price, Stock) VALUES (@Name, @Price, @Stock)", connection);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Price", price);
            command.Parameters.AddWithValue("@Stock", stock);
            command.ExecuteNonQuery();
            Console.WriteLine("Продукт добавлен!");
        }
    }
    static void UpdateProduct()
    {
        Console.Write("Введите ID продукта для обновления: ");
        if (int.TryParse(Console.ReadLine(), out int productId))
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var selectCommand = new SqlCommand("SELECT * FROM Products WHERE Id = @ProductId", connection);
                selectCommand.Parameters.AddWithValue("@ProductId", productId);

                using (var reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine("Текущие данные продукта:");
                        Console.WriteLine($"ID: {reader["Id"]}, Наименование: {reader["Name"]}, Цена: {reader["Price"]}, Количество: {reader["Stock"]}");

                        Console.Write("Введите новое наименование (оставьте пустым для сохранения текущего): ");
                        string newName = Console.ReadLine();
                        Console.Write("Введите новую цену (оставьте пустым для сохранения текущей): ");
                        string priceInput = Console.ReadLine();
                        Console.Write("Введите новое количество (оставьте пустым для сохранения текущего): ");
                        string stockInput = Console.ReadLine();

                        string updatedName = string.IsNullOrEmpty(newName) ? reader["Name"].ToString() : newName;
                        decimal updatedPrice = string.IsNullOrEmpty(priceInput) ? (decimal)reader["Price"] : decimal.Parse(priceInput);
                        int updatedStock = string.IsNullOrEmpty(stockInput) ? (int)reader["Stock"] : int.Parse(stockInput);

                        reader.Close(); 
                        var updateCommand = new SqlCommand("UPDATE Products SET Name = @NewName, Price = @NewPrice, Stock = @NewStock WHERE Id = @ProductId", connection);
                        updateCommand.Parameters.AddWithValue("@NewName", updatedName);
                        updateCommand.Parameters.AddWithValue("@NewPrice", updatedPrice);
                        updateCommand.Parameters.AddWithValue("@NewStock", updatedStock);
                        updateCommand.Parameters.AddWithValue("@ProductId", productId);

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        Console.WriteLine(rowsAffected > 0 ? "Продукт успешно обновлён." : "Не удалось обновить продукт.");
                    }
                    else
                    {
                        Console.WriteLine("Продукт не найден.");
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("Некорректный ID продукта.");
        }
    }
    static void SortProducts()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Products ORDER BY Price", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["Id"]}, Name: {reader["Name"]}, Price: {reader["Price"]}, Stock: {reader["Stock"]}");
                }
            }
        }
    }
    static void SearchProductByName()
    {
        Console.Write("Введите подстроку для поиска продукта: ");
        var searchString = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Products WHERE Name LIKE @SearchString", connection);
            command.Parameters.AddWithValue("@SearchString", "%" + searchString + "%");

            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine("Результаты поиска:");
                bool found = false;
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["Id"]}, Name: {reader["Name"]}, Price: {reader["Price"]}, Stock: {reader["Stock"]}");
                    found = true;
                }
                if (!found)
                {
                    Console.WriteLine("Продукты не найдены.");
                }
            }
        }
    }
    static void ShowProducts()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Products", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["Id"]}, Name: {reader["Name"]}, Price: {reader["Price"]}, Stock: {reader["Stock"]}");
                }
            }
        }
    }

    static void FilterProducts()
    {
        Console.Write("Введите минимальную цену: ");
        var minPrice = decimal.Parse(Console.ReadLine());
        Console.Write("Введите максимальную цену: ");
        var maxPrice = decimal.Parse(Console.ReadLine());

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Products WHERE Price BETWEEN @MinPrice AND @MaxPrice", connection);
            command.Parameters.AddWithValue("@MinPrice", minPrice);
            command.Parameters.AddWithValue("@MaxPrice", maxPrice);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["Id"]}, Name: {reader["Name"]}, Price: {reader["Price"]}, Stock: {reader["Stock"]}");
                }
            }
        }
    }

    static void DeleteProduct()
    {
        Console.Write("Введите ID продукта для удаления: ");
        var productId = int.Parse(Console.ReadLine());

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            var command = new SqlCommand("DELETE FROM Products WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", productId);
            command.ExecuteNonQuery();
            Console.WriteLine("Продукт удален!");
        }
    }

    static void SeedDatabase()
    {
        //создаем
        var faker = new Faker();
        var users = new List<(string Username, string PasswordHash)>();
        var products = new List<(string Name, decimal Price, int Stock)>();
        var categories = new List<string>();

        for (int i = 0; i < 1000; i++)
        {
            var username = faker.Internet.UserName();
            var password = faker.Internet.Password();
            var passwordHash = HashPassword(password);
            users.Add((username, passwordHash));
        }

        for (int i = 0; i < 1000; i++)
        {
            var productName = faker.Commerce.ProductName();
            var price = faker.Commerce.Price();
            var stock = faker.Random.Int(1, 1000);
            products.Add((productName, decimal.Parse(price), stock));
        }

        for (int i = 0; i < 100; i++)
        {
            var categoryName = faker.Commerce.Categories(1).First();
            categories.Add(categoryName);
        }
        //вставляем
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            foreach (var user in users)
            {
                var command = new SqlCommand("INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)", connection);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.ExecuteNonQuery();
            }

            foreach (var product in products)
            {
                var command = new SqlCommand("INSERT INTO Products (Name, Price, Stock) VALUES (@Name, @Price, @Stock)", connection);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@Stock", product.Stock);
                command.ExecuteNonQuery();
            }

            foreach (var category in categories)
            {
                var command = new SqlCommand("INSERT INTO Categories (Name) VALUES (@Name)", connection);
                command.Parameters.AddWithValue("@Name", category);
                command.ExecuteNonQuery();
            }

            //связи
            var productIDs = Enumerable.Range(1, products.Count).ToList();
            var categoryIDs = Enumerable.Range(1, categories.Count).ToList();

            foreach (var productId in productIDs)
            {
                var categoryId = faker.PickRandom(categoryIDs);
                var command = new SqlCommand("INSERT INTO ProductCategories (ProductId, CategoryId) VALUES (@ProductId, @CategoryId)", connection);
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@CategoryId", categoryId);
                command.ExecuteNonQuery();
            }
        }

        Console.WriteLine("1000 рандомных записей добавлены в каждую таблицу");
    }
}