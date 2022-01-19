namespace SignalRSwaggerGen.Enums
{
	/// <summary>
	/// A flag indicating what components will have Swagger documentation enabled automatically
	/// </summary>
	public enum AutoDiscover
	{
		/// <summary>
		/// Inherit value from higher level configurations
		/// </summary>
		Inherit = -1,

		/// <summary>
		/// None
		/// </summary>
		None = 0,

		/// <summary>
		/// Public non-static methods
		/// </summary>
		Methods = 1,

		/// <summary>
		/// Method params
		/// </summary>
		Params = 2,

		/// <summary>
		/// Public non-static methods and their params
		/// </summary>
		MethodsAndParams = 3,
	}
}
