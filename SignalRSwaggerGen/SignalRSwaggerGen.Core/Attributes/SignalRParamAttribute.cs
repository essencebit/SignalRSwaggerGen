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
		public bool Deprecated { get; }

		/// <param name="description">The text that will appear in description section of decorated parameter in Swagger doc</param>
		/// <param name="paramType">Parameter type. If null specified, the type of the parameter holding this attribute will be used.</param>
		/// <param name="deprecated">A flag which indicates if the decorated parameter will be marked as deprecated in Swagger doc</param>
		public SignalRParamAttribute(string description = null, Type paramType = null, bool deprecated = false)
		{
			Description = description;
			ParamType = paramType;
			Deprecated = deprecated;
		}
	}
}
