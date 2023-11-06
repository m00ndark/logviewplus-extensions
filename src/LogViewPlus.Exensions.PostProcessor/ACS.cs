using System.Collections.Generic;
using System.Linq;
using Clearcove.LogViewer.Common;

namespace LogViewPlus.Exensions.PostProcessor
{
	public class ACS : ILogPostProcessor, IColumnManagement
	{
		private const string COLUMN_LOGGER_PATH = "Logger Path";
		private const string COLUMN_STACK_TRACE = "StackTrace";

		public List<FieldColumnInfo> GetSupportedTypes()
		{
			return
				new[]
					{
						new FieldColumnInfo(ElementType.String, COLUMN_LOGGER_PATH, false, -1),
						new FieldColumnInfo(ElementType.String, COLUMN_STACK_TRACE, false, -1)
					}
				.ToList();
		}

		public void Modify(LogEntry newEntry)
		{
			// REVERT NEW LINE CHARACTER

			newEntry.OriginalLogEntry = newEntry.OriginalLogEntry.Replace('\u21B5', '\n');

			// COLUMN_LOGGER_PATH

			string logger = newEntry.Logger;
			string loggerPath = string.Empty;

			if (!string.IsNullOrEmpty(logger))
			{
				int lastSeparatorIndex = logger.LastIndexOf('\\');

				if (lastSeparatorIndex != -1)
				{
					newEntry.Logger = logger.Substring(lastSeparatorIndex + 1);
					loggerPath = logger.Substring(0, lastSeparatorIndex);
				}
			}

			newEntry.AddString(loggerPath);


			// COLUMN_STACK_TRACE

			string message = newEntry.Message;
			string stackTrace = string.Empty;

			if (!string.IsNullOrEmpty(message))
			{
				string[] lines = message.Split('\n', '\u21B5');

				newEntry.Message = lines
					.TakeWhile(line => !line.StartsWith("   at "))
					.AsSingleLine();

				stackTrace = lines
					.SkipWhile(line => !line.StartsWith("   at "))
					.TakeWhile(line => !line.StartsWith("---> (Inner Exception #"))
					.AsSingleLine();
			}

			newEntry.AddString(stackTrace);
		}
	}
}
