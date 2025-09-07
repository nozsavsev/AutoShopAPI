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
        public async Task<ActionResult<AllCarsDTO>> SearchCars([FromQuery] SearchCarFilters filters)
        {
            var result = await _carService.SearchCarsAsync(filters);
            if (!result.Success)
            {
                return Problem(result.Message, statusCode: 400);
            }
            return Ok(result.Value);
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CarDTO>> GetCar(int id)
        {
            var result = await _carService.GetCarByIdAsync(id);
            if (!result.Success)
            {
                return Problem(result.Message, statusCode: 404);
            }
            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult<CarDTO>> CreateCar(CreateUpdateCarDTO createUpdateCarDTO)
        {
            var result = await _carService.CreateCarAsync(createUpdateCarDTO);
            if (!result.Success)
            {
                return Problem(result.Message, statusCode: 400);
            }
            return Ok(result.Value);
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<CarDTO>>> BulkCreateCars(IEnumerable<CreateUpdateCarDTO> createUpdateCarDTOs)
        {
            var result = await _carService.BulkCreateCarsAsync(createUpdateCarDTOs);
            if (!result.Success)
            {
                return Problem(result.Message, statusCode: 400);
            }
            return Ok(result.Value);
        }

        [HttpPut]
        [Route("{id}")]

        public async Task<ActionResult<CarDTO>> UpdateCar(int id, CreateUpdateCarDTO createUpdateCarDTO)
        {
            var result = await _carService.UpdateCarAsync(id, createUpdateCarDTO);
            if (!result.Success)
            {
                return Problem(result.Message, statusCode: 400);
            }
            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var result = await _carService.DeleteCarAsync(id);
            if (!result.Success)
            {
                return Problem(result.Message, statusCode: 400);
            }
            return Ok(true);
        }
    }

}

