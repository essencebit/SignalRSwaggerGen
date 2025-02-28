using SignalRSwaggerGen.Utils.Comparison;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SignalRSwaggerGen.Utils
{
	internal static class ReflectionUtils
	{
		public const BindingFlags DeclaredPublicInstance = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
		public const BindingFlags PublicInstanceMethod = BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod;
		public const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

		private static readonly BySignatureMethodComparer _bySignatureMethodComparer = new BySignatureMethodComparer();

		public static MethodInfo[] GetMethodsExcludingInherited(this Type type)
		{
			return type.GetMethods(DeclaredPublicInstance)
				.Where(x => !x.IsSpecialName)
				.ToArray();
		}

		public static MethodInfo[] GetMethodsIncludingInherited(this Type type, HashSet<Type> declaringTypesOfMethodsToIgnore)
		{
			return type.IsInterface
				? type.GetInterfaceMethodsIncludingInherited(declaringTypesOfMethodsToIgnore)
				: type.GetNonInterfaceMethodsIncludingInherited(declaringTypesOfMethodsToIgnore);
		}

		private static MethodInfo[] GetNonInterfaceMethodsIncludingInherited(this Type type, HashSet<Type> declaringTypesOfMethodsToIgnore)
		{
			var set = new HashSet<MethodInfo>(_bySignatureMethodComparer); // helps skipping shadowed methods

			var declaredMethods = type.GetMethods(DeclaredPublicInstance)
				.Where(x => !x.IsSpecialName);

			foreach (var method in declaredMethods)
				set.Add(method);

			var inheritedMethods = type.GetMethods(PublicInstance)
				.Where(x => !x.IsSpecialName)
				.Where(x =>
					x.DeclaringType != type
					&& x.DeclaringType != typeof(object)
					&& !declaringTypesOfMethodsToIgnore.Contains(x.DeclaringType));

			foreach (var method in inheritedMethods)
				set.Add(method);

			return set.ToArray();
		}

		private static MethodInfo[] GetInterfaceMethodsIncludingInherited(this Type type, HashSet<Type> declaringTypesOfMethodsToIgnore)
		{
			return type.GetInterfaceMethodsIncludingInherited(new HashSet<Type>(), declaringTypesOfMethodsToIgnore);
		}

		private static MethodInfo[] GetInterfaceMethodsIncludingInherited(this Type type, HashSet<Type> processedInterfaces, HashSet<Type> declaringTypesOfMethodsToIgnore)
		{
			if (processedInterfaces.Contains(type) || declaringTypesOfMethodsToIgnore.Contains(type))
				return Array.Empty<MethodInfo>();

			processedInterfaces.Add(type);

			var set = new HashSet<MethodInfo>(_bySignatureMethodComparer); // helps skipping shadowed methods

			var declaredMethods = type.GetMethods(DeclaredPublicInstance)
				.Where(x => !x.IsSpecialName);

			foreach (var method in declaredMethods)
				set.Add(method);

			var inheritedMethods = type.GetInterfaces()
				.SelectMany(x => x.GetInterfaceMethodsIncludingInherited(processedInterfaces, declaringTypesOfMethodsToIgnore));

			foreach (var method in inheritedMethods)
				set.Add(method);

			return set.ToArray();
		}
	}
}
