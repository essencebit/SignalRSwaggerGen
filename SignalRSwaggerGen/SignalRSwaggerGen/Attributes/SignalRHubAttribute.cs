using SignalRSwaggerGen.Enums;
using SignalRSwaggerGen.Naming;
using SignalRSwaggerGen.Utils;
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

		/// <param name="path">Relative path of the hub.
		/// If path contains "[Hub]", this part will be replaced with the name of the type holding this attribute.</param>
		/// <param name="autoDiscover">A flag indicating what components will have Swagger documentation enabled automatically</param>
		/// <param name="documentNames">An array of names of the Swagger documents the hub will be displayed in. If null or empty array specified, then the hub will be displayed in all documents.</param>
		/// <param name="nameTransformerType">The type of the name transformer. The type must inherit from 'SignalRSwaggerGen.Naming.NameTransformer' class, be non-abstract and have public parameterless constructor.
		/// The name transformer will be used to transform the name of the hub and its methods. If null specified, no transformation will happen. The namespace 'SignalRSwaggerGen.Naming' already contains some predefined name transformers, so check 'em out.</param>
		/// <exception cref="ArgumentException">Thrown if
		/// - <paramref name="path"/> is null or empty
		/// - <paramref name="autoDiscover"/> value not allowed for this attribute
		/// - <paramref name="nameTransformerType"/> is abstract or does not inherit from 'SignalRSwaggerGen.Naming.NameTransformer' class or has no public parameterless constructor</exception>
		public SignalRHubAttribute(string path = Constants.DefaultHubPath, AutoDiscover autoDiscover = AutoDiscover.None, string[] documentNames = null, Type nameTransformerType = null)
		{
			if (path.IsNullOrEmpty()) throw new ArgumentException("Path is null or empty", nameof(path));
			if (!_validAutoDiscoverValues.Contains(autoDiscover)) throw new ArgumentException($"Value {autoDiscover} not allowed for this attribute", nameof(autoDiscover));
			ValidateNameTransformerType(nameTransformerType);
			Path = path;
			AutoDiscover = autoDiscover;
			DocumentNames = documentNames?.ToList().Distinct() ?? Enumerable.Empty<string>();
			NameTransformer = nameTransformerType == null ? null : (NameTransformer)Activator.CreateInstance(nameTransformerType);
		}

		private static readonly IEnumerable<AutoDiscover> _validAutoDiscoverValues = new List<AutoDiscover>
		{
			AutoDiscover.None,
			AutoDiscover.Methods,
			AutoDiscover.MethodsAndArgs
		};

		private static void ValidateNameTransformerType(Type nameTransformerType)
		{
			if (nameTransformerType == null) return;
			if (nameTransformerType.IsAbstract) throw new ArgumentException($"Type {nameTransformerType.Name} is abstract", nameof(nameTransformerType));
			if (!nameTransformerType.IsSubclassOf(typeof(NameTransformer))) throw new ArgumentException($"Type {nameTransformerType.Name} is not a subtype of {typeof(NameTransformer).Name}", nameof(nameTransformerType));
			if (nameTransformerType.GetConstructor(Type.EmptyTypes) == null) throw new ArgumentException($"Type {nameTransformerType.Name} has no public parameterless constructor", nameof(nameTransformerType));
		}
	}
}
