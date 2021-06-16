using SignalRSwaggerGen.Utils;
using System;

namespace SignalRSwaggerGen.Attributes
{
	/// <summary>
	/// Use this attribute to enable Swagger documentation for hubs
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class SignalRHubAttribute : Attribute
	{
		internal string Path { get; }

		/// <param name="path">Relative path of the hub</param>
		/// <exception cref="ArgumentException">Thrown if path is null or empty</exception>
		public SignalRHubAttribute(string path = Constants.DefaultHubPath)
		{
			if (path.IsNullOrEmpty()) throw new ArgumentException("Path is null or empty", nameof(path));
			Path = path;
		}
	}
}
