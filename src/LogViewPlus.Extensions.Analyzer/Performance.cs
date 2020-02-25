using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;
using Clearcove.LogViewer.Common;

namespace LogViewPlus.Extensions.Analyzer
{
	public class Performance : ILogAnalyzer
	{
		public Performance()
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

		private static IEnumerable<DataPoint> Analyze(IReadOnlyList<LogEntry> logEntries)
		{
			Debug.WriteLine($"LogEntries.Count: {logEntries.Count}");

			DataPoint dataPoint = null;
			foreach (LogEntry logEntry in logEntries)
			{
				DataType dataType = DataType.Undefined;

				switch (logEntry.Message)
				{
					case string message when message.StartsWith("Physical RAM:"):
						dataType = DataType.MemoryUsage;
						break;
					case string message when message.StartsWith("Total CPU usage:"):
						dataType = DataType.CpuUsage;
						break;
				}

				if (dataType == DataType.Undefined)
					continue;


				if (dataPoint == null
					|| Math.Abs(logEntry.Date.Ticks - dataPoint.Time.Ticks) > TimeSpan.FromMilliseconds(500).Ticks)
				{
					//DateTime time = logEntry.Date.AddTicks(-(logEntry.Date.Ticks % TimeSpan.TicksPerSecond));
					dataPoint = new DataPoint(logEntry.Date);
				}

				switch (dataType)
				{
					case DataType.MemoryUsage:
						ParseMemoryUsage(dataPoint, logEntry.Message);
						break;
					case DataType.CpuUsage:
						ParseCpuUsage(dataPoint, logEntry.Message);
						break;
					default:
						continue;
				}

				if (dataPoint.IsComplete)
				{
					yield return dataPoint;
					Debug.WriteLine($"{dataPoint.Time:T} > MEM: {dataPoint.MemoryUsage}%, CPU: {dataPoint.CpuUsage}%");
				}
			}
		}

		private static void ParseMemoryUsage(DataPoint dataPoint, string message)
		{
			Match match = Regex.Match(message, @"^Physical RAM:\s*Total=([0-9\.]+)\s*.*Processes working set:\s*([0-9\.]+)\s*.*$");

			if (!match.Success)
				return;

			if (!double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double total))
				return;

			if (!double.TryParse(match.Groups[2].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double used))
				return;

			dataPoint.MemoryUsage = 100 * used / total;
		}

		private static void ParseCpuUsage(DataPoint dataPoint, string message)
		{
			Match match = Regex.Match(message, @"^Total CPU usage:\s*([0-9\.]+)%,.*$");

			if (!match.Success)
				return;

			if (!double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double usage))
				return;

			dataPoint.CpuUsage = usage;
		}

		public enum DataType
		{
			Undefined,
			MemoryUsage,
			CpuUsage
		}

		public class DataPoint
		{
			public DataPoint(DateTime time)
			{
				Time = time;
			}

			public DateTime Time { get; set; }
			public double MemoryUsage { get; set; } = double.MinValue;
			public double CpuUsage { get; set; } = double.MinValue;

			public bool IsComplete
				=> MemoryUsage > double.MinValue
					&& CpuUsage > double.MinValue;
		}
	}
}
