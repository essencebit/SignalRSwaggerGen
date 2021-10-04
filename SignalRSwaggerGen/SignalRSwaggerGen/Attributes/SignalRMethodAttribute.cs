using Microsoft.OpenApi.Models;
using SignalRSwaggerGen.Enums;
using SignalRSwaggerGen.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRSwaggerGen.Attributes
{
	/// <summary>
	/// Use this attribute to enable Swagger documentation for hub methods
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class SignalRMethodAttribute : Attribute
	{
		public string Name { get; }
		public OperationType OperationType { get; }
		public AutoDiscover AutoDiscover { get; }
		public string Summary { get; }
		public string Description { get; }

		/// <param name="name">Name of the method which will be invoked on the client side.
		/// If name contains "[Method]", this part will be replaced with the name of the method holding this attribute.</param>
		/// <param name="operationType">Same as HTTP verb</param>
		/// <param name="autoDiscover">A flag indicating what components will have Swagger documentation enabled automatically</param>
		/// <param name="summary">The text that will appear in summary section of decorated method in Swagger document</param>
		/// <param name="description">The text that will appear in description section of decorated method in Swagger document</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty, or auto-discover value not allowed for this attribute</exception>
		public SignalRMethodAttribute(
			string name = Constants.MethodNamePlaceholder,
			OperationType operationType = Constants.DefaultOperationType,
			AutoDiscover autoDiscover = AutoDiscover.None,
			string summary = null,
			string description = null)
		{
			if (name.IsNullOrEmpty()) throw new ArgumentException("Name is null or empty", nameof(name));
			if (!_validAutoDiscoverValues.Contains(autoDiscover)) throw new ArgumentException($"Value {autoDiscover} not allowed for this attribute", nameof(autoDiscover));
			Name = name;
			OperationType = operationType;
			AutoDiscover = autoDiscover;
			Summary = summary;
			Description = description;
		}

		private static readonly IEnumerable<AutoDiscover> _validAutoDiscoverValues = new List<AutoDiscover>
		{
			AutoDiscover.None,
			AutoDiscover.Args
		};
	}
}
