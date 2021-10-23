using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;
using SignalRSwaggerGen.Enums;
using System.Threading.Tasks;

namespace TestWebApi.Hubs
{
    public interface IHubClient
    {
        Task ClientTestMethod(int arg1, string arg2, WeatherForecast forecast);
    }

    [SignalRHub(autoDiscover: AutoDiscover.MethodsAndArgs, documentNames: new[] { "hubs" })]
    public class StronglyTypedTestHub : Hub<IHubClient>
    {
        public async Task HubTestMethod(int arg1, string arg2, WeatherForecast forecast)
        {
            await Task.CompletedTask;
        }
    }
}
