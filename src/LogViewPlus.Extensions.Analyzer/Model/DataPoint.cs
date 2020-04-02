using System;

namespace LogViewPlus.Extensions.Analyzer.Model
{
	public class DataPoint
	{
		public DataPoint(DateTime time, double value)
		{
			Time = time;
			Value = value;
		}

		public DateTime Time { get; set; }
		public double Value { get; set; }
	}
}