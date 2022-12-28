using SignalRSwaggerGen;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extensions for SwaggerGenOptions
	/// </summary>
	public static class SwaggerGenOptionsExtensions
	{
		/// <summary>
		/// Add SignalRSwaggerGen to generate documentation for SignalR hubs
		/// </summary>
		/// <param name="swaggerGenOptions">...</param>
		public static void AddSignalRSwaggerGen(this SwaggerGenOptions swaggerGenOptions)
		{
			var signalRSwaggerGenOptions = new SignalRSwaggerGenOptions();
			swaggerGenOptions.DocumentFilter<SignalRSwaggerGen.SignalRSwaggerGen>(signalRSwaggerGenOptions);
		}

		/// <summary>
		/// Add SignalRSwaggerGen to generate documentation for SignalR hubs
		/// </summary>
		/// <param name="swaggerGenOptions">...</param>
		/// <param name="action">Action for setting up options for SignalRSwaggerGen</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
		public static void AddSignalRSwaggerGen(this SwaggerGenOptions swaggerGenOptions, Action<SignalRSwaggerGenOptions> action)
		{
			if (action == null) throw new ArgumentNullException(nameof(action));
			var signalRSwaggerGenOptions = new SignalRSwaggerGenOptions();
			action(signalRSwaggerGenOptions);
			swaggerGenOptions.DocumentFilter<SignalRSwaggerGen.SignalRSwaggerGen>(signalRSwaggerGenOptions);
		}
	}
}
