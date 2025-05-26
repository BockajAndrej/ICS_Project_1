namespace ICS_Project.App.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToFormattedDuration(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalHours >= 1)
            {
                // Format as H:MM:SS e.g., 1:03:20
                return $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }
            // No hours, format as M:SS or 0:SS e.g., 3:20 or 0:45
            return $"{timeSpan.Minutes}:{timeSpan.Seconds:D2}";
        }
    }
}