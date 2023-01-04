using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;
using SignalRSwaggerGen.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestWebApi.Hubs
{
	[SignalRHub(autoDiscover: AutoDiscover.MethodsAndParams, documentNames: new[] { "hubs" })]
	public class StronglyTypedTestHub : Hub<IStronglyTypedTestHub>
	{
		public async Task TestMethod4()
		{
			await Clients.All.TestMethod(default, default, default, default);
		}
	}

	[SignalRHub(autoDiscover: AutoDiscover.MethodsAndParams, documentNames: new[] { "hubs" })]
	public interface IStronglyTypedTestHub
	{
		[Authorize]
		[return: SignalRReturn]
		[SignalRMethod(summary: "method1 summary", description: "method1 description", autoDiscover: AutoDiscover.Params)]
		public ValueTask TestMethod(
			int agr1,
			string arg2,
			[SignalRParam(description: "arg3 description")] WeatherForecast arg3,
			[SignalRHidden] CancellationToken cancellationToken);

		[Obsolete]
		[return: SignalRReturn(typeof(Task<WeatherForecast>), 200, "Success")]
		[return: SignalRReturn(returnType: typeof(ValueTask<>), statusCode: 201, description: "Created")]
		[SignalRMethod(summary: "method2 summary", description: "method2 description", autoDiscover: AutoDiscover.Params)]
		public void TestMethod2(
			[SignalRParam(description: "arg1 description")] int agr1,
			[SignalRParam(description: "arg2 description")] string arg2,
			WeatherForecast arg3,
			[SignalRHidden] CancellationToken cancellationToken);

		[return: SignalRHidden]
		public Task<WeatherForecast> TestMethod3(
			[SignalRParam(paramType: typeof(WeatherForecast))] int agr1,
			string arg2,
			[FromBody] WeatherForecast arg3,
			[SignalRHidden] CancellationToken cancellationToken);

		[return: SignalRHidden]
		public Task<WeatherForecast> TestMethod3();
	}
}
