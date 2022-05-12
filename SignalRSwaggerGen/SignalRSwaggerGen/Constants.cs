using SignalRSwaggerGen.Enums;

namespace SignalRSwaggerGen
{
	public class Constants
	{
		public const string HubNamePlaceholder = "[Hub]";
		public const string MethodNamePlaceholder = "[Method]";
		public const string DefaultHubPathTemplate = "/hubs/" + HubNamePlaceholder;
		public const AutoDiscover DefaultAutoDiscover = AutoDiscover.MethodsAndParams;
		public const Operation DefaultOperation = Operation.Post;
	}
}
