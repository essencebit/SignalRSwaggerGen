using Microsoft.OpenApi.Models;
using SignalRSwaggerGen.Utils;
using System;

namespace SignalRSwaggerGen.Attributes
{
	/// <summary>
	/// Use this attribute to enable Swagger documentation for hub methods
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class SignalRMethodAttribute : Attribute
	{
		internal string Name { get; }
		internal OperationType OperationType { get; }

		/// <param name="name">Name of the method which will be invoked on the client side.
		/// If "[Method]" is specified, the name of the method holding this attribute will be used.</param>
		/// <param name="operationType">Same as HTTP verb</param>
		/// <exception cref="ArgumentException">Thrown if name is null or empty</exception>
		public SignalRMethodAttribute(string name = Constants.MethodNamePlaceholder, OperationType operationType = OperationType.Post)
		{
			if (name.IsNullOrEmpty()) throw new ArgumentException("Name is null or empty", nameof(name));
			Name = name;
			OperationType = operationType;
		}
	}
}
