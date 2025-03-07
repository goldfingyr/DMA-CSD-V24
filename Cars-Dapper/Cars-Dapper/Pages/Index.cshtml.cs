// Author: Karsten Jeppesen, UCN, 2024
//
// This exeercise is the prelude to the CarAPI exercise
//
// CarAPI is used to demonstrate the potential of a REST API
// The exercise suggested is building a total API which may be accessed
// from:
//  - PostMan
//  - Swagger
//  - Razor based application
//
// Nuget Packages:
// - Dapper
// - System.Data.SqlClient
//
// NOTE: You must define the environment variable: ConnectionString
// NOTE: In the "Debug Launch Profile" you must change "App URL" to "http://localhost:15000"
//
// CarAPI-1: Adding the apiV1/Cars HttpGET action
// CarAPI-2: Adding the additional apiV2/Cars/AA 12345 HttpGET action
//           Adding the apiV1/Cars HttpPOST action 
// CarAPI-3: Adding filter to the apiV1/Cars HttpGET action
// CarAPI-4: Adding Razor UI as a multiproject solution


using Cars_Dapper.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Cars_Dapper.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Car> Cars { get; set; }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            using (var connection = new SqlConnection(System.Environment.GetEnvironmentVariable("ConnectionString")))
            {
                string createTable = "CREATE TABLE Cars (licenseplate VARCHAR(16),make VARCHAR(128),model VARCHAR(128),color VARCHAR(128),PRIMARY KEY (licenseplate))";
                connection.Open();
                try
                {
                    // Return a List<Car>
                    Cars = connection.Query<Car>("SELECT * FROM dbo.Cars").ToList();
                    return;
                }
                catch
                {
                    // It doesn't exist - create and add two cars
                    connection.Execute(createTable);
                    connection.ExecuteScalar<Car>("INSERT INTO dbo.Cars (licenseplate,make,model,color) VALUES (@licenseplate, @make, @model, @color)", new
                    {
                        licenseplate = "AA 12345",
                        make = "Toyota",
                        model = "Yaris",
                        color = "Silver"
                    });
                    connection.ExecuteScalar<Car>("INSERT INTO dbo.Cars (licenseplate,make,model,color) VALUES (@licenseplate, @make, @model, @color)", new
                    {
                        licenseplate = "BB 12345",
                        make = "Ford",
                        model = "Ka",
                        color = "Red"
                    });
                }
                // Return a two item List
                List<Car> cars = connection.Query<Car>("SELECT * FROM dbo.Cars").ToList();
                Cars = connection.Query<Car>("SELECT * FROM dbo.Cars").ToList();
            }
        }
    }
}
