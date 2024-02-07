using Microsoft.AspNetCore.Mvc;
using WarehouseApp.Models;
using WarehouseApp.Services;

namespace WarehouseApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WarehousesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly WarehousesService _warehouseService;

        public WarehousesController(IConfiguration configuration, WarehousesService warehouseService)
        {
            _configuration = configuration;
            _warehouseService = warehouseService;
        }

        [HttpPost]
        public async Task<IActionResult> AddWarehouse([FromBody] Envelope env)
        {
            try
            {
                var result = await _warehouseService.ValidateObject(env);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}