using System;

namespace SignalRSwaggerGen.Attributes
{
	/// <summary>
	/// Use this attribute to enable Swagger documentation for request body
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class SignalRRequestBodyAttribute : Attribute
	{
		public Type BodyType { get; }
		public bool IsRequired { get; }
		public string Description { get; }

		/// <param name="bodyType">The type of the request body</param>
		/// <param name="isRequired">The value that indicates if the request body is required or not</param>
		/// <param name="description">The text that will appear in description section of the corresponding request body in Swagger document</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="bodyType"/> is null</exception>
		public SignalRRequestBodyAttribute(Type bodyType, bool isRequired = false, string description = null)
		{
			BodyType = bodyType ?? throw new ArgumentNullException(nameof(bodyType));
			IsRequired = isRequired;
			Description = description;
		}
	}
}
