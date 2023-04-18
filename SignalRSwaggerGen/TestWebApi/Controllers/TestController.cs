namespace TestWebApi.Controllers;

[ApiExplorerSettings(GroupName = "controllers")]
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
	[ProducesResponseType(typeof(int), 200)]
	[ProducesResponseType(typeof(string), 400)]
	[HttpGet]
	public IEnumerable<WeatherForecast> Get()
	{
		return default;
	}

	[HttpPost]
	public int Post([FromBody] WeatherForecast body)
	{
		return body.TemperatureC;
	}
}
