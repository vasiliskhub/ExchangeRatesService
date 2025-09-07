using System.Runtime.InteropServices;
using ZiggyCreatures.Caching.Fusion;

namespace ExchangeRateProviders.Czk.Config
{
	/// <summary>
	/// Provides intelligent caching strategies for Czech National Bank (CNB) exchange rate data
	/// based on their publication schedule and Prague timezone.
	/// Strategy:
	///   - 13:30–14:30 (weekday pre-publication): 1 minute (avoid carrying stale data past 14:30)
	///   - 14:30–15:30 (weekday publication window): 5 minutes
	///   - Other weekdays: 1 hour
	///   - Weekends: 14.5 hours (no new data)
	/// </summary>
	public static class CnbCacheStrategy
	{
		public static FusionCacheEntryOptions GetCacheOptionsBasedOnPragueTime()
		{
			var pragueTime = GetPragueTime();

			TimeSpan duration;

			if (IsWeekend(pragueTime))
			{
				// Weekends: CNB never releases new data, cache for 14.5 hours to avoid unnecessary API calls
				duration = TimeSpan.FromHours(14.5);
			}
			else if (IsBeforePublishWindow(pragueTime))
			{
				// Shortly before publication window (1:30 PM - 2:30 PM): very short to avoid stale carry-over
				duration = TimeSpan.FromMinutes(1);
			}
			else if (IsWithinPublishWindow(pragueTime))
			{
				// Publication window (2:30 PM - 3:30 PM): refresh frequently to catch new data
				duration = TimeSpan.FromMinutes(5);
			}
			else
			{
				// Outside publication and pre-window: data is stable
				duration = TimeSpan.FromHours(1);
			}

			return new FusionCacheEntryOptions
			{
				Duration = duration,
				FailSafeMaxDuration = TimeSpan.FromTicks(duration.Ticks)
			};
		}

		private static DateTime GetPragueTime()
		{
			var pragueTimeZone = GetPragueTimeZone();
			return TimeZoneInfo.ConvertTime(DateTime.UtcNow, pragueTimeZone);
		}

		private static bool IsWithinPublishWindow(DateTime pragueTime)
		{
			var time = pragueTime.TimeOfDay;
			return time >= new TimeSpan(14, 30, 0) && time <= new TimeSpan(15, 30, 0);
		}

		private static bool IsBeforePublishWindow(DateTime pragueTime)
		{
			var time = pragueTime.TimeOfDay;
			return time >= new TimeSpan(13, 30, 0) && time < new TimeSpan(14, 30, 0);
		}

		private static bool IsWeekend(DateTime pragueTime)
			=> pragueTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

		// Support only for Windows and Linux/MacOS timezone identifiers
		private static TimeZoneInfo GetPragueTimeZone()
		{
			string timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
				? "Central Europe Standard Time"
				: "Europe/Prague";

			return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
		}
	}
}