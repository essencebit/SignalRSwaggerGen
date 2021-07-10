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

		/// <param name="description">The text that will appear in description section of decorated argument in Swagger document</param>
		public SignalRArgAttribute(string description = null)
		{
			Description = description;
		}
	}
}
