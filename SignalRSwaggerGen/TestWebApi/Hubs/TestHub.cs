using SignalRSwaggerGen.Attributes;
using SignalRSwaggerGen.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace TestWebApi.Hubs
{
	[SignalRHub(autoDiscover: AutoDiscover.MethodsAndArgs, documentNames: new[] { "hubs" })]
	public class TestHub
	{
		[return: SignalRReturn]
		[SignalRMethod(summary: "method1 summary", description: "method1 description", autoDiscover: AutoDiscover.Args)]
		public ValueTask TestMethod(
			int agr1,
			string arg2,
			[SignalRArg(description: "arg3 description")] WeatherForecast arg3,
			[SignalRHidden] CancellationToken cancellationToken)
		{
			return default;
		}

		[return: SignalRReturn(typeof(Task<WeatherForecast>), 200, "Success")]
		[return: SignalRReturn(returnType: typeof(ValueTask<>), statusCode: 201, description: "Created")]
		[SignalRMethod(summary: "method2 summary", description: "method2 description", autoDiscover: AutoDiscover.Args)]
		public void TestMethod2(
			[SignalRArg(description: "arg1 description")] int agr1,
			[SignalRArg(description: "arg2 description")] string arg2,
			WeatherForecast arg3,
			[SignalRHidden] CancellationToken cancellationToken)
		{
			return;
		}

		[return: SignalRHidden]
		public Task<WeatherForecast> TestMethod3(
			int agr1,
			string arg2,
			WeatherForecast arg3,
			[SignalRHidden] CancellationToken cancellationToken)
		{
			return default;
		}
	}
}
