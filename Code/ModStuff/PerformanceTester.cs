using System.Diagnostics;

namespace ModStuff
{
	public static class PerformanceTester
	{
		public static Stopwatch StartStopwatch()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			return stopwatch;
		}

		public static float StopStopwatch(Stopwatch stopwatch)
		{
			stopwatch.Stop();
			return stopwatch.ElapsedMilliseconds;
		}
	}
}