using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;

namespace SignalRSwaggerGen
{
	internal static class BusinessUtils
	{
		public static string GetName(this Type type)
		{
			return type.FullName ?? type.Name;
		}

		public static bool IsFromBody(this ParameterInfo parameterInfo)
		{
			return !parameterInfo.IsFromForm()
				&& parameterInfo.GetCustomAttribute<FromBodyAttribute>() != null;
		}

		public static bool IsFromForm(this ParameterInfo parameterInfo)
		{
			return parameterInfo.GetCustomAttribute<FromFormAttribute>() != null;
		}

		public static bool IsFormFile(this Type type)
		{
			return typeof(IFormFile).IsAssignableFrom(type);
		}
	}
}
