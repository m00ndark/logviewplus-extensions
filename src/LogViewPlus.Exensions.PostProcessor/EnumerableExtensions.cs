using System.Collections.Generic;

namespace LogViewPlus.Exensions.PostProcessor
{
	public static class EnumerableExtensions
	{
		public static string AsSingleLine(this IEnumerable<string> lines)
		{
			// \u21B5 == ↵
			return string.Join("\u21B5", lines);
		}
	}
}
