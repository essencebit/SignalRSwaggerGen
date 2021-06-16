using Microsoft.OpenApi.Models;
using SignalRSwaggerGen.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SignalRSwaggerGen
{
	/// <summary>
	/// This class can be used by Swagger to generate documentation for SignalR hubs.
	/// In order for Swagger to use this class, just add this class as document filter for Swagger generator.
	/// Don't forget to add assemblies which contain SignalR hubs as parameters for document filter.
	/// </summary>
	public sealed class SignalRSwaggerGen : IDocumentFilter
	{
		private IEnumerable<Assembly> Assemblies { get; }

		/// <param name="assemblies">Assemblies which contain SignalR hubs</param>
		/// <exception cref="ArgumentException">Thrown if no assemblies provided</exception>
		public SignalRSwaggerGen(IEnumerable<Assembly> assemblies)
		{
			if (assemblies == null || assemblies.Count() == 0) throw new ArgumentException("No assemblies provided", nameof(assemblies));
			Assemblies = assemblies;
		}

		/// <summary>
		/// This method is automatically called by Swagger generator
		/// </summary>
		/// <param name="swaggerDoc"></param>
		/// <param name="context"></param>
		public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
		{
			var hubs = Assemblies.SelectMany(x => x.GetTypes().Where(x => x.GetCustomAttribute<SignalRHubAttribute>() != null));
			foreach (var hub in hubs)
			{
				ProcessHub(swaggerDoc, context, hub);
			}
		}

		private void ProcessHub(OpenApiDocument swaggerDoc, DocumentFilterContext context, Type hub)
		{
			var hubAttribute = hub.GetCustomAttribute<SignalRHubAttribute>();
			var hubPath = GetHubPath(hub, hubAttribute);
			var tag = GetTag(hub);
			var methods = hub.GetMethods().Where(x => x.GetCustomAttribute<SignalRMethodAttribute>() != null);
			foreach (var method in methods)
			{
				ProcessMethod(swaggerDoc, context, hubPath, tag, method);
			}
		}

		private void ProcessMethod(OpenApiDocument swaggerDoc, DocumentFilterContext context, string hubPath, string tag, MethodInfo method)
		{
			var methodAttribute = method.GetCustomAttribute<SignalRMethodAttribute>();
			var methodPath = GetMethodPath(hubPath, method, methodAttribute);
			var methodArgs = method.GetParameters().Where(x => x.GetCustomAttribute<SignalRArgAttribute>() != null);
			foreach (var arg in methodArgs)
			{
				GenerateOpenApiSchemaForType(context, arg.ParameterType);
			}
			AddOpenApiPath(swaggerDoc, tag, methodPath, methodAttribute.OperationType, methodArgs);
		}

		private void AddOpenApiPath(OpenApiDocument swaggerDoc, string tag, string methodPath, OperationType operationType, IEnumerable<ParameterInfo> methodArgs)
		{
			swaggerDoc.Paths.Add(
				methodPath,
				new OpenApiPathItem
				{
					Operations = new Dictionary<OperationType, OpenApiOperation>
					{
						{
							operationType,
							new OpenApiOperation
							{
								Tags = new List<OpenApiTag> { new OpenApiTag { Name = tag } },
								Parameters = ToOpenApiParameters(methodArgs).ToList()
							}
						}
					}
				});
		}

		private void GenerateOpenApiSchemaForType(DocumentFilterContext context, Type type)
		{
			if (context.SchemaRepository.TryLookupByType(type, out OpenApiSchema _)) return;
			context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
		}

		private static IEnumerable<OpenApiParameter> ToOpenApiParameters(IEnumerable<ParameterInfo> args)
		{
			return args.Select(x => new OpenApiParameter
			{
				Name = x.Name,
				In = ParameterLocation.Query,
				Schema = new OpenApiSchema
				{
					Reference = new OpenApiReference
					{
						Id = GetSchemaId(x.ParameterType),
						Type = ReferenceType.Schema
					}
				}
			});
		}

		private static string GetHubPath(Type hub, SignalRHubAttribute hubAttribute)
		{
			return hubAttribute.Path.Replace(Constants.HubNamePlaceholder, hub.Name);
		}

		private static string GetMethodPath(string hubPath, MethodInfo method, SignalRMethodAttribute methodAttribute)
		{
			return $"{hubPath}/{methodAttribute.Name.Replace(Constants.MethodNamePlaceholder, method.Name)}";
		}

		private static string GetTag(Type hub)
		{
			return $"{hub.Name}(SignalR)";
		}

		private static string GetSchemaId(Type type)
		{
			return type.Name;
		}
	}
}
