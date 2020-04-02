using System.Collections.ObjectModel;

namespace LogViewPlus.Extensions.Analyzer.Model
{
	public class ChartData
	{
		public string Component { get; set; }
		public ObservableCollection<DataPoint> DataPoints { get; set; }
	}
}