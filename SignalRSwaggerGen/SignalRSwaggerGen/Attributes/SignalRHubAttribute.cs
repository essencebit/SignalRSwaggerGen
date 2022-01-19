using SignalRSwaggerGen.Enums;
using SignalRSwaggerGen.Naming;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRSwaggerGen.Attributes
{
	/// <summary>
	/// Use this attribute to enable Swagger documentation for hubs
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	public sealed class SignalRHubAttribute : Attribute
	{
		public string Path { get; }
		public AutoDiscover AutoDiscover { get; }
		public IEnumerable<string> DocumentNames { get; }
		public NameTransformer NameTransformer { get; }
		public string Tag { get; }
		public bool XmlCommentsDisabled { get; }

		/// <param name="path">Path of the hub. If path contains "[Hub]", this part will be replaced with the name of the type holding this attribute(hub name).
		/// If not specified, the func from 'SignalRSwaggerGenOptions' will be used to get the path.</param>
		/// <param name="autoDiscover">A flag indicating what components will have Swagger documentation enabled automatically.
		/// If 'AutoDiscover.Inherit' specified, the value from 'SignalRSwaggerGenOptions' will be used.</param>
		/// <param name="documentNames">An array of names of the Swagger documents the hub will be displayed in.
		/// If null specified, then the value from 'SignalRSwaggerGenOptions' will be used. If empty array specified, then the hub will be displayed in all documents.</param>
		/// <param name="nameTransformerType">The type of the name transformer. The type must inherit from 'SignalRSwaggerGen.Naming.NameTransformer' class, be non-abstract and have public parameterless constructor.
		/// The name transformer will be used to transform the name of the hub and its methods. If null specified, the transformer from 'SignalRSwaggerGenOptions' will be used.
		/// The namespace 'SignalRSwaggerGen.Naming' already contains some predefined name transformers, so check 'em out.</param>
		/// <param name="tag">The tag under which the hub will be placed in Swagger doc. If null specified, the summary section of the XML comments of the hub will be used.
		/// If XML comments missing or not enabled, the name of the type holding this attribute will be used.</param>
		/// <param name="xmlCommentsDisabled">A flag indicating if XML comments are disabled for the hub</param>
		/// <exception cref="ArgumentException">Thrown if
		/// - <paramref name="autoDiscover"/> value not allowed for this attribute
		/// - <paramref name="nameTransformerType"/> is abstract or does not inherit from 'SignalRSwaggerGen.Naming.NameTransformer' class or has no public parameterless constructor</exception>
		public SignalRHubAttribute(
			string path = null,
			AutoDiscover autoDiscover = AutoDiscover.Inherit,
			string[] documentNames = null,
			Type nameTransformerType = null,
			string tag = null,
			bool xmlCommentsDisabled = false)
		{
			if (!_validAutoDiscoverValues.Contains(autoDiscover)) throw new ArgumentException($"Value {autoDiscover} not allowed for this attribute", nameof(autoDiscover));
			ValidateNameTransformerType(nameTransformerType);
			Path = path;
			AutoDiscover = autoDiscover;
			DocumentNames = documentNames;
			NameTransformer = nameTransformerType == null ? null : (NameTransformer)Activator.CreateInstance(nameTransformerType);
			Tag = tag;
			XmlCommentsDisabled = xmlCommentsDisabled;
		}

		private static void ValidateNameTransformerType(Type nameTransformerType)
		{
			if (nameTransformerType == null) return;
			if (nameTransformerType.IsAbstract) throw new ArgumentException($"Type {nameTransformerType.Name} is abstract", nameof(nameTransformerType));
			if (!nameTransformerType.IsSubclassOf(typeof(NameTransformer))) throw new ArgumentException($"Type {nameTransformerType.Name} is not a subtype of {typeof(NameTransformer).Name}", nameof(nameTransformerType));
			if (nameTransformerType.GetConstructor(Type.EmptyTypes) == null) throw new ArgumentException($"Type {nameTransformerType.Name} has no public parameterless constructor", nameof(nameTransformerType));
		}

		private static readonly IEnumerable<AutoDiscover> _validAutoDiscoverValues = new List<AutoDiscover>
		{
			AutoDiscover.Inherit,
			AutoDiscover.None,
			AutoDiscover.Methods,
			AutoDiscover.Params,
			AutoDiscover.MethodsAndParams,
		};
	}
}
