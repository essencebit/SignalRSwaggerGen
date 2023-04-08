using System;

namespace SignalRSwaggerGen.Attributes
{
	/// <summary>
	/// Use this attribute to enable Swagger documentation for method return type
	/// </summary>
	[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
	public sealed class SignalRReturnAttribute : Attribute
	{
		public Type ReturnType { get; }
		public int StatusCode { get; }
		public string Description { get; }

		/// <param name="returnType">Return type. If null specified, the return type of the method will be picked up.</param>
		/// <param name="statusCode">The text that will appear in status code section of the corresponding response in Swagger document</param>
		/// <param name="description">The text that will appear in description section of the corresponding response in Swagger document</param>
		public SignalRReturnAttribute(Type returnType = null, int statusCode = 200, string description = "Success")
		{
			ReturnType = returnType;
			StatusCode = statusCode;
			Description = description;
		}
	}
}
