namespace Refactoring.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Data.SqlClient;
    using System.Security.Cryptography;
    using Microsoft.Data.Sqlite;

    [ApiController]
    [Route("/api/order")]
    public class OrderController : ControllerBase
    {
        [HttpPost("process", Name = "ProcessOrder")]
        public ActionResult ProcessOrder([FromQuery] int productId, [FromQuery] string productType, [FromQuery] string paymentType)
        {
            // Fetch product
            using (var connection = new SqliteConnection("Data Source=orders.db"))
            {
                connection.Open();
                var command = new SqliteCommand($"SELECT * FROM Products WHERE Id = {productId} AND Type = '{productType}'", connection);
                var reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    throw new Exception("Product not found");
                }

                var product = new Product
                {
                    Id = (long)reader["Id"],
                    Name = (string)reader["Name"],
                    Price = (double)reader["Price"]
                };

                Console.WriteLine($"Product {product.Name} fetched.");

                // Process payment
                if (paymentType == "CreditCard")
                {
                    Console.WriteLine($"Processing credit card payment for {product.Price}");
                }
                else if (paymentType == "PayPal")
                {
                    Console.WriteLine($"Processing PayPal payment for {product.Price}");
                }
                else
                {
                    throw new Exception("Unsupported payment type");
                }

                // Send confirmation email
                using (var client = new HttpClient())
                {
                    try
                    {
                        var response = client.GetStringAsync("https://api.emailservice.com/send?to=user@example.com&message=Order confirmed").Result;
                        Console.WriteLine(response);
                    }
                    catch
                    {
                    }

                }
            }

            return Ok();
        }
    }

    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}
