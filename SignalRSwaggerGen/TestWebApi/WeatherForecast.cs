using System;
using TestWebApi;

namespace TestWebApi
{
	public class WeatherForecast : X.Base
	{
		public DateTime Date { get; set; }

		public int TemperatureC { get; set; }

		public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

		public string Summary { get; set; }

		public static string Static { get; set; }

		private string Private { get; set; }

		public Y.Base BaseY { get; set; }

		public MyEnum EnumField { get; set; }
	}

	public enum MyEnum
	{
		Value1 = 2,
		Value2 = 3,
	}
}
namespace X
{
	public class Base
	{
		public string Arrow => "";

		public string Inherit { get; set; }

		protected string Protected { get; set; }
	}
}

namespace Y
{
	public class Base
	{
		public string Arrow => "";

		public string Inherit { get; set; }

		protected string Protected { get; set; }

		public Base Self { get; set; }

		public MyEnum EnumField { get; set; }
	}
}