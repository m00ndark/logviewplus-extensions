using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;
using Clearcove.LogViewer.Common;
using LogViewPlus.Extensions.Analyzer.Model;

namespace LogViewPlus.Extensions.Analyzer
{
	public class ACS : ILogAnalyzer
	{
		private const string PERFORMANCE_MEMORY_USAGE = "Memory Usage";
		private const string PERFORMANCE_CPU_USAGE = "CPU Usage";

		private static readonly IDictionary<string, int> _unitConvertion = new Dictionary<string, int>
			{
				{ "B", -20 },
				{ "KB", -10 },
				{ "MB", 0 },
				{ "GB", 10 },
			};

		public ACS()
		{
			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjE0NDQzQDMxMzcyZTM0MmUzMGFIVm5XbVpWOFByaU9sU01haTlORURrU3JndHp3bnh4a0ZNdHpRTi92WFE9");
		}

		public void Analyze(object ownerWindow, IReadOnlyList<LogEntry> logEntries)
		{
			Thread thread = new Thread(() =>
				{
					AnalyzerViewModel viewModel = new AnalyzerViewModel(() => Analyze(logEntries));
					AnalyzerWindow window = new AnalyzerWindow(viewModel);
					window.Show();
					window.Closed += (s, e) => window.Dispatcher?.InvokeShutdown();
					Dispatcher.Run();
				});

			thread.SetApartmentState(ApartmentState.STA);
			thread.IsBackground = true;
			thread.Start();
		}

		private static IDictionary<string, List<DataPoint>> Analyze(IReadOnlyList<LogEntry> logEntries)
		{
			Debug.WriteLine($"LogEntries.Count: {logEntries.Count}");

			IDictionary<string, List<DataPoint>> dataPoints = new Dictionary<string, List<DataPoint>>();

			foreach (LogEntry logEntry in logEntries)
			{
				if (IsMemoryUsageEntry(logEntry))
				{
					if (TryParseMemoryUsage(logEntry, out double memoryValue))
					{
						dataPoints.AddDataPoint(PERFORMANCE_MEMORY_USAGE, new DataPoint(logEntry.Date, memoryValue));
					}

					continue;
				}

				if (IsCpuUsageEntry(logEntry))
				{
					if (TryParseCpuUsage(logEntry, out double cpuValue))
					{
						dataPoints.AddDataPoint(PERFORMANCE_CPU_USAGE, new DataPoint(logEntry.Date, cpuValue));
					}

					continue;
				}

				if (IsNetworkUsageEntry(logEntry))
				{
					if (TryParseNetworkUsage(logEntry, out string interfaceName, out double cpuValue))
					{
						dataPoints.AddDataPoint(interfaceName, new DataPoint(logEntry.Date, cpuValue));
					}

					continue;
				}
			}

			return dataPoints;
		}

		private static bool IsMemoryUsageEntry(LogEntry logEntry)
			=> logEntry.GetMethod() == "LogMemoryUsage" && logEntry.Message.StartsWith("Physical RAM:");

		private static bool IsCpuUsageEntry(LogEntry logEntry)
			=> logEntry.GetMethod() == "LogProcessCpuUsage" && logEntry.Message.StartsWith("Total CPU usage:");

		private static bool IsNetworkUsageEntry(LogEntry logEntry)
			=> logEntry.GetMethod() == "LogNetworkUsage" && logEntry.Message.StartsWith("NetIf");

		private static bool TryParseMemoryUsage(LogEntry logEntry, out double value)
		{
			value = double.MinValue;
			Match match = Regex.Match(logEntry.Message, @"^Physical RAM:\s*Total=([0-9\.]+)\s*.*Processes working set:\s*([0-9\.]+)\s*.*$");

			if (!match.Success)
				return false;

			if (!double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double total))
				return false;

			if (!double.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double used))
				return false;

			value = 100 * used / total;
			return true;
		}

		private static bool TryParseCpuUsage(LogEntry logEntry, out double value)
		{
			value = double.MinValue;
			Match match = Regex.Match(logEntry.Message, @"^Total CPU usage:\s*([0-9\.]+)%,.*$");

			if (!match.Success)
				return false;

			if (!double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double usage))
				return false;

			value = usage;
			return true;
		}

		private static bool TryParseNetworkUsage(LogEntry logEntry, out string interfaceName, out double value)
		{
			interfaceName = null;
			value = double.MinValue;
			Match match = Regex.Match(logEntry.Message, @"^NetIf\s+'([^']+)':.*Total:\s+([0-9\.]+)\s+([a-zA-Z]+)/s.*$");

			if (!match.Success)
				return false;

			interfaceName = match.Groups[1].Value;

			if (!double.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double usage))
				return false;

			if (!_unitConvertion.TryGetValue(match.Groups[3].Value, out int factor))
				return false;

			value = usage * Math.Pow(2, factor);
			return true;
		}
	}
}
