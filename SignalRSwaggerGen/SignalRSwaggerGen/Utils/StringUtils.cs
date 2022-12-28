using System.Text;

namespace SignalRSwaggerGen.Utils
{
	internal static class StringUtils
	{
		public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

		public static string Glue(char glue, params string[] pieces)
		{
			var firstPieceAppended = false;
			var whole = new StringBuilder();
			foreach (var piece in pieces)
			{
				if (firstPieceAppended)
				{
					whole.Append(glue);
					whole.Append(piece);
				}
				else
				{
					whole.Append(piece);
					firstPieceAppended = true;
				}
			}
			return whole.ToString();
		}
	}
}
