using System.Collections.Generic;
using Clearcove.LogViewer.Common;
using LogViewPlus.Extensions.Analyzer.Model;

namespace LogViewPlus.Extensions.Analyzer
{
	public static class Extensions
	{
		public static string GetMethod(this LogEntry logEntry) => logEntry.Strings[2];

		public static void AddDataPoint(this IDictionary<string, List<DataPoint>> dataPoints, string component, DataPoint dataPoint)
		{
			if (!dataPoints.ContainsKey(component))
			{
				dataPoints.Add(component, new List<DataPoint>());
			}

			dataPoints[component].Add(dataPoint);
		}
	}
}
