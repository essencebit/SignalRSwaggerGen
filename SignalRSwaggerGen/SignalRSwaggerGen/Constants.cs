using Microsoft.OpenApi.Models;

namespace SignalRSwaggerGen
{
	public class Constants
	{
		public const string HubNamePlaceholder = "[Hub]";
		public const string MethodNamePlaceholder = "[Method]";
		public const string DefaultHubPath = "hubs/[Hub]";
		public const OperationType DefaultOperationType = OperationType.Post;
	}
}
