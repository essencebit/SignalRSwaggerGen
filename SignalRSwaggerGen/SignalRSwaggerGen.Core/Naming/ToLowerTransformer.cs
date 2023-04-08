using SignalRSwaggerGen.Utils;

namespace SignalRSwaggerGen.Naming
{
	public class ToLowerTransformer : NameTransformer
	{
		/// <summary>
		/// Transforms the name to lower case
		/// </summary>
		/// <param name="name">The name to transform</param>
		/// <returns>Lower case name</returns>
		public override string Transform(string name)
		{
			if (name.IsNullOrEmpty()) return name;
			return name.ToLowerInvariant();
		}
	}
}
