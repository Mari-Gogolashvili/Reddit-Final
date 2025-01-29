using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reddit.Controllers
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

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            // Log the start of the operation
            _logger.LogInformation("WeatherForecastController.Get - Starting to generate weather forecasts.");

            try
            {
                var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();

                // Log the successful generation of forecasts
                _logger.LogInformation("WeatherForecastController.Get - Successfully generated {Count} weather forecasts.", forecasts.Length);

                return forecasts;
            }
            catch (Exception ex)
            {
                // Log the exception with details
                _logger.LogError(ex, "WeatherForecastController.Get - An error occurred while generating weather forecasts.");

                // Re-throw the exception to be handled by the global exception handler
                throw;
            }
            finally
            {
                // Log the end of the operation
                _logger.LogInformation("WeatherForecastController.Get - Completed generating weather forecasts.");
            }
        }
    }
}