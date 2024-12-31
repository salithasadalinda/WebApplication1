using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http; // Imports the Http namespace for handling HTTP requests and responses.
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;
using System.Data;
using System.Data.Odbc; // Imports the MVC namespace, which provides the foundation for building web APIs.

namespace WebApplication1.Controllers // Declares a namespace to organize your controllers.
{
    [Route("api/[controller]")] // Defines the base route for this controller. The "[controller]" part will be replaced with "Products" during runtime.
    [ApiController] // Marks this class as an API controller, enabling features like automatic model binding and validation.
    public class ProductsOdbcController : ControllerBase // Defines the ProductsController class, inheriting from ControllerBase to handle HTTP requests.
    {
        private readonly string connectionString; // Declares a private string variable to store the database connection string.
        public ProductsOdbcController(IConfiguration configuration) // Constructor for the ProductsController, taking an IConfiguration object as a parameter.
        {
            connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? ""; // Retrieves the connection string from the configuration object and assigns it to the connectionString variable. If the connection string is not found, it assigns an empty string.
        }

        //product apis
        //create->get ->insert
        [HttpPost]
        public IActionResult CreateProduct(ProductDto productDto)
        {
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO products" +
                        "(name,brand,category,price,description) VALUES" +
                        "(?,?,?,?,?)";
                    using (var command = new OdbcCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", productDto.Name);
                        command.Parameters.AddWithValue("@brand", productDto.Brand);
                        command.Parameters.AddWithValue("@category", productDto.Category);
                        command.Parameters.AddWithValue("@price", productDto.Price);
                        command.Parameters.AddWithValue("@description", productDto.Description);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("Product", "sorry, but we have an exception ");
                return BadRequest(ModelState);
            }
            return Ok();

        }

        //Read->post->select from
        [HttpGet]
        public IActionResult GetProduct()
        {
            List<Product> products = new List<Product>();
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM products";
                    using (var command = new OdbcCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product product = new Product();

                                product.Id = reader.GetInt32(0);
                                product.Name = reader.GetString(1);
                                product.Brand = reader.GetString(2);
                                product.Category = reader.GetString(3);
                                product.Price = reader.GetDecimal(4);
                                product.Description = reader.GetString(5);
                                product.CreatedAt = reader.GetDateTime(6);

                                products.Add(product);
                            }
                        }

                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("Product", "Sorry, but we have an exception");
                return BadRequest(ModelState);
            }

            return Ok(products);
        }

        // create a product by id
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            Product product = new Product();
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM products WHERE id=?";
                    using (var command = new OdbcCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            //only one product so add a one condition
                            if (reader.Read())
                            {
                                product.Id = reader.GetInt32(0);
                                product.Name = reader.GetString(1);
                                product.Brand = reader.GetString(2);
                                product.Category = reader.GetString(3);
                                product.Price = reader.GetDecimal(4);
                                product.Description = reader.GetString(5);
                                product.CreatedAt = reader.GetDateTime(6);
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("Product", "sorry, but we have an exception ");
                return BadRequest(ModelState);
            }

            return Ok(product);
        }


        // update a product using it's id
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, ProductDto productDto)
        {
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE products SET name = ? , brand = ? ,category= ?," +
                        "price= ?,description = ? WHERE id = ?";
                    using (var command = new OdbcCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", productDto.Name);
                        command.Parameters.AddWithValue("@brand", productDto.Brand);
                        command.Parameters.AddWithValue("@category", productDto.Category);
                        command.Parameters.AddWithValue("@price", productDto.Price);
                        command.Parameters.AddWithValue("@description", productDto.Description);
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("Product", "sorry, but we have an exception");
                return BadRequest(ModelState);
            }
            return Ok(productDto);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                   connection.Open ();
                   string sql = "DELETE FROM products WHERE id = ?";
                   using (var command = new OdbcCommand(sql, connection))
                   {
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                   }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("Product", "sorry, but we have an exception");
                return BadRequest(ModelState);
            }
            return Ok();
        }
    }
}
