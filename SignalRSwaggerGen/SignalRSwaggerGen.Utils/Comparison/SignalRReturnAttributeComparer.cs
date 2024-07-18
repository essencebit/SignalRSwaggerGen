using SignalRSwaggerGen.Attributes;
using System.Collections.Generic;

namespace SignalRSwaggerGen.Utils.Comparison
{
	internal class SignalRReturnAttributeComparer : IEqualityComparer<SignalRReturnAttribute>
	{
		public bool Equals(SignalRReturnAttribute x, SignalRReturnAttribute y)
		{
			return x != null
				&& y != null
				&& x.StatusCode == y.StatusCode;
		}

		public int GetHashCode(SignalRReturnAttribute obj)
		{
			return obj.StatusCode;
		}
	}
}
