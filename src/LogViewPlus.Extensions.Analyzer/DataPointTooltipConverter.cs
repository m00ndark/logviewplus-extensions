using System;
using System.Globalization;
using System.Windows.Data;
using LogViewPlus.Extensions.Analyzer.Model;
using Syncfusion.UI.Xaml.Charts;

namespace LogViewPlus.Extensions.Analyzer
{
	public class DataPointTooltipConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is ChartSegment chartSegment) || !(chartSegment.Item is DataPoint dataPoint))
				return Binding.DoNothing;

			return $"X: {dataPoint.Time:yyyy-MM-dd HH:mm:ss.fff}{Environment.NewLine}Y: {dataPoint.Value:0.0}%";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
