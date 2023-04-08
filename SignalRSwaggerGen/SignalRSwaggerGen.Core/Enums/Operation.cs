namespace SignalRSwaggerGen.Enums
{
	/// <summary>
	/// Same as HTTP verb
	/// </summary>
	public enum Operation
	{
		/// <summary>
		/// Inherit value from higher level configurations
		/// </summary>
		Inherit = -1,
		Get = 0,
		Put = 1,
		Post = 2,
		Delete = 3,
		Options = 4,
		Head = 5,
		Patch = 6,
		Trace = 7,
	}
}
