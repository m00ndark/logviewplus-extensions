using System.Collections;
using System.Windows;
using Syncfusion.UI.Xaml.Charts;

namespace LogViewPlus.Extensions.Analyzer
{
	public class SfChartExt : SfChart
	{
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register(nameof(Source), typeof(object), typeof(SfChartExt), new PropertyMetadata(null, OnPropertyChanged));

		public static readonly DependencyProperty SeriesTemplateProperty =
			DependencyProperty.Register(nameof(SeriesTemplate), typeof(DataTemplate), typeof(SfChartExt), new PropertyMetadata(null, OnPropertyChanged));

		public object Source
		{
			get => GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		public DataTemplate SeriesTemplate
		{
			get => (DataTemplate) GetValue(SeriesTemplateProperty);
			set => SetValue(SeriesTemplateProperty, value);
		}

		private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as SfChartExt)?.GenerateSeries();
		}

		private void GenerateSeries()
		{
			if (!(Source is IEnumerable sourceCollection) || SeriesTemplate == null)
				return;

			Series.Clear();

			foreach (object source in sourceCollection)
			{
				ChartSeries series = (ChartSeries) SeriesTemplate.LoadContent();
				series.DataContext = source;
				Series.Add(series);
			}
		}
	}
}
