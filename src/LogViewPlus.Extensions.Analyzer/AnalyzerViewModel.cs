using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clearcove.LogViewer.Common;

namespace LogViewPlus.Extensions.Analyzer
{
	public class AnalyzerViewModel
	{
		private readonly Func<IEnumerable<Performance.DataPoint>> _analyze;
		private ObservableCollection<Performance.DataPoint> _dataPoints;

		public AnalyzerViewModel(Func<IEnumerable<Performance.DataPoint>> analyze)
		{
			_analyze = analyze;
		}

		public ObservableCollection<Performance.DataPoint> DataPoints
		{
			get
			{
				if (_dataPoints == null)
				{
					_dataPoints = new ObservableCollection<Performance.DataPoint>(_analyze());
					Debug.WriteLine($"DataPoints.Count: {_dataPoints.Count}");
				}

				return _dataPoints;
			}
		}

		public int MaxPercentage => (int) Math.Min(100, Math.Ceiling((DataPoints.Max(x => x.MaxUsageValue) + 2.5) / 5d) * 5);
	}
}
