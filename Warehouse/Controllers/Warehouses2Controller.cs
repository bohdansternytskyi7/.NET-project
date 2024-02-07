using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using WarehouseApp.Models;

namespace WarehouseApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Warehouses2Controller: ControllerBase
    {
        private readonly IConfiguration _configuration;

        public Warehouses2Controller(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> AddWarehouse([FromBody] Envelope env)
        {
            using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            using SqlCommand command = new SqlCommand("AddProductToWarehouse", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@IdProduct", env.IdProduct);
            command.Parameters.AddWithValue("@IdWarehouse", env.IdWarehouse);
            command.Parameters.AddWithValue("@Amount", env.Amount);
            command.Parameters.AddWithValue("@CreatedAt", env.CreatedAt);

            await connection.OpenAsync();
            await command.ExecuteScalarAsync();
            return Ok("Successfully added.");
        }
    }
}
