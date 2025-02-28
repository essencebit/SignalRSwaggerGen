using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SignalRSwaggerGen.Utils.Comparison
{
	internal class BySignatureMethodComparer : IEqualityComparer<MethodInfo>
	{
		public bool Equals(MethodInfo x, MethodInfo y)
		{
			if (x == null || y == null) return false;
			if (x.Name != y.Name) return false;
			var xParameters = x.GetParameters();
			var yParameters = y.GetParameters();
			if (xParameters.Length != yParameters.Length) return false;
			for (var i = 0; i < xParameters.Length; i++)
				if (xParameters[i].ParameterType != yParameters[i].ParameterType) return false;
			return true;
		}

		public int GetHashCode(MethodInfo obj)
		{
			return GetMethodSignature(obj).GetHashCode();
		}

		private static string GetMethodSignature(MethodInfo obj)
		{
			return obj.Name + '(' + string.Join(",", obj.GetParameters().Select(x => x.ParameterType.FullName ?? x.ParameterType.Name));
		}
	}
}
