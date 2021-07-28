using System;

namespace SignalRSwaggerGen.Attributes
{
	/// <summary>
	/// Use this attribute to disable Swagger documentation for specific components
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct
		| AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.ReturnValue,
		AllowMultiple = false, Inherited = false)]
	public sealed class SignalRHiddenAttribute : Attribute
	{
	}
}
