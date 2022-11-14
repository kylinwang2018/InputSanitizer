using Microsoft.AspNetCore.Mvc;
using InputSanitizer.Demo.WebApi.Dtos;
using Newtonsoft.Json.Linq;

namespace InputSanitizer.Demo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [SanitizeAllInputFilter(PolicyName = "Default")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "PostWeatherForecast")]
        public IActionResult Post(dynamic data)
        {
            //if (!ModelState.IsValid)
            //{
            //    return UnprocessableEntity(ModelState);
            //}
            return Ok(data);
        }
    }
}