namespace Railcar.Client.Pages;

internal static class DateTimeFormatter
{
    public static string ToTripDetailDateTime(this DateTime value) => value.ToString("yyyy-MM-dd HH:mm");
}