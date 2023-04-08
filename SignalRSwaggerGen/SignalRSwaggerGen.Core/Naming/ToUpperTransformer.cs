using SignalRSwaggerGen.Utils;

namespace SignalRSwaggerGen.Naming
{
	public class ToUpperTransformer : NameTransformer
	{
		/// <summary>
		/// Transforms the name to upper case
		/// </summary>
		/// <param name="name">The name to transform</param>
		/// <returns>Upper case name</returns>
		public override string Transform(string name)
		{
			if (name.IsNullOrEmpty()) return name;
			return name.ToUpperInvariant();
		}
	}
}
