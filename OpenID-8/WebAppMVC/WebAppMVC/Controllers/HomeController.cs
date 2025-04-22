using Microsoft.AspNetCore.Authentication;      // Added namespace
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;                          // Added namespace
using Newtonsoft.Json.Linq;                     // Added namespace
using System.Diagnostics;
using System.Text;                              // Added namespace
using WebAppMVC.Models;
using WebAppMVC.Services;                       // Added namespace

namespace WebAppMVC.Controllers
{
    public class HomeController : Controller
    {
        // >>> Added
        public WeatherDataList weatherData { get; set; }
        private readonly WeatherService _myService;
        // <<<

        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger, WeatherService myService)     // Changed
        {
            _logger = logger;
            _myService = myService;                                                         // Added
        }

        // >>> Added
        public static string DecodeBase64String(string base64EncodedData)
        {
            // Decode the base64 encoded string to a byte array
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

            // Convert the byte array to a plain text string
            string decodedString = Encoding.UTF8.GetString(base64EncodedBytes);

            return decodedString;
        }
        // <<<

        // >>> Changed to demonstrate Access Token retrieval
        public IActionResult Index(string theaction = "none")
        {
            // If an access_token was provided, then this part shows how to get at it
            string? accessToken = HttpContext.GetTokenAsync("access_token").Result;
            if (accessToken != null)
            {
                JObject accessTokenBody = JObject.Parse(DecodeBase64String(accessToken.Split(".", 3)[1]));
                string name = (string)accessTokenBody["name"];
                ViewBag.name = name;
            }
            try
            {
                var data = _myService.GetProtectedDataAsync(accessToken).Result;
                List<WeatherData> weatherDataList = JsonConvert.DeserializeObject<List<WeatherData>>(data);
                weatherData = new();
                weatherData.data = new();
                foreach (var item in weatherDataList)
                {
                    weatherData.data.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            ViewBag.weatherData = weatherData;
            return View();
        }
        // <<<

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    // >>> Added for formality
    public class WeatherData
    {
        public string date { get; set; }
        public string temperatureC { get; set; }
        public string temperatureF { get; set; }
        public string summary { get; set; }

    }

    public class WeatherDataList
    {
        public List<WeatherData> data { get; set; }
    }
    // <<< Added
}
