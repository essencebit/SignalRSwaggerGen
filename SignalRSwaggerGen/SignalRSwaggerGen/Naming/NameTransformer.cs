namespace SignalRSwaggerGen.Naming
{
	/// <summary>
	/// Inherit from this class in order to create a name transformer
	/// </summary>
	public abstract class NameTransformer
	{
		/// <summary>
		/// Name transformation function
		/// </summary>
		/// <param name="name">The name to transform</param>
		/// <returns>Transformed name</returns>
		public abstract string Transform(string name);
	}
}
