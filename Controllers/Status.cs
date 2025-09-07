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
        public ActionResult<bool> IsAlive() => Ok(true);

    }
}
