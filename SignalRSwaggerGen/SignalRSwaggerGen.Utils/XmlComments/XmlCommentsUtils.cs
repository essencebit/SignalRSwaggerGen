using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SignalRSwaggerGen.Utils.XmlComments
{
	internal static class XmlCommentsUtils
	{
		public static string GetXmlCommentsName(this Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			var result = $"T:{GetTypeName(type)}";

			return result;
		}

		public static string GetXmlCommentsName(this MethodInfo method)
		{
			if (method == null) throw new ArgumentNullException(nameof(method));

			var result = $"M:{GetTypeName(method.DeclaringType)}.{method.Name}";

			if (method.IsGenericMethodDefinition)
			{
				result += $"``{method.GetGenericArguments().Length}";
			}

			var parameters = method.GetParameters();
			if (parameters.Length == 0) return result;

			result += "(";
			for (int i = 0; i < parameters.Length; i++)
			{
				if (i > 0) result += ",";
				result += GetParamTypeName(parameters[i].ParameterType);
			}
			result += ")";

			return result;
		}

		private static string GetTypeName(Type type)
		{
			return Regex.Replace(type.FullName ?? $"{type.Namespace}.{type.Name}", @"\[.*\]", string.Empty).Replace('+', '.');
		}

		private static string GetParamTypeName(Type paramType)
		{
			if (paramType.HasElementType) return GetWithElementParamTypeName(paramType);
			if (paramType.IsGenericParameter) return GetGenericParameterParamTypeName(paramType);
			if (paramType.IsGenericType) return GetGenericParamTypeName(paramType);
			return GetSimpleParamTypeName(paramType);
		}

		private static string GetWithElementParamTypeName(Type paramType)
		{
			if (paramType.IsByRef) return GetByRefParamTypeName(paramType);
			if (paramType.IsPointer) return GetPointerParamTypeName(paramType);
			if (paramType.IsArray) return GetArrayParamTypeName(paramType);
			throw new NotSupportedException($"Type [{paramType.AssemblyQualifiedName}] not supported");
		}

		private static string GetByRefParamTypeName(Type paramType)
		{
			return GetParamTypeName(paramType.GetElementType()) + "@";
		}

		private static string GetPointerParamTypeName(Type paramType)
		{
			return GetParamTypeName(paramType.GetElementType()) + "*";
		}

		private static string GetArrayParamTypeName(Type paramType)
		{
			var arrayRank = paramType.GetArrayRank();
			var arrayTypeSpecifier = arrayRank == 1
				? "[]"
				: $"[{string.Join(",", Enumerable.Range(0, arrayRank).Select(x => "0:"))}]";

			return $"{GetParamTypeName(paramType.GetElementType())}{arrayTypeSpecifier}";
		}

		private static string GetGenericParameterParamTypeName(Type paramType)
		{
#if NETSTANDARD2_1
			if (paramType.IsGenericMethodParameter) return $"``{paramType.GenericParameterPosition}";
			if (paramType.IsGenericTypeParameter) return $"`{paramType.GenericParameterPosition}";
			throw new NotSupportedException($"Type [{paramType.AssemblyQualifiedName}] not supported");
#else
			throw new NotSupportedException("Certain API required for further execution is missing. Target netstandard2.1 to get rid of this exception.");
#endif
		}

		private static string GetGenericParamTypeName(Type paramType)
		{
			var genericArgs = paramType.GetGenericArguments();
			if (paramType.IsNested) return GetNestedGenericParamTypeName(paramType, genericArgs);
			return $"{(paramType.FullName ?? $"{paramType.Namespace}.{paramType.Name}").Split('`')[0]}{{{string.Join(",", genericArgs.Select(x => GetParamTypeName(x)))}}}";
		}

		private static string GetNestedGenericParamTypeName(Type paramType, IEnumerable<Type> allGenericArgs)
		{
			var declaringType = paramType.DeclaringType;
			var declaringTypeGenericArgsCount = declaringType.GetGenericArguments().Length;
			var declaringTypeName = !declaringType.IsGenericType
				? GetParamTypeName(declaringType)
				: GetNestedGenericParamTypeName(declaringType, allGenericArgs.Take(declaringTypeGenericArgsCount));

			var genericArgsCount = paramType.GetGenericArguments().Length - declaringTypeGenericArgsCount;
			var genericArgsNames = genericArgsCount > 0
				? $"{{{string.Join(",", allGenericArgs.TakeLast(genericArgsCount).Select(x => GetParamTypeName(x)))}}}"
				: "";

			return $"{declaringTypeName}.{paramType.Name.Split('`')[0]}{genericArgsNames}";
		}

		private static string GetSimpleParamTypeName(Type paramType)
		{
			if (paramType.IsNested) return $"{GetParamTypeName(paramType.DeclaringType)}.{paramType.Name}";
			return paramType.FullName ?? $"{paramType.Namespace}.{paramType.Name}";
		}
	}
}
