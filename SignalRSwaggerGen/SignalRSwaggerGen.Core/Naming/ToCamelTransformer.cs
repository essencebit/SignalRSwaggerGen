using SignalRSwaggerGen.Utils;

namespace SignalRSwaggerGen.Naming
{
	public class ToCamelTransformer : NameTransformer
	{
		/// <summary>
		/// Transforms the name to camel case
		/// </summary>
		/// <param name="name">The name to transform</param>
		/// <returns>Camel case name</returns>
		public override string Transform(string name)
		{
			if (name.IsNullOrEmpty()) return name;
			return char.ToLowerInvariant(name[0]) + name.Substring(1);
		}
	}
}
