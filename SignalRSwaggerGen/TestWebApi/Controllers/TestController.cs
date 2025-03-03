using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalRSwaggerGen.Attributes;
using SignalRSwaggerGen.Enums;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TestWebApi.Controllers
{
	[SignalRHub(autoDiscover: AutoDiscover.MethodsAndParams, path: "/hubs/Test", tag: "Test")]
	[ApiExplorerSettings(GroupName = "controllers", IgnoreApi = false)]
	[ApiController]
	[Route("[controller]")]
	public class TestController : ControllerBase
	{
		[HttpPost("query")]
		public void query([FromQuery] WeatherForecast query)
		{
			return;
		}

		[HttpPost("get")]
		public IEnumerable<WeatherForecast> get([FromBody] int arg1, string arg2)
		{
			return default;
		}

		[HttpPost("form")]
		public void form([FromForm] WeatherForecast form)
		{
			return;
		}

		[HttpPost("body")]
		public void body([FromBody] WeatherForecast body)
		{
			return;
		}

		[HttpPost("file")]
		public void file(IFormFile formFile)
		{
			return;
		}

		[HttpPost("myEnum")]
		public MyEnum myEnum([FromBody] MyEnum arg1, MyEnum arg2, MyEnum arg3)
		{
			return MyEnum.Value1;
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
