namespace SignalRSwaggerGen.Enums
{
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
		/// Public non-static method args
		/// </summary>
		Args,

		/// <summary>
		/// Public non-static methods and their args
		/// </summary>
		MethodsAndArgs
	}
}
