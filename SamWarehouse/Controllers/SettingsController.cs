using Microsoft.AspNetCore.Mvc;

namespace SamWarehouse.Controllers
    {

    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
        {
        [HttpPost("SetTheme")]
        public async Task<IActionResult> SetTheme([FromBody] ThemeSettings setting)
            {
            HttpContext.Session.SetString("Theme", setting.Theme);
            return Ok();
            }

        public class ThemeSettings
            {
            public string Theme { get; set; }
            }
        }
    }
