namespace RebelCmsTemplate.Util;

public static class CustomDateTimeConvert
{
    public static DateOnly? ConvertToDate(DateTime? dateString)
    {
        if (dateString is not { })
        {
            return null;
        }

        return new DateOnly(dateString.Value.Year, dateString.Value.Month, dateString.Value.Day);
    }

    public static TimeOnly? ConvertToTime(DateTime? timeString)
    {
        if (timeString is not { })
        {
            return null;
        }

        return new TimeOnly(timeString.Value.Hour, timeString.Value.Minute, timeString.Value.Second);
    }
}