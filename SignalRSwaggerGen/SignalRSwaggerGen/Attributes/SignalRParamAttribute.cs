using System;

namespace SignalRSwaggerGen.Attributes
{
	/// <summary>
	/// Use this attribute to enable Swagger documentation for method params
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public sealed class SignalRParamAttribute : Attribute
	{
		public string Description { get; }
		public Type ParamType { get; }

		/// <param name="description">The text that will appear in description section of decorated parameter in Swagger document</param>
		/// <param name="paramType">Parameter type. If null specified, the type of the parameter holding this attribute will be picked up.</param>
		public SignalRParamAttribute(string description = null, Type paramType = null)
		{
			Description = description;
			ParamType = paramType;
		}
	}
}
