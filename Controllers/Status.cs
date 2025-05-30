using AutoShopAPI.Models.DTOs;
using AutoShopAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {

        public StatusController()
        {
        }

        [HttpGet]
        public async Task<ActionResult> IsAlive()
        {
            return Ok(true);
        }

    }
}
