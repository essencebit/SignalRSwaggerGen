using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TestWebApi.Controllers
{
	[ApiExplorerSettings(GroupName = "controllers")]
	[ApiController]
	[Route("[controller]")]
	public class TestController : ControllerBase
	{
		[HttpGet]
		public IEnumerable<WeatherForecast> Get()
		{
			return default;
		}

		/// <summary>
		/// Summary
		/// </summary>
		/// <remarks>Remarks</remarks>
		/// <example>Example</example>
		/// <param name="form"></param>
		/// <returns></returns>
		[HttpPost("form")]
		public WeatherForecast Post([FromForm] WeatherForecast form)
		{
			return default;
		}

		[HttpPost("body")]
		public int Post([FromBody] WeatherForecast body, [FromQuery] string _)
		{
			return default;
		}

		[HttpPost("form-file")]
		public int Post(File formFile)
		{
			return default;
		}
	}

	public class File : IFormFile
	{
		public string ContentType => throw new System.NotImplementedException();

		public string ContentDisposition => throw new System.NotImplementedException();

		public IHeaderDictionary Headers => throw new System.NotImplementedException();

		public long Length => throw new System.NotImplementedException();

		public string Name => throw new System.NotImplementedException();

		public string FileName => throw new System.NotImplementedException();

		public void CopyTo(Stream target)
		{
			throw new System.NotImplementedException();
		}

		public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
		{
			throw new System.NotImplementedException();
		}

		public Stream OpenReadStream()
		{
			throw new System.NotImplementedException();
		}
	}
}
