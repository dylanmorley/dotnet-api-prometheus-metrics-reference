using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;
using Prometheus;

namespace PrometheusMetrics.Api.Reference.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    // Inbuilt net6 meter/metric implementation
    private static readonly Meter Meter = new Meter("Demo.Meter", "1.0.0");
    private static readonly Counter<int> WeatherPredicted = Meter.CreateCounter<int>("weather_predicted");
    
    // Prometheus-net library example
    private static readonly Histogram StormPredictionDuration = 
        Metrics.CreateHistogram("weather_storm_prediction_time", "Histogram of storm prediction processing durations.");
    
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
    private static readonly string[] Storms = {
        "Stormy", "No Storm"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        // This is an example of using net6 counter metrics, a simple increment here and because we've 
        // configured these to be converted into prometheus format by MeterAdapter, you'll see this in the output
        // at the /metrics endpoint after you've called this method
        WeatherPredicted.Add(1);
        
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }

    [HttpGet(Name = "PredictStorm")]
    public StormPrediction PredictStorm()
    {
        double delay = Random.Shared.Next(1000, 3000);
        
        // This is an example of using a histogram metric from the prometheus-net library. Again, will be exposed
        // at the /metrics endpoint, after you've called this method
        
        // Whether you use the inbuilt net6 metrics or are happy to adopt a library that provides the capability is
        // a design choice for your application
        using (StormPredictionDuration.NewTimer())
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(delay));
        }

        return new StormPrediction
        {
            Date = DateTime.Today.AddDays(1),
            Summary = Storms[Random.Shared.Next(Storms.Length)]
        };
    }
}