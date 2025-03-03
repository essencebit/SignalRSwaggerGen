using NSwag.Generation.AspNetCore;
using SignalRSwaggerGen.NSwag;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extensions for AspNetCoreOpenApiDocumentGeneratorSettings
	/// </summary>
	public static class AspNetCoreOpenApiDocumentGeneratorSettingsExtensions
	{
		/// <summary>
		/// Add SignalRSwaggerGen to generate documentation for SignalR hubs
		/// </summary>
		/// <param name="settings">...</param>
		public static void AddSignalRSwaggerGen(this AspNetCoreOpenApiDocumentGeneratorSettings settings)
		{
			var signalRSwaggerGenOptions = new SignalRSwaggerGenOptions();
			var signalRSwaggerGen = new SignalRSwaggerGen.NSwag.SignalRSwaggerGen(signalRSwaggerGenOptions);
			settings.DocumentProcessors.Add(signalRSwaggerGen);
		}

		/// <summary>
		/// Add SignalRSwaggerGen to generate documentation for SignalR hubs
		/// </summary>
		/// <param name="settings">...</param>
		/// <param name="action">Action for setting up options for SignalRSwaggerGen</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
		public static void AddSignalRSwaggerGen(this AspNetCoreOpenApiDocumentGeneratorSettings settings, Action<SignalRSwaggerGenOptions> action)
		{
			if (action == null) throw new ArgumentNullException(nameof(action));
			var signalRSwaggerGenOptions = new SignalRSwaggerGenOptions();
			action(signalRSwaggerGenOptions);
			var signalRSwaggerGen = new SignalRSwaggerGen.NSwag.SignalRSwaggerGen(signalRSwaggerGenOptions);
			settings.DocumentProcessors.Add(signalRSwaggerGen);
		}
	}
}
