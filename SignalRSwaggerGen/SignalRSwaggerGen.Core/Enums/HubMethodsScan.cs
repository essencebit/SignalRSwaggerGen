namespace SignalRSwaggerGen.Enums
{
	/// <summary>
	/// A flag indicating what hub methods must be included in Swagger documentation
	/// </summary>
	public enum HubMethodsScan
	{
		/// <summary>
		/// Inherit value from higher level configurations
		/// </summary>
		Inherit = -1,

		/// <summary>
		/// Public instance methods declared directly in the hub
		/// </summary>
		Default = 0,

		/// <summary>
		/// Include inherited methods as well, not only the ones declared in the hub
		/// </summary>
		IncludeInherited = 1,
	}
}
