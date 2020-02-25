using System.Windows;

namespace LogViewPlus.Extensions.Analyzer
{
	public partial class AnalyzerWindow : Window
	{
		public AnalyzerWindow(AnalyzerViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}
