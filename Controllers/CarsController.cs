using AutoShopAPI.Models.DTOs;
using AutoShopAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoShopAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly ILogger<CarsController> _logger;

        public CarsController(ICarService carService, ILogger<CarsController> logger)
        {
            _carService = carService;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<ActionResult<AllCarsDTO>> SearchCars(string? textMatch, int? skip = null, int? take = null)
        {
            var cars = await _carService.SearchCarsAsync(textMatch, skip, take);
            return Ok(cars);
        }

        [HttpGet]
        public async Task<ActionResult<AllCarsDTO>> GetCars(int? skip = null, int? take = null)
        {
            var cars = await _carService.GetAllCarsAsync(skip, take);
            return Ok(cars);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CarDTO>> GetCar(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return Ok(car);
        }

        [HttpPost]
        public async Task<ActionResult<CarDTO>> CreateCar(CreateUpdateCarDTO CreateUpdateCarDTO)
        {
            try
            {
                var car = await _carService.CreateCarAsync(CreateUpdateCarDTO);
                return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating car");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id}")]

        public async Task<ActionResult<CarDTO>> UpdateCar(int id, CreateUpdateCarDTO CreateUpdateCarDTO)
        {
            try
            {
                var car = await _carService.UpdateCarAsync(id, CreateUpdateCarDTO);
                return Ok(car);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating car with ID {CarId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                await _carService.DeleteCarAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting car with ID {CarId}", id);
                return BadRequest(ex.Message);
            }
        }
    }

}

