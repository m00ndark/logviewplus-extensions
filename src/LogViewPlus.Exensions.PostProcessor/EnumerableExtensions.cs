using System;
using System.Collections.Generic;
using System.Text;

namespace LogViewPlus.Exensions.PostProcessor
{
	public static class EnumerableExtensions
	{
		public static string AsSingleLine(this IEnumerable<string> lines)
		{
			return string.Join("↲↓", lines);
		}
	}
}
