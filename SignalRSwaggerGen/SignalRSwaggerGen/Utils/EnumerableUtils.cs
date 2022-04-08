using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRSwaggerGen.Utils
{
	internal static class EnumerableUtils
	{
		public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> enumerable, int takeCount)
		{
			if (takeCount < 0) throw new ArgumentException("Less than zero", nameof(takeCount));
			if (enumerable == null) return null;
			var enumerableCount = enumerable.Count();
			var skipCount = enumerableCount <= takeCount ? 0 : enumerableCount - takeCount;
			return enumerable.Skip(skipCount).Take(takeCount);
		}
	}
}
