using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestWebApi.Controllers
{
	[ApiExplorerSettings(GroupName = "controllers")]
	[ApiController]
	[Route("[controller]")]
	public class TestController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		[ProducesResponseType(typeof(int), 200)]
		[ProducesResponseType(typeof(string), 400)]
		[HttpGet]
		public IEnumerable<WeatherForecast> Get()
		{
			var rng = new Random();
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateTime.Now.AddDays(index),
				TemperatureC = rng.Next(-20, 55),
				Summary = Summaries[rng.Next(Summaries.Length)]
			})
			.ToArray();
		}

		[HttpPost]
		public int Post()
		{
			var rng = new Random();
			return rng.Next();
		}
	}
}
