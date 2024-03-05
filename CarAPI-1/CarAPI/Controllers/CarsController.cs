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
    [Route("apiV1/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private string createTable = "CREATE TABLE Cars (licensplate VARCHAR(16),make VARCHAR(128),model VARCHAR(128),color VARCHAR(128),PRIMARY KEY (licensplate))";
        // GET: api/Car
        /// <summary>
        /// Gets all cars in the table. Optional search arguments "make" and "color"
        /// </summary>
        /// <returns> List of Car</returns>
        //
        // Name of method is irrelevant. It is the routing that matters: [HttpGet]
        [HttpGet]
        public IActionResult Get_ThisNameDoesNotMatter()
        {
            using (var connection = new SqlConnection(System.Environment.GetEnvironmentVariable("ConnectionString")))
            {
                connection.Open();
                try
                {
                    // Return a List<Car>
                    return Ok(connection.Query<Car>("SELECT * FROM dbo.Cars").ToList());
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
                List<Car> cars = connection.Query<Car>("SELECT * FROM dbo.Cars").ToList();
                return Ok(connection.Query<Car>("SELECT * FROM dbo.Cars").ToList());
            }
        }

        // GET api/<CarsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CarsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
