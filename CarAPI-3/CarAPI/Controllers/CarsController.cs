// Author: Karsten Jeppesen, UCN, 2023
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



using CarAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CarAPI.Controllers
{
    [Route("apiV2/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private string createTable = "CREATE TABLE Cars (licensplate VARCHAR(16),make VARCHAR(128),model VARCHAR(128),color VARCHAR(128),PRIMARY KEY (licensplate))";
        // GET: api/Car
        /// <summary>
        /// Gets all cars in the table. Optional search arguments "make" and "color"
        /// </summary>
        /// <param name="make"> Optional search argument</param>
        /// <param name="color"> Optional search argument</param>
        /// <returns> List of Car</returns>
        //
        // Name of method is irrelevant. It is the routing that matters: [HttpGet]
        [HttpGet]
        public IActionResult Get_ThisNameDoesNotMatter(string? make = "%", string? color = "%")
        {
            using (var connection = new SqlConnection(System.Environment.GetEnvironmentVariable("ConnectionString")))
            {
                connection.Open();
                try
                {
                    return Ok(connection.Query<Car>("SELECT * FROM dbo.Cars WHERE make LIKE @make AND color LIKE @color", new
                    {
                        make = make,
                        color = color
                    }).ToList());
                }
                catch
                {
                    // It doesn't exist - create and add two cars
                    connection.Execute(createTable);
                    connection.ExecuteScalar<Car>("INSERT INTO dbo.Cars (licensplate,make,model,color) VALUES (@licensplate, @make, @model, @color)", new
                    {
                        licensplate = "AA 12345",
                        make = "Toyota",
                        model = "Yaris",
                        color = "Silver"
                    });
                    connection.ExecuteScalar<Car>("INSERT INTO dbo.Cars (licensplate,make,model,color) VALUES (@licensplate, @make, @model, @color)", new
                    {
                        licensplate = "BB 12345",
                        make = "Ford",
                        model = "Ka",
                        color = "Red"
                    });
                }
                // Return a two item List
                return Ok(connection.Query<Car>("SELECT * FROM dbo.Cars WHERE make LIKE @make AND color LIKE @color", new
                {
                    make = make,
                    color = color
                }).ToList());
            }
        }


        // GET api/Cars/cm57812
        /// <summary>
        /// Get a car identified by its licensplate. Note URLEncode(AA 12345): AA%2012345
        /// </summary>
        /// <param name="licensplate"></param>
        /// <returns>Car</returns>
        //
        // Name of method is irrelevant. It is the routing that matters: [HttpGet("{licensplate}")]
        [HttpGet("{licensplate}")]
        public IActionResult GetByLicenceplate(string licensplate)
        {
            using (var connection = new SqlConnection(System.Environment.GetEnvironmentVariable("ConnectionString")))
            {
                connection.Open();
                try
                {
                    return Ok(connection.Query<Car>("SELECT * FROM dbo.Cars WHERE licensplate='" + licensplate + "'").FirstOrDefault());
                }
                catch
                {
                    connection.Execute(createTable);
                }
                return Ok(connection.Query<Car>("SELECT * FROM dbo.Cars WHERE licensplate='" + licensplate + "'").FirstOrDefault());
            }
        }

        // POST api/<CarsController>
        [HttpPost]
        public IActionResult Post(string iLicensplate, string iMake, string iModel, string iColor)
        {
            Car car = new()
            {
                licensplate = iLicensplate,
                make = iMake,
                model = iModel,
                color = iColor
            };
            using (var connection = new SqlConnection(System.Environment.GetEnvironmentVariable("ConnectionString")))
            {
                connection.Open();
                try
                {
                    return Ok(connection.ExecuteScalar<Car>("INSERT INTO dbo.Cars (licensplate,make,model,color) VALUES (@licensplate, @make, @model, @color)", car));
                }
                catch
                {
                    connection.Execute(createTable);
                    return Ok(connection.ExecuteScalar<Car>("INSERT INTO dbo.Cars (licensplate,make,model,color) VALUES (@Licensplate, @Make, @Model, @Color)", new
                    {
                        Licensplate = iLicensplate,
                        Make = iMake,
                        Model = iModel,
                        color = iColor
                    }));
                }
            }
        }

        // PUT api/<CarsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CarsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
