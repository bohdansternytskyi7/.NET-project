using System.Data;
using System.Data.SqlClient;
using WarehouseApp.Models;

namespace WarehouseApp.Services
{
    public interface IWarehousesService
    {
        Task<int> ValidateObject(Envelope productWarehouse);
    }
    public class WarehousesService : IWarehousesService
    {
        private readonly IConfiguration _configuration;
        public WarehousesService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> ValidateObject(Envelope env)
        {
            if (env.Amount <= 0)
                throw new ArgumentException("Amount is less or equal to zero.");

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                string productQuery = "SELECT TOP 1 * FROM Product WHERE IdProduct = @IdProduct";
                var product = new Product();
                using (SqlCommand command = new SqlCommand(productQuery, connection))
                {
                    command.Parameters.AddWithValue("@IdProduct", env.IdProduct);
                    product.IdProduct = env.IdProduct;
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            product.Price = reader.GetDecimal("Price");
                            product.Name = reader.GetString("Name");
                            product.Description = reader.GetString("Description");
                        }
                    }
                    if (product.Price == 0)
                        throw new KeyNotFoundException($"Product with ID {env.IdProduct} does not exist in the database.");
                }

                string warehouseQuery = "SELECT COUNT(*) FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
                using (SqlCommand command = new SqlCommand(warehouseQuery, connection))
                {
                    command.Parameters.AddWithValue("@IdWarehouse", env.IdWarehouse);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    if (count <= 0)
                        throw new KeyNotFoundException($"Warehouse with ID {env.IdWarehouse} does not exist in the database.");
                }

                string orderQuery = "SELECT TOP 1 * FROM \"Order\" WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt";
                var order = new Order();
                using ( SqlCommand command = new SqlCommand(orderQuery, connection))
                {
                    command.Parameters.AddWithValue("@IdProduct", env.IdProduct);
                    command.Parameters.AddWithValue("@Amount", env.Amount);
                    command.Parameters.AddWithValue("@CreatedAt", env.CreatedAt);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            order.IdOrder = reader.GetInt32("IdOrder");
                            order.IdProduct = reader.GetInt32("IdProduct");
                            order.Amount = reader.GetInt32("Amount");
                            order.CreatedAt = reader.GetDateTime("CreatedAt");
                        }
                    }
                    if (order.IdProduct == 0)
                        throw new Exception("Order does not exist in the database.");
                }

                string productWarehouseQuery = "SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder = @IdOrder";
                using (SqlCommand command = new SqlCommand(productWarehouseQuery, connection))
                {
                    command.Parameters.AddWithValue("@IdOrder", order.IdOrder);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    if (count > 0)
                        throw new Exception("Order is already fulfilled.");
                }

                string updateOrderQuery = "UPDATE \"Order\" SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
                using (SqlCommand command = new SqlCommand(updateOrderQuery, connection))
                {
                    command.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);
                    command.Parameters.AddWithValue("@IdOrder", order.IdOrder);
                    await command.ExecuteNonQueryAsync();
                }

                string insertProductWarehouseQuery = "INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)" +
                    " VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt); SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(@insertProductWarehouseQuery, connection))
                {
                    command.Parameters.AddWithValue("@IdWarehouse", env.IdWarehouse);
                    command.Parameters.AddWithValue("@IdProduct", env.IdProduct);
                    command.Parameters.AddWithValue("@IdOrder", order.IdOrder);
                    command.Parameters.AddWithValue("@Amount", order.Amount);
                    command.Parameters.AddWithValue("@Price", order.Amount * product.Price);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    var result = await command.ExecuteScalarAsync();
                    if (result != null && result != DBNull.Value)
                        return Convert.ToInt32(result);
                }
                return 0;
            }
        }
    }
}
