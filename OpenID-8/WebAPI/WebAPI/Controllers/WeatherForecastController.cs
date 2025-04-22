// >>> This namespace is added
using Microsoft.AspNetCore.Authorization;
// <<<
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }
        // Authorization is shown here using the Authorize policy option
        //[Authorize]
        [Authorize(Policy = "Admin")]
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            // >>> If you need access to the accesstoken here, then this is one possible solution
            var request = HttpContext.Request;
            var headers = request.Headers;
            string? authorizationHeader = headers["Authorization"];
            string? access_token = (authorizationHeader == null) ? null : authorizationHeader.Split(' ')[1];
            // <<<
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
