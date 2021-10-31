using System;

namespace SignalRSwaggerGen.Attributes
{
	/// <summary>
	/// Use this attribute to enable Swagger documentation for method args
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public sealed class SignalRArgAttribute : Attribute
	{
		public string Description { get; }
		public Type ArgType { get; }

		/// <param name="description">The text that will appear in description section of decorated argument in Swagger document</param>
		/// <param name="argType">Argument type. If null specified, the type of the argument holding this attribute will be picked up.</param>
		public SignalRArgAttribute(string description = null, Type argType = null)
		{
			Description = description;
			ArgType = argType;
		}
	}
}
