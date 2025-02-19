public static class TimeUtils
{
    /// <summary>
    /// Convert an amount of elapsed time to a time display string.
    /// e.g. 0 -> 09:00 AM
    /// </summary>
    public static string ElapsedTimeToDisplay(int value)
    {
        int actualClockHours = 9 + value / 60;
        int displayClockHours = actualClockHours >= 13 ? actualClockHours - 12 : actualClockHours;
        int clockMinutes = value % 60;
        string ampm = actualClockHours >= 12 ? "PM" : "AM";

        return $"{displayClockHours:D2}:{clockMinutes:D2} {ampm}";
    }

    /// <summary>
    /// Convert an amount of elapsed time to a time display string.
    /// e.g. 0 -> 09:00 AM
    /// </summary>
    public static string ElapsedTimeToDisplay(float value) => ElapsedTimeToDisplay((int)value);
}
