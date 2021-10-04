namespace SignalRSwaggerGen.Enums
{
	/// <summary>
	/// A flag indicating what components will have Swagger documentation enabled automatically
	/// </summary>
	public enum AutoDiscover
	{
		/// <summary>
		/// None
		/// </summary>
		None = 0,

		/// <summary>
		/// Public non-static methods
		/// </summary>
		Methods,

		/// <summary>
		/// Method args
		/// </summary>
		Args,

		/// <summary>
		/// Public non-static methods and their args
		/// </summary>
		MethodsAndArgs
	}
}
