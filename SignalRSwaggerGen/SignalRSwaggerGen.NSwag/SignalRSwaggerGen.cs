using Microsoft.AspNetCore.Authorization;
using NJsonSchema;
using NSwag;
using NSwag.Generation;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using SignalRSwaggerGen.Attributes;
using SignalRSwaggerGen.Enums;
using SignalRSwaggerGen.Utils;
using SignalRSwaggerGen.Utils.Comparison;
using SignalRSwaggerGen.Utils.XmlComments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SignalRSwaggerGen.NSwag
{
	internal sealed class SignalRSwaggerGen : IDocumentProcessor
	{
		private static readonly SignalRReturnAttributeComparer _returnAttributeComparer = new SignalRReturnAttributeComparer();
		private static readonly HashSet<JsonObjectType> _primitiveTypes = new HashSet<JsonObjectType> { JsonObjectType.Boolean, JsonObjectType.Integer, JsonObjectType.Number, JsonObjectType.String, JsonObjectType.Array };
		private readonly SignalRSwaggerGenOptions _options;
		private readonly List<XmlComments> _xmlComments;

		public SignalRSwaggerGen(SignalRSwaggerGenOptions options)
		{
			_options = options ?? throw new ArgumentNullException(nameof(options));
			if (_options.Assemblies.Count == 0) _options.ScanAssembly(Assembly.GetEntryAssembly());
			_xmlComments = new List<XmlComments>();
			LoadXmlComments();
		}

		public void Process(DocumentProcessorContext context)
		{
			var hubs = GetHubs();
			foreach (var hub in hubs)
			{
				var xmlComments = GetXmlComments(hub);
				ProcessHub(context, hub, xmlComments);
			}
		}

		private void ProcessHub(
			DocumentProcessorContext context,
			Type hub,
			XmlComments xmlComments)
		{
			var hubAttribute = hub.GetCustomAttribute<SignalRHubAttribute>();
			if (!HubShouldBeDisplayedOnDocument(context, hubAttribute)) return;
			var hubXml = GetHubXml(hub, xmlComments);
			var hubPath = GetHubPath(hub, hubAttribute);
			var hubTag = GetHubTag(hub, hubAttribute, hubXml);
			var hubDescription = GetHubDescription(hubAttribute, hubXml);
			var methods = GetHubMethods(hub, hubAttribute);
			var methodAttributes = methods.ToDictionary(x => x, x => x.GetCustomAttribute<SignalRMethodAttribute>());
			var methodNames = methods.ToDictionary(x => x, x => GetMethodName(x, hubAttribute, methodAttributes[x]));
			context.Document.Tags.Add(new OpenApiTag { Name = hubTag, Description = hubDescription });
			foreach (var method in methods.OrderBy(x => methodNames[x]))
			{
				ProcessMethod(
					context,
					hub,
					hubAttribute,
					hubPath,
					hubTag,
					method,
					methodAttributes,
					methodNames,
					xmlComments);
			}
		}

		private void ProcessMethod(
			DocumentProcessorContext context,
			Type hub,
			SignalRHubAttribute hubAttribute,
			string hubPath,
			string hubTag,
			MethodInfo method,
			Dictionary<MethodInfo, SignalRMethodAttribute> methodAttributes,
			Dictionary<MethodInfo, string> methodNames,
			XmlComments xmlComments)
		{
			var methodAttribute = methodAttributes[method];
			var methodName = methodNames[method];
			var methodIsPolymorphic = MethodIsPolymorphic(methodName, methodNames);
			var methodParams = GetMethodParams(method, hubAttribute, methodAttribute);
			var methodPath = GetMethodPath(hubPath, methodName, methodParams.Length, methodIsPolymorphic);
			var methodReturnParam = method.ReturnParameter;
			var operationType = GetOperationType(methodAttribute);
			var methodXml = GetMethodXml(method, xmlComments);
			var summary = GetMethodSummary(hubAttribute, methodAttribute, methodXml);
			var description = GetMethodDescription(hubAttribute, methodAttribute, methodXml);
			var methodTag = GetMethodTag(hubTag, methodAttribute);
			AddOpenApiPath(
				context,
				hub,
				hubAttribute,
				methodTag,
				methodPath,
				operationType,
				summary,
				description,
				methodParams,
				methodReturnParam,
				method,
				methodAttribute,
				methodXml);
		}

		private void AddOpenApiPath(
			DocumentProcessorContext context,
			Type hub,
			SignalRHubAttribute hubAttribute,
			string tag,
			string methodPath,
			Operation operationType,
			string summary,
			string description,
			IEnumerable<ParameterInfo> methodParams,
			ParameterInfo methodReturnParam,
			MethodInfo method,
			SignalRMethodAttribute methodAttribute,
			MemberElement methodXml)
		{
			var operation = new OpenApiOperation
			{
				Summary = summary,
				Description = description,
				Tags = new List<string> { tag },
				RequestBody = GetOpenApiRequestBody(context, hubAttribute, methodParams, methodXml),
				Security = GetOpenApiSecurityRequirements(hub, method),
				IsDeprecated = MethodIsDeprecated(hub, hubAttribute, method, methodAttribute),
			};
			var parameters = ToOpenApiParameters(context, hubAttribute, methodParams, methodXml);
			foreach (var item in parameters)
			{
				operation.Parameters.Add(item);
			}
			var responses = ToOpenApiResponses(context, methodReturnParam);
			foreach (var item in responses)
			{
				operation.Responses.Add(item.Key, item.Value);
			}
			context.Document.Paths.Add(
				methodPath,
				new OpenApiPathItem
				{
					{
						GetOpenApiOperationMethod(operationType),
						operation
					}
				});
			ApplyOperationProcessors(operation, context, hub, method);
		}

		private List<OpenApiSecurityRequirement> GetOpenApiSecurityRequirements(Type hub, MethodInfo method)
		{
			if (_options.DisableSecurity
				|| hub.GetCustomAttribute<AllowAnonymousAttribute>() != null
				|| method.GetCustomAttribute<AllowAnonymousAttribute>() != null)
				return new List<OpenApiSecurityRequirement> { new OpenApiSecurityRequirement() };

			var authorizeAttributes = method.GetCustomAttributes<AuthorizeAttribute>();
			if (!authorizeAttributes.Any())
				authorizeAttributes = hub.GetCustomAttributes<AuthorizeAttribute>();

			var securitySchemes = authorizeAttributes
				.SelectMany(x => x.AuthenticationSchemes
					?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
					?? Enumerable.Empty<string>())
				.Select(x => x.Trim())
				.Distinct()
				.ToList();

			if (securitySchemes.Count == 0)
				return _options.SecurityRequirements.Count != 0
					? _options.SecurityRequirements.ToList()
					: _options.DisregardOtherSecurityRequirements
						? new List<OpenApiSecurityRequirement> { new OpenApiSecurityRequirement() }
						: null;

			var securityRequirement = new OpenApiSecurityRequirement();
			foreach (var securityScheme in securitySchemes)
			{
				securityRequirement.Add(securityScheme, Array.Empty<string>());
			}

			return new List<OpenApiSecurityRequirement> { securityRequirement };
		}

		private static List<OpenApiParameter> ToOpenApiParameters(
			DocumentProcessorContext context,
			SignalRHubAttribute hubAttribute,
			IEnumerable<ParameterInfo> parameters,
			MemberElement methodXml)
		{
			return parameters
				.Where(x =>
					!x.IsFromBody()
					&& !x.IsFromForm())
				.Select(param =>
				{
					var paramXml = methodXml?.Params?.FirstOrDefault(x => x.Name == param.Name);
					var paramAttribute = param.GetCustomAttribute<SignalRParamAttribute>();
					var paramDescription = GetParamDescription(hubAttribute, paramAttribute, paramXml);
					var paramType = GetParamType(param, paramAttribute);
					var deprecated = ParamIsDeprecated(param, paramAttribute);
					var required = ParamIsRequired(param, paramAttribute);
					var schema = GetSchema(context, paramType);
					var parameter = new OpenApiParameter
					{
						Name = param.Name,
						Description = paramDescription,
						Type = GetOpenApiParameterType(schema),
						Kind = GetOpenApiParameterKind(schema),
						Style = GetOpenApiParameterStyle(schema),
						Schema = schema,
						IsDeprecated = deprecated,
						IsRequired = required,
					};
					if (paramType.IsEnum)
						foreach (var item in Enum.GetValues(paramType))
							parameter.Enumeration.Add(item);
					return parameter;
				})
				.ToList();
		}

		private static OpenApiRequestBody GetOpenApiRequestBody(
			DocumentProcessorContext context,
			SignalRHubAttribute hubAttribute,
			IEnumerable<ParameterInfo> parameters,
			MemberElement methodXml)
		{
			var param = parameters.FirstOrDefault(x =>
				(x.IsFromBody()
				|| x.IsFromForm())
				&& !GetParamType(x, x.GetCustomAttribute<SignalRParamAttribute>()).IsFormFile());

			if (param == null) return null;

			var paramXml = methodXml?.Params?.FirstOrDefault(x => x.Name == param.Name);
			var paramAttribute = param.GetCustomAttribute<SignalRParamAttribute>();
			var paramDescription = GetParamDescription(hubAttribute, paramAttribute, paramXml);
			var paramType = GetParamType(param, paramAttribute);
			var isFromForm = param.IsFromForm();
			var isFormData = isFromForm;
			var isRequired = ParamIsRequired(param, paramAttribute);
			var schema = GetSchema(context, paramType.IsEnum ? typeof(string) : paramType);
			var mediaType = GetOpenApiMediaType(schema);
			var requestBody = new OpenApiRequestBody
			{
				Name = param.Name,
				Description = paramDescription,
				IsRequired = isRequired,
			};
			var content = GetContentByMediaType(mediaType, isFormData);
			requestBody.Content.Clear();
			foreach (var item in content)
			{
				requestBody.Content.Add(item.Key, item.Value);
			}

			return requestBody;
		}

		private static Dictionary<string, OpenApiResponse> ToOpenApiResponses(DocumentProcessorContext context, ParameterInfo returnParam)
		{
			var responses = new Dictionary<string, OpenApiResponse>();
			if (returnParam.GetCustomAttribute<SignalRHiddenAttribute>() != null) return responses;
			var returnAttributes = returnParam.GetCustomAttributes<SignalRReturnAttribute>().Distinct(_returnAttributeComparer).ToList();
			if (returnAttributes.Count == 0) returnAttributes.Add(new SignalRReturnAttribute());
			foreach (var returnAttribute in returnAttributes)
			{
				var responseType = returnAttribute.ReturnType ?? returnParam.ParameterType;
				if (!TryGetReturnType(responseType, out responseType)) continue;
				var schema = GetSchema(context, responseType);
				var mediaType = new OpenApiMediaType
				{
					Schema = schema,
				};
				var response = new OpenApiResponse
				{
					Description = returnAttribute.Description,
				};
				response.Content.Clear();
				var content = GetContentByMediaType(mediaType, false);
				foreach (var item in content)
				{
					response.Content.Add(item.Key, item.Value);
				}
				responses.Add(returnAttribute.StatusCode.ToString(), response);
			}
			return responses;
		}

		private static JsonObjectType GetOpenApiParameterType(JsonSchema schema)
		{
			return schema.Reference == null
				? schema.Type
				: schema.Reference.IsEnumeration
					? JsonObjectType.Integer
					: JsonObjectType.Object;
		}

		private static OpenApiParameterKind GetOpenApiParameterKind(JsonSchema schema)
		{
			return schema.Type == JsonObjectType.File
				? OpenApiParameterKind.FormData
				: OpenApiParameterKind.Query;
		}

		private static OpenApiParameterStyle GetOpenApiParameterStyle(JsonSchema schema)
		{
			return schema.Reference != null
				? (schema.Reference.IsEnumeration
					? OpenApiParameterStyle.Simple
					: OpenApiParameterStyle.DeepObject)
				: (schema.Type == JsonObjectType.File
					? OpenApiParameterStyle.Form
					: OpenApiParameterStyle.Simple);
		}

		private static bool MethodIsDeprecated(
			Type hub,
			SignalRHubAttribute hubAttribute,
			MethodInfo method,
			SignalRMethodAttribute methodAttribute)
		{
			return hub.GetCustomAttribute<ObsoleteAttribute>() != null
				|| method.GetCustomAttribute<ObsoleteAttribute>() != null
				|| (hubAttribute?.Deprecated ?? false)
				|| (methodAttribute?.Deprecated ?? false);
		}

		private static bool ParamIsDeprecated(ParameterInfo param, SignalRParamAttribute paramAttribute)
		{
			return param.GetCustomAttribute<ObsoleteAttribute>() != null
				|| (paramAttribute?.Deprecated ?? false);
		}

		private static bool ParamIsRequired(ParameterInfo param, SignalRParamAttribute paramAttribute)
		{
			return param.GetCustomAttribute<RequiredAttribute>() != null
				|| (paramAttribute?.Required ?? false);
		}

		private static JsonSchema GetSchema(DocumentProcessorContext context, Type type)
		{
			var isIntEnum = type.IsIntEnum();
			var schema = !context.SchemaResolver.HasSchema(type, isIntEnum)
				? context.SchemaGenerator.Generate(type, context.SchemaResolver)
				: context.SchemaResolver.GetSchema(type, isIntEnum);
			return _primitiveTypes.Contains(schema.Type) && !schema.IsEnumeration
				? schema
				: schema.Type == JsonObjectType.File
					? schema
					: new JsonSchema { Reference = schema };
		}

		private static OpenApiMediaType GetOpenApiMediaType(JsonSchema schema)
		{
			return new OpenApiMediaType
			{
				Schema = schema,
			};
		}

		private static Dictionary<string, OpenApiMediaType> GetContentByMediaType(OpenApiMediaType mediaType, bool isFormData)
		{
			return isFormData
				? new Dictionary<string, OpenApiMediaType>
				{
					{ "multipart/form-data", mediaType }
				}
				: new Dictionary<string, OpenApiMediaType>
				{
					{ "application/json", mediaType },
					{ "text/json", mediaType },
					{ "text/plain", mediaType },
				};
		}

		private static string GetOpenApiOperationMethod(Operation operation)
		{
			switch (operation)
			{
				case Operation.Get:
					return OpenApiOperationMethod.Get;
				case Operation.Put:
					return OpenApiOperationMethod.Put;
				case Operation.Post:
					return OpenApiOperationMethod.Post;
				case Operation.Delete:
					return OpenApiOperationMethod.Delete;
				case Operation.Options:
					return OpenApiOperationMethod.Options;
				case Operation.Head:
					return OpenApiOperationMethod.Head;
				case Operation.Patch:
					return OpenApiOperationMethod.Patch;
				case Operation.Trace:
					return OpenApiOperationMethod.Trace;
				default:
					throw new NotSupportedException($"Operation '{operation}' not supported");
			}
		}

		private bool HubShouldBeDisplayedOnDocument(DocumentProcessorContext context, SignalRHubAttribute hubAttribute)
		{
			var documentNames = hubAttribute.DocumentNames ?? _options.DocumentNames;
			return !documentNames.Any()
				|| context.Settings.GetType() != typeof(AspNetCoreOpenApiDocumentGeneratorSettings)
				|| documentNames.Contains(((AspNetCoreOpenApiDocumentGeneratorSettings)context.Settings).DocumentName);
		}

		private string GetHubPath(Type hub, SignalRHubAttribute hubAttribute)
		{
			var hubName = GetHubName(hub);
			var nameTransformer = hubAttribute.NameTransformer ?? _options.NameTransformer;
			if (nameTransformer != null) hubName = nameTransformer.Transform(hubName);
			if (hubAttribute.Path != null) return hubAttribute.Path.Replace(Constants.HubNamePlaceholder, hubName);
			return _options.HubPathFunc(hubName);
		}

		private static string GetHubName(Type hub)
		{
			var hubName = hub.IsInterface && hub.Name[0] == 'I'
				? hub.Name.Substring(1)
				: hub.Name;
			return hubName.Split('`')[0];
		}

		private string GetHubTag(Type hub, SignalRHubAttribute hubAttribute, MemberElement hubXml)
		{
			if (hubAttribute.Tag != null) return hubAttribute.Tag;
			if (!hubAttribute.XmlCommentsDisabled
				&& _options.UseHubXmlCommentsSummaryAsTag
				&& hubXml?.Summary?.Text != null) return hubXml.Summary.Text;
			return GetHubName(hub);
		}

		private string GetHubDescription(SignalRHubAttribute hubAttribute, MemberElement hubXml)
		{
			if (hubAttribute.Description != null) return hubAttribute.Description;
			if (!hubAttribute.XmlCommentsDisabled
				&& !_options.UseHubXmlCommentsSummaryAsTag
				&& _options.UseHubXmlCommentsSummaryAsTagDescription
				&& hubXml?.Summary?.Text != null) return hubXml.Summary.Text;
			return null;
		}

		private static bool MethodIsPolymorphic(string methodName, Dictionary<MethodInfo, string> methodNames)
		{
			return methodNames.Values.Count(x => x == methodName) > 1;
		}

		private static string GetMethodPath(
			string hubPath,
			string methodName,
			int methodParamsCount,
			bool methodIsPolymorphic)
		{
			var methodPathSuffix = methodIsPolymorphic ? new string(' ', methodParamsCount) : null;
			return $"{hubPath}/{methodName}{methodPathSuffix}";
		}

		private string GetMethodName(
			MethodInfo method,
			SignalRHubAttribute hubAttribute,
			SignalRMethodAttribute methodAttribute)
		{
			var methodName = methodAttribute == null
				? method.Name
				: methodAttribute.Name.Replace(Constants.MethodNamePlaceholder, method.Name);
			var nameTransformer = hubAttribute.NameTransformer ?? _options.NameTransformer;
			if (nameTransformer != null) methodName = nameTransformer.Transform(methodName);
			return methodName;
		}

		private static string GetMethodTag(string hubTag, SignalRMethodAttribute methodAttribute)
		{
			return methodAttribute?.Tag ?? hubTag;
		}

		private Operation GetOperationType(SignalRMethodAttribute methodAttribute)
		{
			var operationType = methodAttribute?.Operation ?? Operation.Inherit;
			if (operationType == Operation.Inherit) operationType = _options.Operation;
			return operationType;
		}

		private IEnumerable<Type> GetHubs()
		{
			return _options.Assemblies
				.SelectMany(a =>
					a.GetTypes()
					.Where(t =>
						t.GetCustomAttribute<SignalRHubAttribute>() != null
						&& t.GetCustomAttribute<SignalRHiddenAttribute>() == null));
		}

		private IEnumerable<MethodInfo> GetHubMethods(Type hub, SignalRHubAttribute hubAttribute)
		{
			IEnumerable<MethodInfo> methods = GetAllHubMethods(hub, hubAttribute);
			var autoDiscover = GetAutoDiscover(hubAttribute);
			switch (autoDiscover)
			{
				case AutoDiscover.None:
				case AutoDiscover.Params:
					methods = methods.Where(x => x.GetCustomAttribute<SignalRMethodAttribute>() != null);
					break;
				case AutoDiscover.Methods:
				case AutoDiscover.MethodsAndParams:
					break;
				default:
					throw new NotSupportedException($"Auto-discover option '{autoDiscover}' not supported");
			}
			return methods.Where(x => x.GetCustomAttribute<SignalRHiddenAttribute>() == null);
		}

		private MethodInfo[] GetAllHubMethods(Type hub, SignalRHubAttribute hubAttribute)
		{
			var hubMethodsScan = GetHubMethodsScan(hubAttribute);
			switch (hubMethodsScan)
			{
				case HubMethodsScan.Default:
					return hub.GetMethodsExcludingInherited();
				case HubMethodsScan.IncludeInherited:
					return hub.GetMethodsIncludingInherited(_options.DeclaringTypesOfMethodsToIgnore);
				default:
					throw new NotSupportedException($"Hub methods scan option '{hubMethodsScan}' not supported");
			}
		}

		private ParameterInfo[] GetMethodParams(
			MethodInfo method,
			SignalRHubAttribute hubAttribute,
			SignalRMethodAttribute methodAttribute)
		{
			var autoDiscover = GetAutoDiscover(hubAttribute, methodAttribute);
			IEnumerable<ParameterInfo> methodParams;
			switch (autoDiscover)
			{
				case AutoDiscover.None:
				case AutoDiscover.Methods:
					methodParams = method
						.GetParameters()
						.Where(x => x.GetCustomAttribute<SignalRParamAttribute>() != null);
					break;
				case AutoDiscover.Params:
				case AutoDiscover.MethodsAndParams:
					methodParams = method.GetParameters();
					break;
				default:
					throw new NotSupportedException($"Auto-discover option '{autoDiscover}' not supported");
			}
			return methodParams.Where(x => x.GetCustomAttribute<SignalRHiddenAttribute>() == null).ToArray();
		}

		private AutoDiscover GetAutoDiscover(SignalRHubAttribute hubAttribute)
		{
			var autoDiscover = hubAttribute.AutoDiscover;
			if (autoDiscover == AutoDiscover.Inherit) autoDiscover = _options.AutoDiscover;
			return autoDiscover;
		}

		private AutoDiscover GetAutoDiscover(SignalRHubAttribute hubAttribute, SignalRMethodAttribute methodAttribute)
		{
			var autoDiscover = methodAttribute?.AutoDiscover ?? AutoDiscover.Inherit;
			if (autoDiscover == AutoDiscover.Inherit) autoDiscover = hubAttribute.AutoDiscover;
			if (autoDiscover == AutoDiscover.Inherit) autoDiscover = _options.AutoDiscover;
			return autoDiscover;
		}

		private static string GetMethodSummary(
			SignalRHubAttribute hubAttribute,
			SignalRMethodAttribute methodAttribute,
			MemberElement methodXml)
		{
			var summary = methodAttribute?.Summary;
			if (summary != null) return summary;
			if (!hubAttribute.XmlCommentsDisabled) summary = methodXml?.Summary?.Text;
			return summary;
		}

		private static string GetMethodDescription(
			SignalRHubAttribute hubAttribute,
			SignalRMethodAttribute methodAttribute,
			MemberElement methodXml)
		{
			var description = methodAttribute?.Description;
			if (description != null) return description;
			if (!hubAttribute.XmlCommentsDisabled) description = methodXml?.Remarks?.Text;
			return description;
		}

		private static string GetParamDescription(
			SignalRHubAttribute hubAttribute,
			SignalRParamAttribute paramAttribute,
			ParamElement paramXml)
		{
			var description = paramAttribute?.Description;
			if (description != null) return description;
			if (!hubAttribute.XmlCommentsDisabled) description = paramXml?.Text;
			return description;
		}

		private static Type GetParamType(ParameterInfo param, SignalRParamAttribute paramAttribute)
		{
			return paramAttribute?.ParamType ?? param.ParameterType;
		}

		private HubMethodsScan GetHubMethodsScan(SignalRHubAttribute hubAttribute)
		{
			var hubMethodsScan = hubAttribute.HubMethodsScan;
			if (hubMethodsScan == HubMethodsScan.Inherit) hubMethodsScan = _options.HubMethodsScan;
			return hubMethodsScan;
		}

		private XmlComments GetXmlComments(Type hub)
		{
			return _xmlComments.FirstOrDefault(x => x.Assembly?.Name?.Text == hub.Assembly.GetName().Name);
		}

		private static MemberElement GetHubXml(Type hub, XmlComments xmlComments)
		{
			return xmlComments?.Members?.FirstOrDefault(x => x.Name == hub.GetXmlCommentsName());
		}

		private static MemberElement GetMethodXml(MethodInfo method, XmlComments xmlComments)
		{
			return xmlComments?.Members?.FirstOrDefault(x => x.Name == method.GetXmlCommentsName());
		}

		private static bool TryGetReturnType(Type inType, out Type outType)
		{
			outType = inType;
			if (inType.IsGenericType)
			{
				if (inType.IsGenericTypeDefinition) return false;
				var genericTypeDef = inType.GetGenericTypeDefinition();
				if (genericTypeDef == typeof(Task<>)
					|| genericTypeDef == typeof(ValueTask<>))
				{
					outType = inType.GetGenericArguments()[0];
					return true;
				}
			}
			else
			{
				if (inType == typeof(void)
					|| inType == typeof(Task)
					|| inType == typeof(ValueTask)) return false;
			}
			return true;
		}

		private void LoadXmlComments()
		{
			var xmlSerializer = new XmlSerializer(typeof(XmlComments));
			foreach (var path in _options.PathsToXmlCommentsFiles)
			{
				using (var streamReader = new StreamReader(path))
				{
					var xmlComments = (XmlComments)xmlSerializer.Deserialize(streamReader);
					_xmlComments.Add(xmlComments);
				}
			}
		}

		private void ApplyOperationProcessors(
			OpenApiOperation operation,
			DocumentProcessorContext context,
			Type hub,
			MethodInfo method)
		{
			foreach (var processor in _options.OperationProcessors)
			{
				processor.Process(
					new OperationProcessorContext(
						context.Document,
						context.Document.Operations.FirstOrDefault(x => x.Operation == operation),
						hub,
						method,
						new OpenApiDocumentGenerator(context.Settings, context.SchemaResolver),
						context.SchemaResolver,
						context.Settings,
						context.Document.Operations.ToList()));
			}
		}
	}
}
