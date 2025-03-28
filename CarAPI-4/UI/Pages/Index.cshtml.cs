//
// NuGet packages:
// - Newtonsoft.Json
//
using CarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string? CarSelectedLicenceplate { get; set; }

        [BindProperty]
        public Car CarSelected { get; set; }

        [BindProperty]
        public List<Car> Cars { get; set; }

        private void SetMyCookie(string carSelected)
        {
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddMinutes(5);
            cookieOptions.Path = "/";
            Response.Cookies.Append("CarAPICookie", JsonConvert.SerializeObject(carSelected), cookieOptions);
        }


        private void GetMyCookie()
        {
            string? jsonString;
            try
            {
                CarSelectedLicenceplate = JsonConvert.DeserializeObject<string>(Request.Cookies["CarAPICookie"]);
            }
            catch
            {
                jsonString = null;
            }
            //if (jsonString != null)
            //{
            //    CarSelected = JsonConvert.DeserializeObject<string>(jsonString);
            //    return;
            //}
            //else
            //{
            //    CarSelected = null;
            //}
        }



        public string APIURI = "http://localhost:15000";
        public void OnGet()
        {
        }

        public IActionResult OnPost(string action = "NA", string target = "NA")
        {
            switch (action)
            {
                case "findCar":
                    SetMyCookie(target);
                    return Redirect("/");
                    break;
                default:
                    break;
            }
            return null;
        }

        public IActionResult OnPostAjax1(string data)
        {
            return new JsonResult(new { result = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt") });
        }
    }
}
