using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clearcove.LogViewer.Common;
using LogViewPlus.Extensions.Analyzer.Model;

namespace LogViewPlus.Extensions.Analyzer
{
	public class AnalyzerViewModel
	{
		private readonly Func<IDictionary<string, List<DataPoint>>> _analyze;
		private ObservableCollection<ChartData> _dataPoints;

		public AnalyzerViewModel(Func<IDictionary<string, List<DataPoint>>> analyze)
		{
			_analyze = analyze;
		}

		public ObservableCollection<ChartData> ChartData
		{
			get
			{
				if (_dataPoints == null)
				{
					_dataPoints = new ObservableCollection<ChartData>(_analyze()
						.Select(x => new ChartData
							{
								Component = x.Key,
								DataPoints = new ObservableCollection<DataPoint>(x.Value)
							}));
					Debug.WriteLine($"DataPoints.Count: {_dataPoints.Count}");
				}

				return _dataPoints;
			}
		}

		public int MaxPercentage => (int) Math.Min(100, Math.Ceiling((ChartData.SelectMany(x => x.DataPoints).Max(x => x.Value) + 2.5) / 5d) * 5);
	}
}
