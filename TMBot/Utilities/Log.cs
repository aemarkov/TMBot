using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMBot.Utilities
{
	/// <summary>
	/// Простой класс для логгирования
	/// </summary>
	public class Log
	{
		public enum Level
		{
			DEBUG,
			WARNING,
			ERROR
		}

		public static event Action<string, Level>  NewLogMessage;

		public static void d(string format, params object[] args)
		{
			Debug.WriteLine(format, args);
			string text = String.Format(format, args);
			NewLogMessage?.Invoke(text, Level.DEBUG);
		}

		public static void d(string text)
		{
			Debug.WriteLine(text);
			NewLogMessage?.Invoke(text, Level.DEBUG);
		}


		public static void w(string format, params object[] args)
		{
			Debug.WriteLine("WARNING: " + format, args);
			string text = String.Format(format, args);
			NewLogMessage?.Invoke(text, Level.WARNING);
		}

		public static void w(string text)
		{
			Debug.WriteLine("WARNING: " + text);
			NewLogMessage?.Invoke(text, Level.WARNING);
		}

		public static void e(string format, params object[] args)
		{
			Debug.WriteLine("ERROR: "+format, args);
			string text = String.Format(format, args);
			NewLogMessage?.Invoke(text, Level.ERROR);
		}

		public static void e(string text)
		{
			Debug.WriteLine("ERROR: "+text);
			NewLogMessage?.Invoke(text, Level.ERROR);
		}
	}
}
